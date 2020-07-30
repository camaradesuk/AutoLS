using System;
using System.Collections.Generic;
using SyRF.SharedKernel.ValueObjects;

namespace SyRF.LivingSearch.Messages.Events
{
    public interface IUniqueLivingSearchStudyIdentifiedEvent
    {
        Guid ProjectId { get; }
        Guid LivingSearchId { get; }
        Guid StudyId { get; }
        Guid FileId { get; }
        string PubmedId { get; }
        string SearchName { get; }
        string Description { get; }
        string Title { get; }
        ICollection<Author> Authors { get; }
        string AuthorAddress { get; }
        PublicationName Publication { get; }
        int Year { get; }
        string Url { get; }
        string Doi { get; }
        string ReferenceType { get; }
        IEnumerable<string> Keywords { get; }
        string Abstract { get; }
    }
}