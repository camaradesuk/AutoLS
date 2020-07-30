using System;

namespace SyRF.LivingSearch.Messages.Commands
{
    public interface IExecutePubmedSearchCommand
    {
        Guid ProjectId { get; set; }
        Guid LivingSearchId { get; set; }
        string SearchTerm { get; set; }
        int BatchSize { get; set; }
    }
}