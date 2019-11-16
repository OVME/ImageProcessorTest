using Hangfire;
using ImageProcessor;
using PngProcessorWebApp.Infrastructure;
using System;
using System.Threading;

namespace PngProcessorWebApp.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        public void RunProcessing(string fileId, string filePath, IJobCancellationToken cancellationToken)
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

        public void RegisterNewJob(string fileId, string jobId)
        {
            ImageProcessingStorage.AddFileJobPair(fileId, jobId);
        }

        public double? GetStatus(string fileId)
        {
            return ImageProcessingStorage.GetStatus(fileId);
        }

        public string GetJobId(string fileId)
        {
            return ImageProcessingStorage.GetJobIdByFile(fileId);
        }

        public void RemoveJobId(string fileId)
        {
            ImageProcessingStorage.RemoveFileJobPairByFileName(fileId);
        }
    }
}