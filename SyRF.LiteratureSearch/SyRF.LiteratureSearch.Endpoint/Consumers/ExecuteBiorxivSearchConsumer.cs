using System;
using System.Threading.Tasks;
using MassTransit;
using SyRF.BiorxivParser.Messages;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Messages.Events;

namespace SyRF.LiteratureSearch.Endpoint.Consumers
{
    public class ExecuteBiorxivSearchConsumer : IConsumer<IExecuteBiorxivCovidFeedSearchCommand>
    {
        public ExecuteBiorxivSearchConsumer(IBiorxivService biorxivService)
        {
            _biorxivService = biorxivService;
        }

        private readonly IBiorxivService _biorxivService;

        public async Task Consume(ConsumeContext<IExecuteBiorxivCovidFeedSearchCommand> context)
        {
            var fileId = Guid.NewGuid();
            const string rssFeedUrl = "https://connect.biorxiv.org/relate/feed/181";
            const string description = "Product of living literature BioRxiv search";
            var projectId = context.Message.ProjectId;
            var searchId = context.Message.BiorxivSearchId;

            var fileInfoList = _biorxivService.FindNewBiorxivStudiesAndSave(rssFeedUrl, searchId, fileId, projectId,
                description, context.Message.BatchSize);

            await foreach (var fileInfo in fileInfoList)
            {
                await context.Publish<IBiorxivFileCreatedEvent>(new
                {
                    SearchId = searchId,
                    FileId = fileId,
                    fileInfo.FileUri,
                    ProjectId = projectId,
                    fileInfo.FileNumber,
                    SearchName = "Living Literature - Biorxiv Search",
                    Description = description,
                    fileInfo.NumberOfReferences,
                    TotalNumberOfFiles = default(int)
                });
            }

        }
    }
        
}