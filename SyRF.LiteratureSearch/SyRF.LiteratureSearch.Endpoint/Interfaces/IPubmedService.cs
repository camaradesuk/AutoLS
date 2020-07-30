using System;
using System.Collections.Generic;
using SyRF.LiteratureSearch.Endpoint.DTOs;

namespace SyRF.LiteratureSearch.Endpoint.Interfaces
{
    public interface IPubmedService
    {
        IAsyncEnumerable<PubmedXmlFileInfoDto> FindNewPubmedStudiesAndSave(Guid livingSearchId, Guid fileId,
            string description, Guid projectId,
            string searchTerm, int batchSize);
    }
}