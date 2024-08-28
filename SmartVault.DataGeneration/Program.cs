using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using SmartVault.DataGeneration.Infrastructure;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (configuration == null)
            {
                Console.WriteLine("Configuration could not be loaded. Please check your configuration file.");
                return;
            }

            string databaseFileName = configuration["DatabaseFileName"];
            string connectionString = string.Format(configuration["ConnectionStrings:DefaultConnection"], databaseFileName);

            if (string.IsNullOrWhiteSpace(databaseFileName))
            {
                Console.WriteLine("Database file name is missing in the configuration. Please check your configuration file.");
                return;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Connection string is missing in the configuration. Please check your configuration file.");
                return;
            }

            var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDoc.txt");
            File.WriteAllText(documentPath, GenerateTestDocument());

            using (var dbService = new DataGenerationRepository(connectionString))
            {
                dbService.InitializeDatabase(databaseFileName);

                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                for (int i = 0; i <= 2; i++)
                {
                    dbService.ExecuteScriptFromFile(files[i]);
                }

                var users = new List<dynamic>();
                var accounts = new List<dynamic>();
                var documents = new List<dynamic>();
                var documentNumber = 0;

                //Batch Insert Operations

                for (int i = 0; i < 100; i++)
                {
                    var randomDay = RandomDay();
                    users.Add(new { Id = i, FirstName = $"FName{i}", LastName = $"LName{i}", DateOfBirth = randomDay, AccountId = i, Username = $"UserName-{i}", Password = "e10adc3949ba59abbe56e057f20f883e" });
                    accounts.Add(new { Id = i, Name = $"Account{i}" });

                    for (int d = 0; d < 10000; d++, documentNumber++)
                    {
                        documents.Add(new { Id = documentNumber, Name = $"Document{i}-{d}.txt", FilePath = documentPath, Length = new FileInfo(documentPath).Length, AccountId = i });
                    }
                }

                dbService.InsertUsers(users);
                dbService.InsertAccounts(accounts);
                dbService.InsertDocuments(documents);

                Console.WriteLine($"AccountCount: {dbService.GetCount("Account")}");
                Console.WriteLine($"DocumentCount: {dbService.GetCount("Document")}");
                Console.WriteLine($"UserCount: {dbService.GetCount("User")}");
            }
        }

        static string GenerateTestDocument()
        {
            return string.Join(Environment.NewLine, new string[100].Select(_ => "This is my test document"));
        }

        static DateTime RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            var gen = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}