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

            string databaseFileName = configuration["DatabaseFileName"];
            string connectionString = string.Format(configuration["ConnectionStrings:DefaultConnection"], databaseFileName);

            var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDoc.txt");
            File.WriteAllText(documentPath, GenerateTestDocument());

            using (var dbService = new DatabaseService(connectionString))
            {
                dbService.InitializeDatabase(databaseFileName);

                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                for (int i = 0; i <= 2; i++)
                {
                    dbService.ExecuteScriptFromFile(files[i]);
                }

                var documentNumber = 0;
                for (int i = 0; i < 100; i++)
                {
                    var randomDay = RandomDay();
                    dbService.InsertUser(i, $"FName{i}", $"LName{i}", randomDay, i, $"UserName-{i}", "e10adc3949ba59abbe56e057f20f883e");
                    dbService.InsertAccount(i, $"Account{i}");

                    for (int d = 0; d < 10000; d++, documentNumber++)
                    {
                        dbService.InsertDocument(documentNumber, $"Document{i}-{d}.txt", documentPath, new FileInfo(documentPath).Length, i);
                    }
                }

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
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}