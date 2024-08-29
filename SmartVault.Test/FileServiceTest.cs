using NUnit.Framework;
using Moq;
using SmartVault.Program.Services;
using SmartVault.Domain.Interfaces;
using SmartVault.Domain.DTO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVault.Tests.Services
{
    [TestFixture]
    public class FileServiceTests
    {
        private Mock<IFileRepository> _mockFileRepository;
        private FileService _fileService;

        [SetUp]
        public void SetUp()
        {
            _mockFileRepository = new Mock<IFileRepository>();
            _fileService = new FileService(_mockFileRepository.Object);
        }

        [Test]
        public async Task GetFileTotalSize_ShouldReturnCorrectTotalSize()
        {
            // Arrange
            var documents = new List<DocumentDTO>
            {
                new DocumentDTO { FilePath = "path1.txt", Length = 100 },
                new DocumentDTO { FilePath = "path2.txt", Length = 200 },
                new DocumentDTO { FilePath = "missing.txt", Length = 50 } // Missing file
            };

            _mockFileRepository.Setup(repo => repo.GetAllDocumentsAsync())
                               .ReturnsAsync(documents);

            _mockFileRepository.Setup(repo => repo.GetTotalFileSizeAsync())
                               .ReturnsAsync(300);

            // Mock file system
            MockFileSystem(new Dictionary<string, long>
            {
                { "path1.txt", 100 },
                { "path2.txt", 200 }
            });

            // Act
            var result = await _fileService.GetFileTotalSize();

            // Assert
            Assert.AreEqual(300, result);
            _mockFileRepository.Verify(repo => repo.GetTotalFileSizeAsync(), Times.Once);
        }

        [Test]
        public void WriteEveryThirdFileToFile_ShouldWriteCorrectFiles()
        {
            // Arrange
            var documents = new List<DocumentDTO>
            {
                new DocumentDTO { Id = 1, FilePath = "file1.txt", AccountId = 1 },
                new DocumentDTO { Id = 2, FilePath = "file2.txt", AccountId = 1 },
                new DocumentDTO { Id = 3, FilePath = "file3.txt", AccountId = 1 }, // This should be included
                new DocumentDTO { Id = 4, FilePath = "file4.txt", AccountId = 1 },
                new DocumentDTO { Id = 5, FilePath = "file5.txt", AccountId = 1 },
                new DocumentDTO { Id = 6, FilePath = "file6.txt", AccountId = 1 }  // This should be included
            };

            _mockFileRepository.Setup(repo => repo.GetAllDocumentsByAccountAsync(1))
                               .ReturnsAsync(documents);

            // Mock file system
            MockFileSystem(new Dictionary<string, long>
            {
                { "file3.txt", 100 },
                { "file6.txt", 200 }
            });

            // Act
            _fileService.WriteEveryThirdFileToFile(1);

            // Assert
            var outputPath = "reportByAccountIDResult.txt";
            Assert.IsTrue(File.Exists(outputPath));
            var outputContent = File.ReadAllText(outputPath);
            Assert.IsTrue(outputContent.Contains("--- Start of file3.txt ---"));
            Assert.IsTrue(outputContent.Contains("--- Start of file6.txt ---"));
        }

        private void MockFileSystem(Dictionary<string, long> files)
        {
            foreach (var file in files)
            {
                File.WriteAllText(file.Key, new string('a', (int)file.Value)); // Create mock file with specific size
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up mock files
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt"))
            {
                File.Delete(file);
            }
        }
    }
}
