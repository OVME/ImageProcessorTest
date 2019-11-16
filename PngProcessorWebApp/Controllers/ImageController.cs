using PngProcessorWebApp.Services;
using System;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace PngProcessorWebApp.Controllers
{
    [RoutePrefix("api/images")]
    public class ImageController : ApiController
    {
        private readonly IImageProcessingService _imageProcessingService;

        public ImageController(IImageProcessingService imageProcessingService)
        {
            _imageProcessingService = imageProcessingService;
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult UploadImage()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var file = httpRequest.Files[0];
                var fileId = Guid.NewGuid().ToString();
                var filePath = HttpContext.Current.Server.MapPath("~/" + fileId);
                file.SaveAs(filePath);

                var workerThread = new Thread(() => _imageProcessingService.RunProcessing(fileId, filePath));
                workerThread.Start();
                _imageProcessingService.RegisterNewThread(fileId, workerThread);
                return Ok(fileId);
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("{fileId}/status")]
        [HttpGet]
        public IHttpActionResult GetImageProcessingStatus(string fileId)
        {
            var status = _imageProcessingService.GetStatus(fileId);
            if (status == null)
            {
                return NotFound();
            }

            return Ok(string.Format("{0:P0}", status));
        }

        [Route("{fileId}")]
        [HttpDelete]
        public IHttpActionResult Delete(string fileId)
        {
            var thread = _imageProcessingService.GetThread(fileId);

            if (thread == null)
            {
                return NotFound();
            }

            thread.Abort();
            _imageProcessingService.RemoveThread(fileId);
            // I know that I also must remove status here. And return 404 for /status request. I don't do this in order to show that processing is actually stopped.
            return Ok();
        }
    }
}
