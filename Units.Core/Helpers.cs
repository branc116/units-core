using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Units.Core
{
    internal static class Helpers
    {
        internal static void CreateDirs(this IEnumerable<string> paths)
        {
            foreach(var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Creating dir: {path}");
                    Directory.CreateDirectory(path);
                }else
                {
                    Console.WriteLine($"Not creating dir: {path}");
                }
            }
        }
        internal static IEnumerable<string> NonEmptyOrWhitespaceLines(this string str)
        {
            var lines = str.Split(Environment.NewLine)
              .Select(i => i.Trim())
              .Where(i => !string.IsNullOrWhiteSpace(i));
            return lines;
        }
    }
}
