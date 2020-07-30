using System;
using Automatonymous;

namespace SyRF.LivingSearch.Endpoint
{
    public class LivingSearchState :  SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid ProjectId { get; set; }
        public string? SearchName { get; set; }
        public string? Description { get; set; }
        public string? SearchString { get; set; }
        public int UpdateIntervalInDays { get; set; } // In days
        public string? SearchEngineName { get; set; }
        public string? ScheduleId { get; set; }
        public string? ScheduleGroup { get; set; }

    }
}