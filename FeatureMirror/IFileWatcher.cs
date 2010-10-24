using System.IO;

namespace FeatureMirror
{
    public interface IFileWatcher
    {
        void Start(string pathToWatch);
        void Stop();
        event FileSystemEventHandler Deleted;
        event FileSystemEventHandler Created;
        event FileSystemEventHandler Changed;
    }

    public class FileWatcher : FileSystemWatcher, IFileWatcher
    {
        public void Start(string pathToWatch)
        {
            Path = pathToWatch;
            IncludeSubdirectories = true;
            EnableRaisingEvents = true;
        }

        public void Stop()
        {
            EnableRaisingEvents = false;
            Dispose();
        }
    }
}