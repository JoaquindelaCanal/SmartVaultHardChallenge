using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

using SmartVault.Library;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml.Serialization;

namespace SmartVault.Infrastructure
{
    public class DataGenerationRepository : IDisposable
    {
        private readonly SqliteConnection _connection;

        public DataGenerationRepository(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

        public void InitializeDatabase(string databaseFileName)
        {
            SQLiteConnection.CreateFile(databaseFileName);
        }

        public void ExecuteScriptFromFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(BusinessObject));
            using var reader = new StreamReader(filePath);
            var businessObject = serializer.Deserialize(reader) as BusinessObject;

            if (businessObject != null)
            {
                _connection.Execute(businessObject.Script);
            }
        }

        public void InsertUsers(IEnumerable<dynamic> users)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                //Parameterized Queries to prevent SQL injection and improve query performance by allowing SQL to cache query plans
                var sql = "INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES (@Id, @FirstName, @LastName, @DateOfBirth, @AccountId, @Username, @Password)";
                try
                {
                    _connection.Execute(sql, users, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void InsertAccounts(IEnumerable<dynamic> accounts)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var sql = "INSERT INTO Account (Id, Name) VALUES (@Id, @Name)";

                try
                {
                    _connection.Execute(sql, accounts, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void InsertDocuments(IEnumerable<dynamic> documents)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var sql = "INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES (@Id, @Name, @FilePath, @Length, @AccountId)";
                try
                {
                    _connection.Execute(sql, documents, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public int GetCount(string tableName)
        {
            return _connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName};");
        }

        public void Dispose() => _connection?.Dispose();
    }
}
