using System.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmartVault.Program.Services;
using Microsoft.Extensions.DependencyInjection;
using SmartVault.Domain.Interfaces;
using System.Data.SQLite;
using SmartVault.Program.Repositories;
using SmartVault.Domain.DTO;

namespace SmartVault.Program
{
    partial class Program
    {
        private static IFileService _fileService;

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }             

            // Validate the accountId argument
            if (!int.TryParse(args[0], out var accountId))
            {
                Console.WriteLine("Invalid accountId. Please provide a valid integer.");
                return;
            }

            var serviceCollection = new ServiceCollection();

            SetupServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _fileService = serviceProvider.GetService<IFileService>() ?? throw new Exception("IFileService not registered.");

            WriteEveryThirdFileToFile(accountId.ToString());
            GetAllFileSizes();
        }
        private static void GetAllFileSizes()
        {
            //Calculate file length
            var result = _fileService.GetFileTotalSize();

            Console.WriteLine($"Result Get Total Size: {result}");
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            _fileService.WriteEveryThirdFileToFile(int.Parse(accountId));
        }

        private static void SetupServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            //setup scoped services and repositories
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileRepository, FileRepository>().AddSingleton(new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"])));
        }
    }
}