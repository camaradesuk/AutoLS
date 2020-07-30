using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using SyRF.LiteratureSearch.Endpoint.DTOs;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Services
{
    public class BiorxivService : IBiorxivService
    {
        public BiorxivService(HttpClient client, IFileService fileService, ILsUnitOfWork lsUnitOfWork)
        {
            _client = client;
            _fileService = fileService;
            _lsUnitOfWork = lsUnitOfWork;
        }

        private readonly IFileService _fileService;
        private readonly HttpClient _client;
        private readonly ILsUnitOfWork _lsUnitOfWork;

        public async IAsyncEnumerable<BiorxivHtmlFileInfoDto> FindNewBiorxivStudiesAndSave(string rssFeedUrl,
            Guid livingSearchId, Guid fileId, Guid projectId, string description, int batchSize)
        {
            var fileNumber = 1;
            var studyNumber = 0;
            var nodes = new Collection<HtmlNode>();
            var biorxivStudyReferences = new Collection<BiorxivStudyReference>();
            var stream = await GetStream(rssFeedUrl);
            var web = new HtmlWeb();
            using var result = new StreamReader(stream);
            var xmlReaderSettings = new XmlReaderSettings()
            {
                Async = true
            };
            using var reader = XmlReader.Create(stream, xmlReaderSettings);

            await reader.MoveToContentAsync();
            XNamespace xmlns = reader[6];
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
                {
                    if (!(XNode.ReadFrom(reader) is XElement el)) continue;
                    var doi = el.Descendants(xmlns + "identifier").First().Value.Split(":")[1];
                    var studyPageUrl = el.FirstAttribute.Value;
                    if (await _lsUnitOfWork.BiorxivStudyReferenceRepository.ContainsReferenceWith(projectId, doi))
                        continue;
                    var studyId = Guid.NewGuid();
                    el.Add(new XElement("StudyId", studyId.ToString()));
                    biorxivStudyReferences.Add(new BiorxivStudyReference(studyId, projectId, livingSearchId, doi,
                        studyPageUrl));
                    HtmlDocument studyPage;
                    try
                    {
                        studyPage = await web.LoadFromWebAsync(studyPageUrl);
                        nodes.Add(studyPage.DocumentNode.SelectSingleNode("//head"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            $"An error occured while fetching study with doi: {doi} from BioRxiv. Error message: {e.Message}");
                        studyPage = new HtmlDocument();
                        studyPage.DocumentNode.SelectSingleNode("html").AppendChild(
                            HtmlNode.CreateNode(
                                $"<head>An error occured while getting study with DOI: {doi} from BioRxiv. Error message: {e.Message}</head>"));
                        nodes.Add(studyPage.DocumentNode.SelectSingleNode("//head"));
                    }

                    studyNumber++;
                    if (studyNumber != batchSize) continue;
                    var fileUri = await SaveBatchOfBiorxivHeadNodes(nodes, projectId, livingSearchId,
                        fileId, description, fileNumber);
                    await _lsUnitOfWork.SaveManyAsync(biorxivStudyReferences);
                    nodes.Clear();
                    biorxivStudyReferences.Clear();
                    yield return new BiorxivHtmlFileInfoDto(fileUri, fileNumber, studyNumber);
                    studyNumber = 0;
                    fileNumber++;
                }
                else
                {
                    await reader.ReadAsync();
                }
            }

            if (studyNumber != 0)
            {
                var fileUri = await SaveBatchOfBiorxivHeadNodes(nodes, projectId, livingSearchId,
                    fileId, description, fileNumber);
                await _lsUnitOfWork.SaveManyAsync(biorxivStudyReferences);
                yield return new BiorxivHtmlFileInfoDto(fileUri, fileNumber, studyNumber);
            }
        }

        private async Task<string> SaveBatchOfBiorxivHeadNodes(IEnumerable<HtmlNode> headNodes, Guid projectId,
            Guid searchId, Guid fileId, string description, int fileNumber)
        {
            var relativePath =
                $"{projectId}/Biorxiv Searches/{searchId}/Biorxiv Search - {searchId} - {fileNumber}.xml";
            var settings = new XmlWriterSettings {Indent = true, Async = true};
            var (fileSaveStream, saveTask) =
                await _fileService.CreateFileSaveStreamAsync(relativePath, fileId, description, "text/xml");
            using var writer = XmlWriter.Create(fileSaveStream as Stream, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "BioRxivStudySet", null);

            foreach (var headNode in headNodes)
            {
                await writer.WriteRawAsync(headNode.InnerHtml);
            }

            await writer.WriteEndDocumentAsync();
            await writer.FlushAsync();
            writer.Close();
            await saveTask;

            return relativePath;
        }

        private async Task<Stream> GetStream(string rssFeedUrl)
        {
            try
            {
                return await _client.GetStreamAsync(rssFeedUrl);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Cannot get RSS feed from BioRxiv. Error: {e.Message}");
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write($"Cannot get RSS feed from Biorxiv. Error: {e.Message}");
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}