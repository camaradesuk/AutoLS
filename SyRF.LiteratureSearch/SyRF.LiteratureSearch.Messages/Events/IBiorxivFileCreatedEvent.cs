using System;

namespace SyRF.LiteratureSearch.Messages.Events
{
    public interface IBiorxivFileCreatedEvent
    {
        Guid SearchId { get; }
        Guid ProjectId { get; }
        string FileUri { get; }
        Guid FileId { get; }
        string SearchName { get; }
        string Description { get; }
        int FileNumber { get; }
        int NumberOfReferences { get; }
        int TotalNumberOfFiles { get; }
    }
}