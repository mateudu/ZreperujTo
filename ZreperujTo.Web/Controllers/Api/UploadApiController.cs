using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Models.FileInfoModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadApiController : Controller
    {
        private readonly ZreperujToDbClient _zreperujDb;
        private const int MaxPixels = 1800;
        private const int ThumbnailMaxPixels = 300;
        private const int CompressionRatio = 75;

        public UploadApiController(ZreperujToDbClient zreperujDb)
        {
            _zreperujDb = zreperujDb;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(PictureUploadReadModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            if (file != null)
            {
                Image image = new Image(file.OpenReadStream());
                BlobUploadResult originalResult, thumbnailResult;
                string fileName = Guid.NewGuid().ToString();

                using (MemoryStream org_ms = new MemoryStream())
                {
                    using (MemoryStream thumb_ms = new MemoryStream())
                    {
                        // Original file compression and upload
                        double ratio = Math.Min(((double)image.Width) / (double)MaxPixels, ((double)image.Height) / (double)MaxPixels);
                        if (ratio < 1.0)
                        {
                            image.SaveAsJpeg(org_ms, CompressionRatio);
                        }
                        else
                        {
                            int height = (int)(((double)image.Height) / ratio);
                            int width = (int)(((double)image.Width) / ratio);
                            image.Resize(width, height).SaveAsJpeg(org_ms, CompressionRatio);
                        }
                        var _originalResultTask = _zreperujDb.UploadToBlobAsync(org_ms.ToArray(), fileName + ".jpeg");

                        // Thumbnail file compression and upload
                        ratio = Math.Min(((double)image.Width) / (double)ThumbnailMaxPixels, ((double)image.Height) / (double)ThumbnailMaxPixels);
                        if (ratio < 1.0)
                        {
                            image.SaveAsJpeg(thumb_ms, CompressionRatio);
                        }
                        else
                        {
                            int height = (int)(((double)image.Height) / ratio);
                            int width = (int)(((double)image.Width) / ratio);
                            image.Resize(width, height).SaveAsJpeg(thumb_ms, CompressionRatio);
                        }
                        var _thumbnailResultTask = _zreperujDb.UploadToBlobAsync(thumb_ms.ToArray(), fileName + "_thumbnail.jpeg");
                        await Task.WhenAll(_originalResultTask, _thumbnailResultTask);
                        originalResult = _originalResultTask.Result;
                        thumbnailResult = _thumbnailResultTask.Result;
                    }
                }
                var dbModel = new PictureInfoDbModel
                {
                    BaseName = fileName,
                    OriginalSizeUri = originalResult?.Uri.ToString(),
                    ThumbnailUri = thumbnailResult?.Uri.ToString()
                };

                await _zreperujDb.InsertPictureInfoDbModelAsync(dbModel);

                PictureUploadReadModel result = new PictureUploadReadModel
                {
                    Id = fileName
                };

                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    error_msg = "File is null! Send request at /api/Upload and pass file using POST request body.",
                    tip = @"Check this website: http://www.janaks.com.np/file-upload-asp-net-core-web-api/ and pass it as a 'file' key-value pair using form-data"
                });
            }

        }
    }
}
