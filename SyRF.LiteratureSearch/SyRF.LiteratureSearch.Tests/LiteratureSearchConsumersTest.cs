using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using Moq;
using SyRF.BiorxivParser.Messages;
using SyRF.LivingSearch.Messages.Commands;
using SyRF.LiteratureSearch.Endpoint.Consumers;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Services;
using SyRF.SharedKernel.Interfaces;
using Xunit;

namespace SyRF.LiteratureSearch.Tests
{
    public class LiteratureSearchConsumersTest
    {
        [Fact]
        public async Task Should_Test_ExecuteBioRxivSearchConsumer()
        {
            //Arrange
            var mockBiorxivService = new Mock<IBiorxivService>();

            var harness = new InMemoryTestHarness();
            var consumerHarness =
                harness.Consumer(() => new ExecuteBiorxivSearchConsumer(mockBiorxivService.Object));

            //Act
            await harness.Start();
            //Assert
            try
            {
                await harness.InputQueueSendEndpoint.Send<IExecuteBiorxivCovidFeedSearchCommand>(new
                {
                    ProjectId = default(Guid),
                    BiorxivSearchId = default(Guid),
                    BatchSize = default(Int32),
                    FileUri = default(string)
                });
                Assert.True(harness.Consumed.Select<IExecuteBiorxivCovidFeedSearchCommand>().Any());
                Assert.True(consumerHarness.Consumed.Select<IExecuteBiorxivCovidFeedSearchCommand>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_Test_ExecutePubmedSearchConsumer()
        {
            //Arrange
            var pubmedWebClient = new Mock<IPubmedWebClient>();
            var mockFileService = new Mock<IFileService>();
            var mockUnitOfWork = new Mock<ILsUnitOfWork>();
            var pubmedService = new PubmedService(pubmedWebClient.Object, mockFileService.Object, mockUnitOfWork.Object);

            var harness = new InMemoryTestHarness();
            var consumerHarness =
                harness.Consumer(() => new ExecutePubmedSearchConsumer(pubmedService));

            //Act
            await harness.Start();
            //Assert
            try
            {
                await harness.InputQueueSendEndpoint.Send<IExecutePubmedSearchCommand>(new
                {
                    ProjectId = default(Guid),
                    LivingSearchId = default(Guid),
                    SearchTerm = default(String),
                    BatchSize = default(Int32)
                });
                Assert.True(harness.Consumed.Select<IExecutePubmedSearchCommand>().Any());
                Assert.True(consumerHarness.Consumed.Select<IExecutePubmedSearchCommand>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}