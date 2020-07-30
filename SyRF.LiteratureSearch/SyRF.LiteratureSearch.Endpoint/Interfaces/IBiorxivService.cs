using System;
using System.Collections.Generic;
using SyRF.LiteratureSearch.Endpoint.DTOs;

namespace SyRF.LiteratureSearch.Endpoint.Interfaces
{
    public interface IBiorxivService
    {
        IAsyncEnumerable<BiorxivHtmlFileInfoDto> FindNewBiorxivStudiesAndSave(string rssFeedUrl, Guid livingSearchId,
            Guid fileId, Guid projectId, string description, int batchSize);
    }
}