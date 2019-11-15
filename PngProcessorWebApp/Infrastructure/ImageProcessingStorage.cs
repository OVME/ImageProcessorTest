using System.Collections.Generic;

namespace PngProcessorWebApp.Infrastructure
{
    public static class ImageProcessingStorage
    {
        // I can do it through db as well, but since there is no requirement about data persistence between launches, I'll make it at easiest way.
        private static Dictionary<string, string> _fileJobIdPair = new Dictionary<string, string>();
        private static Dictionary<string, double> _fileStatusPair = new Dictionary<string, double>();

        public static void AddFileJobPair(string fileName, string jobId)
        {
            _fileJobIdPair.Add(fileName, jobId);
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

        public static void RemoveFileJobPairByFileName(string fileId)
        {
            var found = _fileJobIdPair.TryGetValue(fileId, out var jobId);

            if (found)
            {
                _fileJobIdPair.Remove(fileId);
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

        public static string GetJobIdByFile(string fileId)
        {
            var found = _fileJobIdPair.TryGetValue(fileId, out var jobId);
            if (found)
            {
                return jobId;
            }

            return null;
        }
    }
}