using Hangfire;
using PngProcessorWebApp.Services;
using System;
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

                // So this is how it should work ideally. But it appears that PngProcessor.Prosess does not get a CancellationToken as parameter.
                // Do you know what that means? That means that hangfire and any other things letting you easilly manage any jobs are just useless.
                // And also that means I'm going to use just bare threads and Thread.Abort method.
                var jobId = BackgroundJob.Enqueue(() => _imageProcessingService.RunProcessing(fileId, filePath, JobCancellationToken.Null));
                _imageProcessingService.RegisterNewJob(fileId, jobId);
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
            var jobId = _imageProcessingService.GetJobId(fileId);

            if (jobId == null)
            {
                return NotFound();
            }

            BackgroundJob.Delete(jobId);
            _imageProcessingService.RemoveJobId(fileId);

            return Ok();
        }
    }
}
