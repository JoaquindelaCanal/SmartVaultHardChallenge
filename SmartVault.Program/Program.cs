using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            WriteEveryThirdFileToFile(args[0]);
            GetAllFileSizes();
        }
        private static void GetAllFileSizes()
        {
            
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            // TODO: Implement functionality
        }
    }
}