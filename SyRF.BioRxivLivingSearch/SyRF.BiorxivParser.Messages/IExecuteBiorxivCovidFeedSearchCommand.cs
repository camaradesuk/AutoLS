using System;

namespace SyRF.BiorxivParser.Messages
{
    public interface IExecuteBiorxivCovidFeedSearchCommand
    {
        Guid ProjectId { get; set; }
        Guid BiorxivSearchId { get; set; }
        int BatchSize { get; set; }
        string FileUri { get; set; }
    }
}