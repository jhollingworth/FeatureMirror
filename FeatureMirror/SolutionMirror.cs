using System;
using System.IO;

namespace FeatureMirror
{
    public class SolutionMirror : IDisposable
    {
        private readonly IFileWatcher _fileWatcher;
        private readonly IFileManager _fileManager;

        public SolutionMirror(string name, string solutionPath, IFileWatcher fileWatcher, IFileManager fileManager)
        {
            Logger.Log("Starting to mirror {0} ({1})", name, solutionPath);

            _fileWatcher = fileWatcher;
            _fileManager = fileManager;
            _fileWatcher.Created += FileChanged;
            _fileWatcher.Changed += FileCreated;
            _fileWatcher.Deleted += FileDeleted;
            _fileWatcher.Start(solutionPath);
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            _fileManager.Copy(e.FullPath);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            _fileManager.Copy(e.FullPath);
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            _fileManager.Delete(e.FullPath);
        }

        public void Dispose()
        {
            Logger.Log("Stopping file watcher and cleaning up");
            
            _fileWatcher.Stop();
            _fileWatcher.Created -= FileChanged;
            _fileWatcher.Changed -= FileChanged;
            _fileWatcher.Deleted -= FileDeleted;
        }
    }
}