using Hangfire;

namespace PngProcessorWebApp.Services
{
    public interface IImageProcessingService
    {
        void RunProcessing(string fileId, string filePath, IJobCancellationToken cancellationToken);
        void RegisterNewJob(string fileId, string jobId);
        double? GetStatus(string fileId);
        string GetJobId(string fileId);
        void RemoveJobId(string fileId);
    }
}
