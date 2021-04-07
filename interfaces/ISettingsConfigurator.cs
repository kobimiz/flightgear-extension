using System;
using System.Collections.Generic;
using System.Text;

namespace flightgearExtension
{
    interface ISettingsConfigurator
    {
        public void Set(string name, string value);
        public string Get(string name);
        public void saveToFile();
        public string this[string s]
        {
            get;
            set;
        }
    }
}
