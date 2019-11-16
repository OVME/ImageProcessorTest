using System.Threading;

namespace PngProcessorWebApp.Services
{
    public interface IImageProcessingService
    {
        void RunProcessing(string fileId, string filePath);
        void RegisterNewThread(string fileId, Thread thread);
        double? GetStatus(string fileId);
        Thread GetThread(string fileId);
        void RemoveThread(string fileId);
    }
}
