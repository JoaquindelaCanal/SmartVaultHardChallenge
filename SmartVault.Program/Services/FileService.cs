using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartVault.Domain.Interfaces;
using SmartVault.Program.Repositories;

namespace SmartVault.Program.Services
{
    internal class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService()
        {
            _fileRepository = new FileRepository();
        }

        public async Task<long> GetFileTotalSize()
        {
            long totalSize = 0;
            var files = await _fileRepository.GetAllDocumentsAsync();

            // Parallel processing
            Parallel.ForEach(files, file =>
            {
                try
                {
                    var fileInfo = new FileInfo(file.FilePath);
                    if (fileInfo.Exists)
                    {
                        Interlocked.Add(ref totalSize, fileInfo.Length);
                    }
                }
                catch (Exception ex)
                {
                    // LogError($"Error processing file: {file.FilePath}, Exception: {ex.Message}");
                }
            });

            return totalSize;
        }

    }
}
