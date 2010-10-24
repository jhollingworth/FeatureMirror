using System;
using System.IO;
using System.Linq;

namespace FeatureMirror
{
    public interface IFileManager
    {
        void Copy(string path);
        void Delete(string path);
    }

    public class FileManager : IFileManager
    {
        private readonly string _solutionDir;
        private readonly IDestinationCalculator _destinationCalculator;

        public FileManager(string solutionDir, IDestinationCalculator destinationCalculator)
        {
            _solutionDir = solutionDir;
            _destinationCalculator = destinationCalculator;
        }

        public void Copy(string path)
        {
            if (false == IsValidFileType(path))
            {
                return;
            }

            if(false == _destinationCalculator.IsValidPath(path))
            {
                return;
            }

            try
            {
                var destination = _destinationCalculator.Calculate(path);

                destination = Path.Combine(destination, GetPath(path));

                var destinationDirectory = Path.GetDirectoryName(destination);

                if (false == Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                Logger.Log("Copying the file from {0} to {1}", path, destination);

                File.Copy(path, destination, true);
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while trying to copy {0}: {1}", path, ex.Message);
            }
        }

        public void Delete(string path)
        {
            if (false == IsValidFileType(path))
            {
                return;
            }
        }

        private string GetPath(string path)
        {
            path = path.Replace(_solutionDir, string.Empty);
            path = path.Substring(path.IndexOf("\\") + 1);
            path = path.Substring(path.IndexOf("\\") + 1);
            path = path.Substring(path.IndexOf("\\") + 1);

            return path;
        }

        private bool IsValidFileType(string path)
        {
            var extension = Path.GetExtension(path);

            return new[] { ".js", ".css", ".aspx", ".dll", ".ascx", ".gif", ".png" }.Any(validExtensions => extension.Equals(validExtensions, StringComparison.InvariantCulture));
        }
    }
}