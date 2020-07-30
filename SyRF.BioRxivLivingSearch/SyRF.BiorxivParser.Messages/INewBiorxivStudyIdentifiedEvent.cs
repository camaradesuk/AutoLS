using System;
using System.Collections.Generic;
using SyRF.SharedKernel.ValueObjects;

namespace SyRF.BiorxivParser.Messages
{
    public interface INewBiorxivStudyIdentifiedEvent
    {
        Guid ProjectId { get; }
        Guid BiorxivSearchId { get; }
        Guid StudyId { get; }
        string SearchName { get; }
        string Description { get; }
        string Title { get; }
        ICollection<Author> Authors { get; }
        string AuthorAddress { get; }
        PublicationName Publication { get; }
        DateTime Year { get; }
        string Url { get; }
        string Doi { get; }
        string ReferenceType { get; }
        IList<string> Keywords { get; }
        string Abstract { get; }
        string PdfRelativePath { get; }
    }
}