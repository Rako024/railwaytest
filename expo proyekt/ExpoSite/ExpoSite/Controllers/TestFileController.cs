using Expo.Business.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestFileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public TestFileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Fayl yükləmə üçün vacibdir
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = await _fileService.UploadFile(file,"pictures");
            return Ok(new { FileName = fileName });
        }

        [HttpPost("upload-multiple")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            var fileNames = await _fileService.UploadFiles(files, "pictures");
            return Ok(fileNames);
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile([FromRoute] string fileName, [FromQuery] string endFolderPath)
        {
            var file = await _fileService.GetFileById(fileName, endFolderPath);
            return File(file.OpenReadStream(), "application/octet-stream", file.FileName);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFiles([FromQuery] List<string> fileNames, [FromQuery] string endFolderPath)
        {
            var files = await _fileService.GetAllFiles(fileNames, endFolderPath);
            return Ok(files);
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string fileName)
        {
            // GetFileById kullanılarak dosya bilgisi alınıyor
            var file = await _fileService.GetFileById(fileName, "pictures");

            // Eğer dosya bulunamazsa
            if (file == null)
                return NotFound("Dosya bulunamadı.");

            // Dosya `File` yanıtı olarak döndürülüyor
            return File(file.OpenReadStream(), "application/octet-stream", file.FileName);
        }


    }
}
