using System;
using System.Threading.Tasks;
using MassTransit;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Messages.Events;
using SyRF.LivingSearch.Messages.Commands;

namespace SyRF.LiteratureSearch.Endpoint.Consumers
{
    public class ExecutePubmedSearchConsumer : IConsumer<IExecutePubmedSearchCommand>
    {
        public ExecutePubmedSearchConsumer(IPubmedService pubmedService)
        {
            _pubmedService = pubmedService;
        }

        private readonly IPubmedService _pubmedService;

        public async Task Consume(ConsumeContext<IExecutePubmedSearchCommand> context)
        {
            await Console.Out.WriteLineAsync("Execute Pubmed Search Command Received.");

            var fileId = Guid.NewGuid();
            var description = "Product of living literature pubmed search";
            var fileInfoList =
                _pubmedService.FindNewPubmedStudiesAndSave(context.Message.LivingSearchId, fileId, description,
                    context.Message.ProjectId, context.Message.SearchTerm, context.Message.BatchSize);
            await foreach (var fileInfo in fileInfoList)
            {
                await context.Publish<IPubmedXmlFileCreatedEvent>(new
                {
                    context.Message.LivingSearchId,
                    context.Message.ProjectId,
                    fileInfo.FileUri,
                    FileId = fileId,
                    SearchName = "Living Search - Pubmed",
                    Description = description,
                    fileInfo.FileNumber,
                    TotalNumberOfFiles = fileInfo.TotalFileNumber,
                    fileInfo.NumberOfReferences,
                    fileInfo.TotalNumberOfReferences,
                    fileInfo.WebEnv,
                    fileInfo.QueryKey
                });
            }

            await Console.Out.WriteLineAsync("Pubmed Xml File has been created.");
        }
    }
}