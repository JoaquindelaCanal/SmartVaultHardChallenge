using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartVault.Domain.Interfaces;
using SmartVault.Program.Repositories;
using SmartVault.Domain.DTO;
using SmartVault.Program.BusinessObjects;
using System.Data.Entity;

namespace SmartVault.Program.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<long> GetFileTotalSize()
        {
            long totalSize = 0;
            long outOfSyncSize = 0;

            var files = await _fileRepository.GetAllDocumentsAsync();

            await Task.Run(() =>
            {
                Parallel.ForEach(files, file =>
                {
                    try
                    {
                        var fileInfo = new FileInfo(file.FilePath);
                        if (fileInfo.Exists)
                        {
                            Interlocked.Add(ref totalSize, fileInfo.Length);
                        }
                        else
                        {
                            Interlocked.Add(ref outOfSyncSize, file.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        // LogError($"Error processing file: {file.FilePath}, Exception: {ex.Message}");
                    }
                });
            });

            // Compare totalSize with the total file size from the database
            var dbTotalSize = await _fileRepository.GetTotalFileSizeAsync();
            if (totalSize != dbTotalSize)
            {
                // Log or handle the discrepancy
                Console.WriteLine($"Total file size mismatch: DB size: {dbTotalSize}, actual size: {totalSize}, out-of-sync size: {outOfSyncSize}");
            }

            return totalSize;
        }

        public void WriteEveryThirdFileToFile(int accountId)
        {
            //Retrieve all files for a specific account from the database
            var files = _fileRepository.GetAllDocumentsByAccountAsync(accountId);

            files.Wait();

            var filesResult = files.Result;

            // 2. Select every third file
            var everyThirdFile = filesResult.Where((doc, index) => index % 3 == 0);

            string outputPath = "reportByAccountIDResult.txt";
            string searchText = "Smith Property";

            // 3. Open output file stream for writing
            using (var outputStream = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                foreach (var doc in everyThirdFile)
                {
                    // Check if the file exists
                    if (doc != null && File.Exists(doc.FilePath))
                    {
                        // 4. Read file content in chunks to avoid memory overload
                        using (var fileStream = new FileStream(doc.FilePath, FileMode.Open, FileAccess.Read))
                        using (var reader = new StreamReader(fileStream))
                        {
                            string line;
                            bool containsSearchText = false;
                            StringBuilder fileContent = new StringBuilder();

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains(searchText))
                                {
                                    containsSearchText = true;
                                }
                                fileContent.AppendLine(line);
                            }

                            // 5. If the file contains the search text, write its contents to the output file
                            if (containsSearchText)
                            {
                                outputStream.WriteLine($"--- Start of {doc.Name} ---");
                                outputStream.Write(fileContent.ToString());
                                outputStream.WriteLine($"--- End of {doc.Name} ---");
                                outputStream.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}
