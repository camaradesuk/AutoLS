using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using SyRF.LivingSearch.Endpoint;
using SyRF.Web.Messages.Events;
using SyRF.WebHostConfig.Common.Model;
using Xunit;

namespace SyRF.LivingSearch.Tests
{
    public class LivingSearchStateMachineTest
    {
        [Fact]
        public async Task LivingSearchStateMachine_Should_Be_Tested()
        {
            var config = new MessageBusConfig
            {
                PubmedParserQueueName = "pubmedParser",
                PubmedSearchQueueName = "pubmedSearch",
                RabbitMqConfig = new RabbitMqConfig
                {
                    SchemeName = "rabbitmq",
                    Hostname = "localhost",
                    Username = "guest",
                    Password = "guest"
                }
            };
            var machine = new LivingSearchStateMachine(config);

            var harness = new InMemoryTestHarness();
            var sagaHarness = harness.StateMachineSaga<LivingSearchState, LivingSearchStateMachine>(machine);
            var saga = harness.Saga<LivingSearchState>();
            
            await harness.Start();
            try
            {
                Guid sagaId = NewId.NextGuid();

                await harness.Bus.Publish<ILivingSearchEnabledOnProjectEvent>(new
                {
                    ProjectId = default(Guid),
                    LivingSearchId = sagaId,
                    SearchString = default(string),
                    UpdateInterval = default(int),
                    BatchSize = default(int),
                    SearchEngineName = default(string),
                    SystematicSearchPrefix = default(string)
                });

                // did the endpoint consume the message
                Assert.NotNull(harness.Sent);

                Assert.True(harness.Consumed.Select<ILivingSearchEnabledOnProjectEvent>().Any());

                // did the actual consumer consume the message
                Assert.True(sagaHarness.Consumed.Select<ILivingSearchEnabledOnProjectEvent>().Any());

                var instance = sagaHarness.Created.ContainsInState(sagaId, machine, machine.Initial);
                Assert.NotNull(instance);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}