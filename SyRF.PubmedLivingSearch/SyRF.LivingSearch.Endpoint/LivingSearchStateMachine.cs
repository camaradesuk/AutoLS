using System;
using Automatonymous;
using MassTransit;
using SyRF.LiteratureSearch.Messages.Events;
using SyRF.LivingSearch.Endpoint.Activities;
using SyRF.LivingSearch.Messages.Events;
using SyRF.StudyFileParser.Messages.Commands;
using SyRF.StudyFileParser.Messages.Events;
using SyRF.Web.Messages.Events;

namespace SyRF.LivingSearch.Endpoint
{
    public class LivingSearchStateMachine : MassTransitStateMachine<LivingSearchState>
    {
        public State Enabled { get; set; }
        public State Disabled { get; set; }

        public Event<ILivingSearchEnabledOnProjectEvent> LivingSearchEnabled { get; set; }
        public Event<ILivingSearchDisabledOnProjectEvent> LivingSearchDisabled { get; set; }

        public Event<IPubmedXmlFileCreatedEvent> PubmedXmlFileCreated { get; set; }
        public Event<ILivingSearchPubmedStudyParsedEvent> PubmedStudyParsed { get; set; }

        public LivingSearchStateMachine(MessageBusConfig messageBusConfig)
        {
            InstanceState(state => state.CurrentState, Enabled, Disabled);

            Event(() => LivingSearchEnabled,
                configurator => configurator.CorrelateById(context => context.Message.LivingSearchId));
            Event(() => LivingSearchDisabled,
                configurator => configurator.CorrelateById(context => context.Message.LivingSearchId));
            Event(() => PubmedXmlFileCreated,
                configurator => configurator.CorrelateById(context => context.Message.LivingSearchId));
            Event(() => PubmedStudyParsed,
                configurator => configurator.CorrelateById(context => context.Message.LivingSearchId));

            Initially(
                When(LivingSearchEnabled)
                    .Then(context => Console.Out.WriteLineAsync("LivingSearch Enabled!"))
                    .Activity(selector => selector.OfType<ExecutePubmedSearchCommandActivity>())
                    .Then(behaviorContext =>
                    {
                        behaviorContext.Instance.ProjectId = behaviorContext.Data.ProjectId;
                        behaviorContext.Instance.SearchString = behaviorContext.Data.SearchString;
                        behaviorContext.Instance.SearchEngineName = behaviorContext.Data.SearchEngineName;
                        behaviorContext.Instance.UpdateIntervalInDays = behaviorContext.Data.UpdateInterval;
                    })
                    .TransitionTo(Enabled)
            );

            During(Enabled,
                When(PubmedXmlFileCreated)
                    .ThenAsync(context => Console.Out.WriteLineAsync("PubmedXmlFileCreatedEvent received!"))
                    .Then(behaviorContext =>
                    {
                        behaviorContext.Instance.SearchName = behaviorContext.Data.SearchName;
                        behaviorContext.Instance.Description = behaviorContext.Data.Description;
                    })
                    .SendAsync(messageBusConfig.PubmedParserQueueUri, context =>
                        context.Init<IParsePubmedXmlFileCommand>(new
                        {
                            context.Data.LivingSearchId,
                            context.Data.ProjectId,
                            context.Data.FileUri,
                            context.Data.FileId,
                            context.Data.SearchName,
                            context.Data.Description,
                            context.Data.FileNumber,
                            context.Data.TotalNumberOfFiles,
                            context.Data.NumberOfReferences,
                            context.Data.TotalNumberOfReferences,
                            context.Data.WebEnv,
                            context.Data.QueryKey
                        }))
                    .ThenAsync(context => Console.Out.WriteLineAsync("ParsePubmedXmlFileCommand Sent!"))
            );

            During(Enabled,
                When(PubmedStudyParsed)
                    .PublishAsync(context => context.Init<IUniqueLivingSearchStudyIdentifiedEvent>(new
                    {
                        context.Data.ProjectId,
                        context.Data.LivingSearchId,
                        context.Data.StudyId,
                        context.Data.FileId,
                        context.Data.PubmedId,
                        context.Data.Title,
                        context.Data.Authors,
                        context.Data.Publication,
                        context.Data.Year,
                        context.Data.Url,
                        context.Data.AuthorAddress,
                        context.Data.Doi,
                        context.Data.ReferenceType,
                        context.Data.Keywords,
                        context.Data.Abstract,
                        context.Instance.SearchName,
                        context.Instance.Description
                    }))
                    .ThenAsync(context => Console.Out.WriteAsync("Unique Living Search Study identified."))
            );


            During(Enabled,
                When(LivingSearchDisabled)
                    .Then(context => Console.Out.WriteLineAsync("Living Search Disabled"))
                    .Then(behaviorContext =>
                    {
                        if (!(behaviorContext.Instance.ProjectId == behaviorContext.Data.ProjectId))
                        {
                            throw new Exception("Living Search cannot be disabled because projects are different");
                        }
                    })
                    .Respond(context => context.CancelScheduledRecurringSend(context.Instance.ScheduleId,
                        context.Instance.ScheduleGroup))
                    .TransitionTo(Disabled)
            );

            During(Disabled,
                When(LivingSearchEnabled)
                    .Then(behaviorContext =>
                    {
                        if (!(behaviorContext.Instance.ProjectId == behaviorContext.Data.ProjectId ||
                              behaviorContext.Instance.SearchString == behaviorContext.Data.SearchString ||
                              behaviorContext.Instance.SearchEngineName == behaviorContext.Data.SearchEngineName))
                        {
                            throw new Exception(
                                "Living Search cannot be enabled in a different project and/or with different search string");
                        }
                    })
                    .Activity(selector => selector.OfType<ExecutePubmedSearchCommandActivity>())
                    .TransitionTo(Enabled)
            );
        }
    }
}