using System;
using Automatonymous;
using MassTransit;
using SyRF.BiorxivParser.Endpoint.Activities;
using SyRF.BiorxivParser.Messages;
using SyRF.LiteratureSearch.Messages.Events;
using SyRF.StudyFileParser.Messages.Events;
using SyRF.Web.Messages.Events;

namespace SyRF.BiorxivParser.Endpoint
{
    public class BiorxivParserStateMachine : MassTransitStateMachine<BiorxivParserState>
    {
        public State Enabled { get; set; }
        public State Disabled { get; set; }

        public Event<IBiorxivCovidFeedEnabledOnProjectEvent> CovidFeedEnabled { get; set; }
        public Event<IBiorxivCovidFeedDisabledOnProjectEvent> BiorxivCovidFeedDisabled { get; set; }
        public Event<IBiorxivFileCreatedEvent> BiorxivFileCreated { get; set; }
        public Event<IBiorxivCovidStudyParsedEvent> BiorxivCovidStudyParsed { get; set; }


        public BiorxivParserStateMachine(MessageBusConfig messageBusConfig)
        {
            InstanceState(state => state.CurrentState, Enabled, Disabled);

            Initially(
                When(CovidFeedEnabled)
                    .Then(context => Console.Out.WriteLineAsync("BioRxiv Covid Feed Enabled!"))
                    .Activity(selector => selector.OfType<EnableBiorxivCovidStudySearchActivity>())
                    .Then(behaviorContext =>
                    {
                        behaviorContext.Instance.ProjectId = behaviorContext.Data.ProjectId;
                        behaviorContext.Instance.UpdateIntervalInDays = behaviorContext.Data.UpdateInterval;
                    })
                    .TransitionTo(Enabled)
                );

            During(Enabled,
                When(BiorxivFileCreated)
                    .ThenAsync(context => Console.Out.WriteLineAsync("BiorxivFileCreatedEvent received!"))
                    .Then(x =>
                    {
                        x.Instance.SearchName = x.Data.SearchName;
                        x.Instance.Description = x.Data.Description;
                    })
                    .SendAsync(messageBusConfig.BiorxivParserQueueUri, context => context.Init<IParseBiorxivFileCommand>(new
                    {
                        context.Data.SearchId,
                        context.Data.ProjectId,
                        context.Data.FileUri,
                        context.Data.FileId,
                        context.Data.FileNumber,
                        context.Data.TotalNumberOfFiles
                    }))
                    .ThenAsync(context => Console.Out.WriteLineAsync("ParsePubmedXmlFileCommand Sent!"))
            );
            
            During(Enabled,
                When(BiorxivCovidStudyParsed)
                    .ThenAsync(context => Console.Out.WriteLineAsync("BiorxivCovidStudyParsedEvent received!"))
                    .PublishAsync(context => context.Init<INewBiorxivStudyIdentifiedEvent>(new
                    {
                        context.Data.Abstract,
                        AuthorAddress = context.Data.AuthorsAddress,
                        context.Data.Authors,
                        BiorxivSearchId = context.Data.SearchId,
                        context.Data.Doi,
                        context.Data.Keywords,
                        context.Data.Url,
                        context.Data.PdfRelativePath,
                        context.Data.ProjectId,
                        context.Data.Publication,
                        context.Data.ReferenceType,
                        context.Data.StudyId,
                        context.Data.Title,
                        context.Data.Year,
                        context.Instance.SearchName,
                        context.Instance.Description
                    }))
            );
            
            During(Enabled,
                When(BiorxivCovidFeedDisabled)
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
                When(CovidFeedEnabled)
                    .Then(context => Console.Out.WriteLineAsync("BioRxiv Covid Feed Enabled!"))
                    .Activity(selector => selector.OfType<EnableBiorxivCovidStudySearchActivity>())
                    .Then(behaviorContext =>
                    { 
                        behaviorContext.Instance.ProjectId = behaviorContext.Data.ProjectId; 
                        behaviorContext.Instance.UpdateIntervalInDays = behaviorContext.Data.UpdateInterval;
                    })
                    .TransitionTo(Enabled)
                );
        }
    }
}