using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VerifyMinifiedFiles
{
    class BundleFileName
    {
        public string outputFileName { get; set; }

        public List<string> inputFiles { get; set; }
    }

    class Program
    {

        const string ROOT = "C:\\Dev\\IXCYS\\";

        static string GetRootFolder(string path)
        {
            while (true)
            {
                string temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;
                path = temp;
            }
            return path;
        }

        static void writeFile(List<BundleFileName> bundles)
        {
            var json = JsonConvert.SerializeObject(bundles, Formatting.Indented);
            File.WriteAllText(ROOT + "bundleconfig.json", json);
        }

        static void Main(string[] args)
        {
            List<IEnumerable<string>> files = new List<IEnumerable<string>>();
            List<string> extensions = new List<string>() { ".min.js", ".min.css", ".min.html" };
            List<string> excludedDirs = new List<string>() {
                "bin",
                "Logs",
                "Migrations",
                "obj",
                "Resources",
                "Temp",
                "Test",
                "Tests",
                "x64",
                "x86",
                "Ged/EmailTemplate/RecoverPassword",
                "Ged/lib/angular",
            };

            var jsFiles = Directory.EnumerateFiles(ROOT, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".js") && !s.Contains(".min."));
            var cssFiles = Directory.EnumerateFiles(ROOT, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".css") && !s.Contains(".min."));
            var htmlFiles = Directory.EnumerateFiles(ROOT, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".html") && !s.Contains(".min."));

            files.Add(jsFiles);
            files.Add(cssFiles);
            files.Add(htmlFiles);

            List<BundleFileName> bundles = new List<BundleFileName>();

            int i = 0;
            foreach (var typedFiles in files)
            {

                var extension = extensions[i];
                foreach (var currentFile in typedFiles)
                {

                    var expectedMinFile = Path.ChangeExtension(currentFile, extension);
                    //exclude file that are marked as excluded
                    var outputFileName = currentFile.Remove(0, ROOT.Length).Replace("\\", "/");

                    bool shouldAdd = true;
                    int index = 0;
                    while (shouldAdd && index < excludedDirs.Count)
                    {
                        if (outputFileName.StartsWith(excludedDirs[index]))
                        {
                            shouldAdd = false;
                        }
                        index++;
                    }
                    if (shouldAdd)
                    {
                        //gets notification if doesn't exists
                        if (!File.Exists(expectedMinFile))
                        {
                            Console.WriteLine("Didn't find expected minified file " + expectedMinFile);
                        }


                        bundles.Add(new BundleFileName()
                        {
                            outputFileName = outputFileName,
                            inputFiles = new List<string>() { outputFileName }
                        });
                    }
                }
                i++;    //helps for extension
            }

            writeFile(bundles);

        }
    }

    internal class Enumerable<T>
    {
    }
}
