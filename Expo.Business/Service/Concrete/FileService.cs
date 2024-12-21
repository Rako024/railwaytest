using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFile(IFormFile file, string endFolderPath)
        {
            if (file == null || file.Length == 0)
                throw new GlobalAppException("Invalid file");

            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files", endFolderPath);

            // Yolu yoxlayır və lazım gəlsə direktoriyanı yaradır.
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileGuid = Guid.NewGuid();
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{fileGuid}{fileExtension}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // Faylın unikal adı qaytarılır.
        }

        public async Task<List<string>> UploadFiles(List<IFormFile> files, string endFolderPath)
        {
            if (files == null || !files.Any())
                throw new GlobalAppException("Files list is empty");

            var fileNames = new List<string>();

            foreach (var file in files)
            {
                var fileName = await UploadFile(file, endFolderPath);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        public async Task<IFormFile> GetFileById(string productId, string endFolderPath)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files", endFolderPath);
            var filePath = Path.Combine(uploadPath, productId);

            if (!File.Exists(filePath))
                throw new GlobalAppException("File not found");

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            return new FormFile(memoryStream, 0, memoryStream.Length, "file", productId);
        }

        public async Task<List<IFormFile>> GetAllFiles(List<string> filenames, string endFolderPath)
        {
            var files = new List<IFormFile>();

            foreach (var fileName in filenames)
            {
                var file = await GetFileById(fileName, endFolderPath);
                files.Add(file);
            }

            return files;
        }

        public async Task DeleteFile(string endFolderPath, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                throw new GlobalAppException("Image name cannot be null or empty");

            // Faylın tam yolu
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", endFolderPath, imageName);

            // Fayl mövcuddursa, sil
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
            else
            {
                throw new GlobalAppException($"File with name '{imageName}' not found in path '{filePath}'");
            }
        }
    }
}
