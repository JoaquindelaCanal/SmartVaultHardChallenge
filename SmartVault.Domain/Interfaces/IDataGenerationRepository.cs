using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Domain.Interfaces
{
    public interface IDataGenerationRepository
    {
        void InitializeDatabase(string databaseFileName);

        void ExecuteScriptFromFile(string filePath);

        void InsertUsers(IEnumerable<dynamic> users);

        void InsertAccounts(IEnumerable<dynamic> accounts);

        void InsertDocuments(IEnumerable<dynamic> documents);

        int GetCount(string tableName);

        void Dispose();
    }
}
