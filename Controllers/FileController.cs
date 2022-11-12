using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace KinoAPI.Controllers
{
    [Route("file")]
    public class FileController : ControllerBase
    {
        [HttpGet("download/{fileName}")]
        public ActionResult DownloadFile([FromRoute] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{rootPath}/Files/{fileName}";

            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return File(fileContents, contentType, fileName);
            
        }

        [HttpGet("{fileName}")]
        public ActionResult GetFile([FromRoute] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{rootPath}/Files/{fileName}";

            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return new FileContentResult(fileContents, contentType);
        }

        [HttpPost]
        public ActionResult Upload([FromForm]IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var rootPath = Directory.GetCurrentDirectory();
                var fileName = file.FileName;
                var fullPath = $"{rootPath}/Files/{fileName}";
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
