using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;

using SmartVault.Domain.Interfaces;
using System.Threading.Tasks;
using System;
using System.Data.Common;
using System.Linq;
using SmartVault.Domain.DTO;

namespace SmartVault.Program.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly SQLiteConnection _connection;

        public FileRepository(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync()
        {
            return await _connection.QueryAsync<DocumentDTO>("SELECT * FROM Document");
        }

        public async Task<IEnumerable<DocumentDTO>> GetAllDocumentsByAccountAsync(int accountId)
        {
            return await _connection.QueryAsync<DocumentDTO>("SELECT * FROM Document WHERE AccountId = @AccountId ORDER BY Id",
                new { AccountId = accountId });
        }

        public async Task<long> GetTotalFileSizeAsync()
        {
            return await _connection.ExecuteScalarAsync<long>($"SELECT SUM(Length) FROM Document;");
        }


    }
}
