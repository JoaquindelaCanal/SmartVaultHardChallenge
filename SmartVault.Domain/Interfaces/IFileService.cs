using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartVault.Domain.Interfaces
{
    public interface IFileService
    {
        Task<long> GetFileTotalSize();

    }
}
