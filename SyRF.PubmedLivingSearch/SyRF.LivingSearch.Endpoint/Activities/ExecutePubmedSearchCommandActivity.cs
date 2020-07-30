using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using MassTransit;
using SyRF.LivingSearch.Messages.Commands;
using SyRF.Web.Messages.Events;

namespace SyRF.LivingSearch.Endpoint.Activities
{
    public class ExecutePubmedSearchCommandActivity : Activity<LivingSearchState, ILivingSearchEnabledOnProjectEvent>
    {
        private readonly ConsumeContext _consumeContext;
        private readonly MessageBusConfig _busConfig;
        
        public ExecutePubmedSearchCommandActivity(ConsumeContext consumeContext, MessageBusConfig busConfig)
        {
            _consumeContext = consumeContext;
            _busConfig = busConfig;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send-execute-pubmed-search");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<LivingSearchState, ILivingSearchEnabledOnProjectEvent> behaviourContext,
            Behavior<LivingSearchState, ILivingSearchEnabledOnProjectEvent> next)
        {
            await _consumeContext.Send<IExecutePubmedSearchCommand>(new
            {
                behaviourContext.Data.LivingSearchId,
                behaviourContext.Data.ProjectId,
                SearchTerm = behaviourContext.Data.SearchString,
                behaviourContext.Data.BatchSize
            });

           var scheduleRecurring = await _consumeContext.ScheduleRecurringSend<IExecutePubmedSearchCommand>(
                _busConfig.LiteratureSearchUri,
                new LivingSearchSchedule(behaviourContext.Instance.UpdateIntervalInDays),
                new
                {
                    behaviourContext.Data.LivingSearchId,
                    behaviourContext.Data.ProjectId,
                    SearchTerm = behaviourContext.Data.SearchString,
                    behaviourContext.Data.BatchSize
                });

           behaviourContext.Instance.ScheduleId = scheduleRecurring.Schedule.ScheduleId;
           behaviourContext.Instance.ScheduleGroup = scheduleRecurring.Schedule.ScheduleGroup;
          
            await Console.Out.WriteLineAsync("ExecutePubmedSearchCommand Sent!");

            await next.Execute(behaviourContext).ConfigureAwait(false);
        }

        public Task Faulted<TException>(
            BehaviorExceptionContext<LivingSearchState, ILivingSearchEnabledOnProjectEvent, TException> context,
            Behavior<LivingSearchState, ILivingSearchEnabledOnProjectEvent> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}