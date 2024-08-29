using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SmartVault.Domain.DTO;

namespace SmartVault.Domain.Interfaces
{
    public interface IFileService
    {
        Task<long> GetFileTotalSize();
        void WriteEveryThirdFileToFile(int accountId);
    }
}
