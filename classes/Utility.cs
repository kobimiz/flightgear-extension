using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace flightgearExtension.classes
{
    class Utility
    {
        static Settings settings = new Settings();
        public static string[] getVariableNamesFromXML(string XMLPath)
        {
            XmlDocument d = new XmlDocument();
            d.Load(XMLPath);
            XmlNodeList x = d.GetElementsByTagName("chunk");
            string[] res = new string[x.Count];
            for (int i = 0; i < x.Count; i++)
                res[i] = x[i].SelectSingleNode("name").InnerText;
            return res;
        }
        // NOTE these does not work if the user changes the dir name from data (or if changed the dir protocol)
        // TODO: fix this
        public static string getFgPath()
        {
            string fgPath = settings.getSettingValue("fgPath");
            return fgPath;
        }
        public static string getDataDir()
        {
            string fgPath = settings.getSettingValue("fgPath");
            string dataDir = Directory.GetParent(System.IO.Path.GetDirectoryName(fgPath)) + "\\data";
            return dataDir;
        }
        public static string getProtocolDir()
        {
            string fgPath = settings.getSettingValue("fgPath");
            string dataDir = Directory.GetParent(System.IO.Path.GetDirectoryName(fgPath)) + "\\data";
            string protocolDir = dataDir + "\\protocol";
            return protocolDir;
        }
    }
}
