using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<dynamic>> GetAllDocumentsAsync();
    }
}
