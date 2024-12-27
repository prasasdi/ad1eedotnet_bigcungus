using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainAplikasi.Presentation.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        /// <summary>
        /// Unggah file 
        /// </summary>
        /// <remarks>
        /// terlepas dari ukuran sebuah file yang diupload berukuran besar atau kecil, yang ada akan di potong/cacah perchunk
        /// </remarks>
        /// <response code="200">Product created</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int chunkIndex = 0)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File tidak ditemukan");
            string destinationPath = "uploaded";
            if (string.IsNullOrEmpty(destinationPath))
                return BadRequest("Path tujuan tidak ditemukan");

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            var filePath = Path.Combine(destinationPath, file.FileName);
            var bufferSize = 1 * 1024 * 1024; // 1 MB
            var buffer = new byte[bufferSize];

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Append))
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        fileStream.Seek(chunkIndex * bufferSize, SeekOrigin.Begin);
                        int bytesRead;
                        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                        {
                            await stream.WriteAsync(buffer, 0, bytesRead);
                            chunkIndex++;
                            Console.WriteLine($"Chunk {chunkIndex} processed.");
                        }
                    }
                }
                return Ok(new { filePath, chunkIndex });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
