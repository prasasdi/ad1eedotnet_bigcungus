﻿using System;
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
        [HttpPost]
        [Route("upload")]
        [SwaggerOperation(Summary = "Unggah file", Description = "Route ini digunakan untuk mengunggah file.")]
        public IActionResult UploadFile([FromBody] IFormFile file)
        {

        }
    }
}
