using Hangfire;
using ImageProcessor;
using PngProcessorWebApp.Infrastructure;
using System;
using System.Web;
using System.Web.Http;

namespace PngProcessorWebApp.Controllers
{
    [RoutePrefix("api/images")]
    public class ImageController : ApiController
    {
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

                var jobId = BackgroundJob.Enqueue(() => RunJob(fileId, filePath));
                ImageProcessingStorage.AddFileJobPair(fileId, jobId);
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
            var status = ImageProcessingStorage.GetStatus(fileId);
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
            var jobId = ImageProcessingStorage.GetJobIdByFile(fileId);

            if (jobId == null)
            {
                return NotFound();
            }

            BackgroundJob.Delete(jobId);
            ImageProcessingStorage.RemoveFileJobPairByFileName(fileId);

            return Ok();
        }

        public void RunJob(string fileId, string filePath)
        {
            using (var processor = new PngProcessor())
            {
                processor.ProgressChanged += (status) =>
                {
                    ImageProcessingStorage.UpdateFileStatus(fileId, status);
                };
                ImageProcessingStorage.UpdateFileStatus(fileId, 0);
                processor.Process(filePath);

                // Sadly, but I can't schedule it as a continuation of current job.
                BackgroundJob.Schedule(() => ImageProcessingStorage.RemoveFileJobPairByFileName(fileId), DateTime.Now.AddMinutes(10));
            }
        }
    }
}
