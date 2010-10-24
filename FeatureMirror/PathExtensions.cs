using System;
using System.Collections.Generic;
using System.IO;

namespace FeatureMirror
{
    public static class PathExtensions
    {
        public static List<string> GetAllFilesWithExtension(this string root, string extension)
        {
            return GetAllFiles(root, "*." + extension);
        }

        public static List<string> GetAllInstancesOf(this string root, string fileName)
        {
            return GetAllFiles(root, fileName);
        }

        private static List<string> GetAllFiles(string root, string filter)
        {
            var result = new List<string>();
            var stack = new Stack<string>();

            stack.Push(root);

            while (stack.Count > 0)
            {
                var dir = stack.Pop();

                var directories = Directory.GetFiles(dir, filter);

                result.AddRange(directories);

                foreach (string dn in Directory.GetDirectories(dir))
                {
                    stack.Push(dn);
                }
            }

            return result;
        }
    }
}
