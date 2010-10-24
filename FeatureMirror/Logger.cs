using System.Diagnostics;
using System.IO;

namespace FeatureMirror
{
    public class Logger
    {
        public static void Log(string message, params object[] args)
        {
            message = string.Format(message + "\n", args);
            File.AppendAllText(@"c:\logs\featuremirror.log.txt", message);
            Debug.Write(message);
        }
    }
}