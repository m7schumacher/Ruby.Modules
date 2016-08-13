using Ruby.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Ruby.Movements
{
    internal class FileLocator : Reflex
    {
        public FileLocator() : base()
        {
            Name = "FileLocator";
        }

        public override string Execute()
        {
            List<string> paths = new List<string>();

            string search = Core.External.Search;
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string download = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Task<List<string>> doc = Task.Factory.StartNew(() => SearchForFile(documents, search));
            Task<List<string>> down = Task.Factory.StartNew(() => SearchForFile(download, search));
            Task<List<string>> desk = Task.Factory.StartNew(() => SearchForFile(desktop, search));

            paths.AddRange(doc.Result);
            paths.AddRange(down.Result);
            paths.AddRange(desk.Result);

            string response = "Files found sir";

            if (paths.Count > 0)
            {
                DisplaySelector(paths);
            }
            else { response = "No files found sir"; }

            return response;
        }

        public static List<string> SearchForFile(string root, string term)
        {
            List<string> results = new List<string>();

            try
            {
                List<string> res = new List<string>();
                IEnumerable<string> dirs = Directory.EnumerateDirectories(root);
                IEnumerable<string> files = Directory.EnumerateFiles(root);

                Parallel.ForEach(files, s =>
                {
                    if (Path.GetFileName(s).ToLower().Contains(term.ToLower()))
                    {
                        res.Add(s);
                    }
                });

                if (res.Count > 0) { results.AddRange(res); }
                if (!root.Contains("Old Laptop")){ dirs = dirs.Where(str => !str.Contains("Old Laptop")); }

                foreach (string s in dirs)
                {
                    results.AddRange(SearchForFile(s, term));
                }
            }
            catch (Exception trouble) { }

            return results;
        }

        public static void DisplaySelector(List<string> paths)
        {
            //Selector sel = new Selector();

            //sel.Initialize("File", paths);

            //sel.Show();
        }
    }
}
