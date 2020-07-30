using System;
using SyRF.SharedKernel.BaseClasses;
using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Model
{
    public class BiorxivStudyReference : Entity<Guid>, IAggregateRoot<Guid>
    {
        public BiorxivStudyReference(Guid id, Guid projectId, Guid livingSearchId, string doi, string studyPageUrl) :
            base(id)
        {
            ProjectId = projectId;
            LivingSearchId = livingSearchId;
            Doi = doi;
            StudyPageUrl = studyPageUrl;
        }

        public Guid LivingSearchId { get; }
        public Guid ProjectId { get; }
        public string Doi { get; }
        public string StudyPageUrl { get; }
    }
}