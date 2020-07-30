using System.Threading.Tasks;
using SyRF.LiteratureSearch.Endpoint.DTOs;

namespace SyRF.LiteratureSearch.Endpoint.Interfaces
{
    public interface IPubmedWebClient
    {
        Task<PubmedResultQueryDto> SubmitSearch(string searchQuery);
        Task<string> GetRecordsXmlString(string? webEnv, string? queryKey, int batchSize, int retStart = 0);
    }
}