using System;
using System.Threading.Tasks;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.Mongo.Common;
using SyRF.SharedKernel;

namespace SyRF.LiteratureSearch.Endpoint.Infrastructure
{
    public class MongoLsUnitOfWork : MongoUnitOfWorkBase, ILsUnitOfWork
    {
        public MongoLsUnitOfWork(MongoContext mongoContext, IEventManager eventManager, IPubmedStudyReferenceRepository pubmedStudyReferenceRepository, IBiorxivStudyReferenceRepository biorxivStudyReferenceRepository) : base(mongoContext, eventManager)
        {
            PubmedStudyReferenceRepository = pubmedStudyReferenceRepository;
            BiorxivStudyReferenceRepository = biorxivStudyReferenceRepository;
        }

        public override async Task InitialiseIndexes()
        {
            await PubmedStudyReferenceRepository.InitialiseIndexesAsync();
        }

        public override void CreateMappings()
        {
            throw new NotImplementedException();
        }

        public IPubmedStudyReferenceRepository PubmedStudyReferenceRepository { get; }
        public IBiorxivStudyReferenceRepository BiorxivStudyReferenceRepository { get; }
    }
}