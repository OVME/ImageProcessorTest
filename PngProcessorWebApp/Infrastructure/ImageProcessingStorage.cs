using System.Collections.Generic;
using System.Threading;

namespace PngProcessorWebApp.Infrastructure
{
    public static class ImageProcessingStorage
    {
        // I can do it through db as well, but since there is no requirement about data persistence between launches, I'll make it at easiest way.
        private static Dictionary<string, Thread> _fileThreadPair = new Dictionary<string, Thread>();
        private static Dictionary<string, double> _fileStatusPair = new Dictionary<string, double>();

        public static void AddFileJobPair(string fileName, Thread thread)
        {
            _fileThreadPair.Add(fileName, thread);
        }

        public static void UpdateFileStatus(string fileId, double newStatus)
        {
            var found = _fileStatusPair.TryGetValue(fileId, out var status);
            if (!found)
            {
                _fileStatusPair.Add(fileId, newStatus);
            }
            else
            {
                _fileStatusPair[fileId] = newStatus;
            }
        }

        public static void RemoveFileThreadPairByFileName(string fileId)
        {
            var found = _fileThreadPair.TryGetValue(fileId, out var thread);

            if (found)
            {
                _fileThreadPair.Remove(fileId);
            }
        }

        public static double? GetStatus(string fileId)
        {
            var statusFound = _fileStatusPair.TryGetValue(fileId, out var status);
            if (statusFound)
            {
                return status;
            }

            return null;
        }

        public static Thread GetThreadByFile(string fileId)
        {
            var found = _fileThreadPair.TryGetValue(fileId, out var thread);
            if (found)
            {
                return thread;
            }

            return null;
        }
    }
}