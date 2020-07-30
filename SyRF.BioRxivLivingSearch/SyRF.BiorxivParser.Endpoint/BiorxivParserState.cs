using System;
using Automatonymous;

namespace SyRF.BiorxivParser.Endpoint
{
    public class BiorxivParserState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid ProjectId { get; set; }
        public int UpdateIntervalInDays { get; set; }
        public string ScheduleId { get; set; }
        public string ScheduleGroup { get; set; }
        public string SearchName { get; set; }
        public string Description { get; set; }
    }
}