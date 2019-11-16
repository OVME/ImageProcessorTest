using ImageProcessor;
using PngProcessorWebApp.Infrastructure;
using System;
using System.Threading;

namespace PngProcessorWebApp.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        public void RunProcessing(string fileId, string filePath)
        {
            using (var processor = new PngProcessor())
            {
                processor.ProgressChanged += (status) =>
                {
                    ImageProcessingStorage.UpdateFileStatus(fileId, status);
                };
                ImageProcessingStorage.UpdateFileStatus(fileId, 0);
                processor.Process(filePath);
            }
        }

        public void RegisterNewThread(string fileId, Thread thread)
        {
            ImageProcessingStorage.AddFileJobPair(fileId, thread);
        }

        public double? GetStatus(string fileId)
        {
            return ImageProcessingStorage.GetStatus(fileId);
        }

        public Thread GetThread(string fileId)
        {
            return ImageProcessingStorage.GetThreadByFile(fileId);
        }

        public void RemoveThread(string fileId)
        {
            ImageProcessingStorage.RemoveFileThreadPairByFileName(fileId);
        }
    }
}