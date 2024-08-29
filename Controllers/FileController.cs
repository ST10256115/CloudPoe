using Microsoft.AspNetCore.Mvc;

namespace CloudPoe.Controllers
{
    using CloudPoe.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using System.Threading.Tasks;

    public class FileController : Controller
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    await _fileService.UploadFileAsync(file.FileName, stream);
                }
            }

            return RedirectToAction("Upload");
        }

        [HttpGet]
        public IActionResult Download()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _fileService.DownloadFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
    }

}
