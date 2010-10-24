namespace FeatureMirror
{
    public interface IFeatureCopier
    {
        void Copy();
    }

    public class FeatureCopier : IFeatureCopier
    {
        private readonly IFileManager _fileManager;
        private readonly string _solutionDir;

        public FeatureCopier(IFileManager fileManager, string solutionDir)
        {
            _fileManager = fileManager;
            _solutionDir = solutionDir;
        }

        public void Copy()
        {
            foreach (var extension in new [] { "js", "css", "aspx", "ascx", "dll", "gif", "png" })
            {
                foreach (var file in _solutionDir.GetAllFilesWithExtension(extension))
                {
                    _fileManager.Copy(file);
                }
            }
        }
    }
}