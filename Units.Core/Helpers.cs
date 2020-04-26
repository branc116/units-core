using System;
using System.Collections.Generic;
using System.IO;

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
    }
}
