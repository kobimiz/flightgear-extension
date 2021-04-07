using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace flightgearExtension.classes
{
    public class SettingsConfigurator : ISettingsConfigurator
    {
        private string fileName;
        private Dictionary<string, string> settings;

        public SettingsConfigurator()
        {
            fileName = "./settings.config";
            settings = new Dictionary<string, string>();
            loadFromFile();
        }

        public string this[string s] { get => Get(s); set => Set(s, value); }

        public string Get(string name)
        {
            if (settings.ContainsKey(name))
                return settings[name];
            return "";
        }

        public void Set(string name, string value)
        {
            settings[name] = value;
        }

        // saves current dictionary to file
        public void saveToFile()
        {
            FileStream f;
            if (!File.Exists(fileName))
            {
                // create the file and write the line to it
                f = System.IO.File.Create(fileName);
            }
            else
            {
                f = File.OpenWrite(fileName);
            }

            System.IO.StreamWriter writer = new StreamWriter(f);
            foreach (var item in settings)
            {
                writer.WriteLine(item.Key + "," + item.Value);
            }
            writer.Close();
        }

        // loads saved settings from file
        private void loadFromFile()
        {
            if (File.Exists(fileName))
            {
                System.IO.StreamReader reader = new StreamReader(fileName);
                string line = reader.ReadLine();
                while (line != null)
                {
                    // NOTE that if the key contains a comma this fails
                    int i = line.IndexOf(',');
                    if (i != -1)
                    {
                        // else ignore.
                        string key = line.Substring(0, i);
                        string value = line.Substring(i + 1);
                        settings[key] = value;
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
            }
        }
    }
}
