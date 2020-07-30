using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SyRF.LiteratureSearch.Endpoint.DTOs;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Services
{
    public class PubmedService : IPubmedService
    {
        public PubmedService(IPubmedWebClient pubmedWebClient,
            IFileService fileService, ILsUnitOfWork lsUnitOfWork)
        {
            _pubmedWebClient = pubmedWebClient;
            _fileService = fileService;
            _lsUnitOfWork = lsUnitOfWork;
        }

        private readonly IPubmedWebClient _pubmedWebClient;
        private readonly IFileService _fileService;
        private readonly ILsUnitOfWork _lsUnitOfWork;

        public async IAsyncEnumerable<PubmedXmlFileInfoDto> FindNewPubmedStudiesAndSave(Guid livingSearchId,
            Guid fileId,
            string description, Guid projectId,
            string searchTerm, int batchSize)
        {
            var queryWebResult = await _pubmedWebClient.SubmitSearch(searchTerm);
            var fileNumber = 1;
            var numberOfNewReference = 0;
            var totalFileNumber = queryWebResult.Count / batchSize;
            var nodes = new Collection<XElement>();
            var pubmedStudyReferences = new Collection<PubmedStudyReference>();

            for (var retStart = 0; retStart < queryWebResult.Count; retStart += batchSize)
            {
                var xmlString = await _pubmedWebClient.GetRecordsXmlString(queryWebResult.WebEnv,
                    queryWebResult.QueryKey, batchSize, retStart);
                var xmlReaderSettings = new XmlReaderSettings()
                {
                    Async = true
                };
                using var reader = XmlReader.Create(xmlString, xmlReaderSettings);
                await reader.MoveToContentAsync();
                while (!reader.EOF)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PubmedArticle")
                    {
                        if (!(XNode.ReadFrom(reader) is XElement el)) continue;
                        var pubmedId = el.Descendants("PMID").First().Value;
                        if (await _lsUnitOfWork.PubmedStudyReferenceRepository.ContainsReferenceWith(projectId, pubmedId)) continue;
                        var studyId = Guid.NewGuid();
                        el.Add(new XElement("StudyId", studyId.ToString()));
                        pubmedStudyReferences.Add(new PubmedStudyReference(studyId, projectId,
                            livingSearchId,el.Descendants("ELocationID").First().Value, pubmedId));
                        nodes.Add(el);
                        numberOfNewReference++;
                        if (numberOfNewReference != batchSize) continue;
                        var fileUri = await SaveBatchOfPubmedNodes(nodes, projectId, livingSearchId, fileId,
                            description, fileNumber);
                        nodes.Clear();
                        await _lsUnitOfWork.SaveManyAsync(pubmedStudyReferences);
                        pubmedStudyReferences.Clear();
                        yield return new PubmedXmlFileInfoDto(fileUri, queryWebResult.WebEnv, queryWebResult.Count,
                            fileNumber, numberOfNewReference, totalFileNumber, queryWebResult.QueryKey);
                        numberOfNewReference = 0;
                        fileNumber++;
                    }
                    else
                    {
                        await reader.ReadAsync();
                    }
                }
            }

            if (numberOfNewReference != 0)
            {
                var fileUri = await SaveBatchOfPubmedNodes(nodes, projectId, livingSearchId, fileId,
                    description, fileNumber);
                await _lsUnitOfWork.SaveManyAsync(pubmedStudyReferences);
                yield return new PubmedXmlFileInfoDto(fileUri, queryWebResult.WebEnv, queryWebResult.Count,
                    fileNumber, numberOfNewReference, totalFileNumber, queryWebResult.QueryKey);
            }
        }

        private async Task<string> SaveBatchOfPubmedNodes(IEnumerable<XElement> nodes, Guid projectId,
            Guid livingSearchId, Guid fileId, string description, int fileNumber)
        {
            var relativePath =
                $"{projectId}/Living Searches/{livingSearchId}/Living Search - {livingSearchId} - {fileNumber}.xml";
            var settings = new XmlWriterSettings {Indent = true, Async = true};
            var (fileSaveStream, saveTask) =
                await _fileService.CreateFileSaveStreamAsync(relativePath, fileId, description, "text/xml");
            using var writer = XmlWriter.Create(fileSaveStream as Stream, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "PubmedArticleSet", null);

            foreach (var node in nodes)
            {
                await writer.WriteRawAsync(node.Value);
            }

            await writer.WriteEndDocumentAsync();
            await writer.FlushAsync();
            writer.Close();
            await saveTask;

            return relativePath;
        }
    }
}