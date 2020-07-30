
using System;

namespace SyRF.LiteratureSearch.Messages.Events
{
    public interface IPubmedXmlFileCreatedEvent
    {
        Guid LivingSearchId { get; }
        Guid ProjectId { get; }
        string FileUri { get; }
        Guid FileId { get; }
        string SearchName { get; }
        string Description { get; }
        int FileNumber { get; }
        int TotalNumberOfFiles { get; }
        int NumberOfReferences { get; }
        int TotalNumberOfReferences { get; }
        string WebEnv { get; }
        string QueryKey { get; }
    }
    
}