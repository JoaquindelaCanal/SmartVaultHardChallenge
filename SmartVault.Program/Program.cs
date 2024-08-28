using System.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmartVault.Program.Services;

namespace SmartVault.Program
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            // Get configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json").Build();

            // Validate the accountId argument
            if (!int.TryParse(args[0], out var accountId))
            {
                Console.WriteLine("Invalid accountId. Please provide a valid integer.");
                return;
            }

            WriteEveryThirdFileToFile(accountId.ToString());
            GetAllFileSizes();
        }
        private static void GetAllFileSizes()
        {
            //Calculate file length
            var _fileService = new FileService();
            var result = _fileService.GetFileTotalSize();


        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            // TODO: Implement functionality
        }
    }
}