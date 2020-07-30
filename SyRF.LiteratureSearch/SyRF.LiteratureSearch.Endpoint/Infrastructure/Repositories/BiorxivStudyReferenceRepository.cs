using System;
using System.Threading.Tasks;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.Mongo.Common;

namespace SyRF.LiteratureSearch.Endpoint.Infrastructure.Repositories
{
    public class BiorxivStudyReferenceRepository : MongoRepositoryBase<BiorxivStudyReference, Guid>,
        IBiorxivStudyReferenceRepository
    {
        public BiorxivStudyReferenceRepository(MongoContext context,
            RepositoryCache<Guid, BiorxivStudyReference> repositoryCache) : base(context, repositoryCache)
        {
        }

        public override Task InitialiseIndexesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsReferenceWith(Guid projectId, string doi)
        {
            return AnyAsync(bsr => bsr.ProjectId == projectId && bsr.Doi == doi);
        }
    }
}