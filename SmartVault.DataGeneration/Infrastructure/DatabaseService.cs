using Dapper;

using Microsoft.Extensions.Configuration;

using SmartVault.Library;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration.Infrastructure
{
    public class DatabaseService : IDisposable
    {
        private readonly SQLiteConnection _connection;

        public DatabaseService(string connectionString)
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

        public void InsertUser(int id, string firstName, string lastName, DateTime dateOfBirth, int accountId, string username, string password)
        {
            var sql = "INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES (@Id, @FirstName, @LastName, @DateOfBirth, @AccountId, @Username, @Password)";
            _connection.Execute(sql, new { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, AccountId = accountId, Username = username, Password = password });
        }

        public void InsertAccount(int id, string name)
        {
            var sql = "INSERT INTO Account (Id, Name) VALUES (@Id, @Name)";
            _connection.Execute(sql, new { Id = id, Name = name });
        }

        public void InsertDocument(int id, string name, string filePath, long length, int accountId)
        {
            var sql = "INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES (@Id, @Name, @FilePath, @Length, @AccountId)";
            _connection.Execute(sql, new { Id = id, Name = name, FilePath = filePath, Length = length, AccountId = accountId });
        }

        public int GetCount(string tableName)
        {
            return _connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName};");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
