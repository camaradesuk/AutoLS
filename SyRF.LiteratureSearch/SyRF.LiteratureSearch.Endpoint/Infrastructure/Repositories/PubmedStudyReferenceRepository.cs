using System;
using System.Threading.Tasks;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.Mongo.Common;

namespace SyRF.LiteratureSearch.Endpoint.Infrastructure.Repositories
{
    public class PubmedStudyReferenceRepository : MongoRepositoryBase<PubmedStudyReference, Guid>, IPubmedStudyReferenceRepository
    {
        public PubmedStudyReferenceRepository(MongoContext context, RepositoryCache<Guid, PubmedStudyReference> repositoryCache) : base(context, repositoryCache)
        {
        }

        public override Task InitialiseIndexesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsReferenceWith(Guid projectId, string pubmedId)
        {
            return AnyAsync(psr => psr.ProjectId == projectId && psr.PubmedId == pubmedId);
        }
    }
}