using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace flightgearExtension.classes
{
    public class CsvDocument
    {
        public string[] Headers { get; private set; }
        public Collection<string[]> Items { get; private set; }

        /// <summary>
        /// Loads the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="separator">The separator (auto-detect if not specified).</param>
        public void Load(string fileName, char separator = '\0')
        {
            using (var r = new StreamReader(fileName))
            {
                var header = r.ReadLine();

                if (separator == '\0')
                {
                    // Auto detect
                    int commaCount = Count(header, ',');
                    int semicolonCount = Count(header, ';');
                    separator = commaCount > semicolonCount ? ',' : ';';
                }

                Headers = header.Split(separator);
                Items = new Collection<string[]>();

                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line == null || line.StartsWith("%") || line.StartsWith("//"))
                        continue;
                    Items.Add(line.Split(separator));
                }
            }
        }

        private int Count(string s, char c)
        {
            return s.Count(ch => ch == c);
        }
    }
}
