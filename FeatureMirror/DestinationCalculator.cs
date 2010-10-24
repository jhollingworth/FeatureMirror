using System;
using System.IO;
using System.Linq;

namespace FeatureMirror
{
    public interface IDestinationCalculator
    {
        bool IsValidPath(string path);
        string Calculate(string path);
    }

    public class DestinationCalculator : IDestinationCalculator
    {
        public bool IsValidPath(string path)
        {
            if (path.Contains("_PublishedWebsites") || path.Contains("output"))
                return false;

            return GetDestination(path) != null;
        }

        public string Calculate(string path)
        {
            var destination = GetDestination(path);
            
            if(destination == null)
                throw new UnknownDestinationException("Could not find the destination for " + path);

            return destination;
        }

        private string GetDestination(string path)
        {
            var dir = new FileInfo(path).Directory;
            var isBin = dir.Name.Equals("bin", StringComparison.InvariantCultureIgnoreCase);
            var configFile = GetFeatureConfigIn(dir);

            if (configFile == null)
            {
                dir = dir.Parent;

                configFile = GetFeatureConfigIn(dir);

                if(configFile == null)
                {
                    dir = dir.Parent;

                    configFile = GetFeatureConfigIn(dir);

                    if (configFile == null)
                    {
                        dir = dir.Parent;

                        configFile = GetFeatureConfigIn(dir);

                        if (configFile == null)
                        {
                            return null;
                        }
 
                    }
                }
            }

            var destination = GetDestination(configFile);

            if (destination == null)
            {
                return null;
            }
            
            return Path.GetFullPath(dir.FullName + "\\" + (isBin ? "..\\" : "") +  destination);
        }

        private FileInfo GetFeatureConfigIn(DirectoryInfo dir)
        {
            return dir.GetFiles("feature.yml").SingleOrDefault();
        }

        private string GetDestination(FileInfo configFile)
        {
            var configEntries = configFile.OpenText().ReadToEnd().Split('\n');
            string destination = null;
            const string destinationField = "destination:";

            foreach (var line in configEntries)
            {
                if (line.StartsWith(destinationField))
                {
                    destination = line.Replace(destinationField, string.Empty).Trim().Replace("\"", string.Empty);
                }
            }

            return destination;
        }
    }
}