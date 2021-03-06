﻿using System;
using System.Threading.Tasks;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.SharedKernel.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Interfaces
{
    public interface IBiorxivStudyReferenceRepository : ICrudRepository<BiorxivStudyReference, Guid>
    {
        Task<bool> ContainsReferenceWith(Guid projectId, string pubmedId);
    }
}