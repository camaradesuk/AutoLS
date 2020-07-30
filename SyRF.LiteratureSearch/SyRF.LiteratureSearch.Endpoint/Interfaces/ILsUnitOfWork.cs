using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Interfaces
{
    public interface ILsUnitOfWork : IUnitOfWork
    {
        IPubmedStudyReferenceRepository PubmedStudyReferenceRepository { get; }
        IBiorxivStudyReferenceRepository BiorxivStudyReferenceRepository { get; }
    }
}