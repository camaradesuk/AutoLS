using System;
using SyRF.SharedKernel.BaseClasses;
using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Model
{
    public class PubmedStudyReference : Entity<Guid>, IAggregateRoot<Guid>
    {
        public PubmedStudyReference(Guid id, Guid projectId, Guid livingSearchId, string doi, string pubmedId) :
            base(id)
        {
            ProjectId = projectId;
            LivingSearchId = livingSearchId;
            Doi = doi;
            PubmedId = pubmedId;
        }

        public Guid LivingSearchId { get; }
        public Guid ProjectId { get; }
        public string Doi { get; }
        public string PubmedId { get; }
    }
}