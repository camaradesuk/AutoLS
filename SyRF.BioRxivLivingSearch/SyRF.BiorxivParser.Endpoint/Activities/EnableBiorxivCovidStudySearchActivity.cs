using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using MassTransit;
using SyRF.BiorxivParser.Messages;
using SyRF.Web.Messages.Events;

namespace SyRF.BiorxivParser.Endpoint.Activities
{
    public class EnableBiorxivCovidStudySearchActivity : Activity<BiorxivParserState, IBiorxivCovidFeedEnabledOnProjectEvent>
    {
        readonly ConsumeContext _consumeContext;
        readonly MessageBusConfig _busConfig;

        public EnableBiorxivCovidStudySearchActivity(ConsumeContext consumeContext, MessageBusConfig busConfig)
        {
            _consumeContext = consumeContext;
            _busConfig = busConfig;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("get-biorxiv-covid-feed");
        }

        public void Accept(StateMachineVisitor visitor)
        {
             visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<BiorxivParserState, IBiorxivCovidFeedEnabledOnProjectEvent> behaviourContext, Behavior<BiorxivParserState, IBiorxivCovidFeedEnabledOnProjectEvent> next)
        {
           
            
            await _consumeContext.Send<IExecuteBiorxivCovidFeedSearchCommand>(new
            {
                behaviourContext.Data.ProjectId,
                behaviourContext.Data.BiorxivSearchId,
                behaviourContext.Data.BatchSize,
                FileUri = "https://connect.biorxiv.org/relate/feed/181",
            });
            
            var scheduleRecurring = await _consumeContext.ScheduleRecurringSend<IExecuteBiorxivCovidFeedSearchCommand>(
                _busConfig.LiteratureSearchUri,
                new BiorxivCovidSearchSchedule(behaviourContext.Instance.UpdateIntervalInDays),
                new
                {
                    behaviourContext.Data.ProjectId,
                    behaviourContext.Data.BiorxivSearchId,
                    behaviourContext.Data.BatchSize,
                    FileUri = "https://connect.biorxiv.org/relate/feed/181",
                });

            behaviourContext.Instance.ScheduleId = scheduleRecurring.Schedule.ScheduleId;
            behaviourContext.Instance.ScheduleGroup = scheduleRecurring.Schedule.ScheduleGroup;
            

            await next.Execute(behaviourContext).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<BiorxivParserState, IBiorxivCovidFeedEnabledOnProjectEvent, TException> context, Behavior<BiorxivParserState, IBiorxivCovidFeedEnabledOnProjectEvent> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
