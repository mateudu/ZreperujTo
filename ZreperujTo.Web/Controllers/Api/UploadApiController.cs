using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadApiController : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(FileName), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            if (file != null)
            {
                var stream = file.OpenReadStream();
                var name = file.FileName;

                return Ok(new FileName
                {
                    Name = file.Name,
                    Size = file.Length,
                    FileUrl = @"http://example.com/file_url_address"
                });
            }
            else
            {
                return Ok(new
                {
                    error_msg = "File is null! Send request at /api/Upload and pass file using POST request body.",
                    tip = @"Check this website: http://www.janaks.com.np/file-upload-asp-net-core-web-api/ and pass it as a 'file' key-value pair using form-data"
                });
            }
        }

        public class FileName
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public string FileUrl { get; set; }
        }
    }
}
