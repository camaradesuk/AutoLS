using System;

namespace SyRF.BiorxivParser.Messages
{
    public interface IParseBiorxivFileCommand
    {
        Guid SearchId { get;}
        Guid ProjectId { get; }
        Guid FileId { get; }
        string FileUri { get; }
        int FileNumber { get; }
        int TotalNumberOfFiles { get; }
    }
}