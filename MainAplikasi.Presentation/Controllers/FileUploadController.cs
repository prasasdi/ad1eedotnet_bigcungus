using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainAplikasi.Presentation.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILoggerManager log;
        public FileUploadController(ILoggerManager loggerManager)
        {
            log = loggerManager;
        }
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
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int chunkIndex, [FromForm] int totalChunks)
        {
            Console.WriteLine($"x> {chunkIndex} > {totalChunks}");
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File tidak ditemukan");
                return BadRequest("File tidak ditemukan");
            }

            string destinationPath = "uploaded";
            if (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("Membuat direktori tujuan: " + destinationPath);
                Directory.CreateDirectory(destinationPath);
            }

            var filePath = Path.Combine(destinationPath, file.FileName + ".part" + chunkIndex);

            try
            {
                Console.WriteLine($"Menerima chunk {chunkIndex + 1} dari {totalChunks} untuk file {file.FileName}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Console.WriteLine($"Chunk {chunkIndex + 1} disimpan ke {filePath}");

                // Jika ini adalah chunk terakhir, kita perlu menggabungkan semua chunk menjadi satu file.
                if (chunkIndex == totalChunks - 1)
                {
                    Console.WriteLine($"Menggabungkan semua chunk untuk file {file.FileName}");
                    var finalFilePath = Path.Combine(destinationPath, file.FileName);
                    using (var finalStream = new FileStream(finalFilePath, FileMode.Create))
                    {
                        for (int i = 0; i < totalChunks; i++)
                        {
                            var partFilePath = Path.Combine(destinationPath, file.FileName + ".part" + i);
                            using (var partStream = new FileStream(partFilePath, FileMode.Open))
                            {
                                await partStream.CopyToAsync(finalStream);
                            }
                            Console.WriteLine($"Menghapus chunk {i + 1}: {partFilePath}");
                            System.IO.File.Delete(partFilePath); // Hapus chunk setelah digabungkan
                        }
                    }
                    Console.WriteLine($"File lengkap disimpan ke {finalFilePath}");
                }

                return Ok(new { filePath, chunkIndex });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
