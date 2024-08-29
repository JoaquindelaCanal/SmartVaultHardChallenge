using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartVault.Domain.DTO;

namespace SmartVault.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync();
        Task<IEnumerable<DocumentDTO>> GetAllDocumentsByAccountAsync(int accountId);
        Task<long> GetTotalFileSizeAsync();
    }
}
