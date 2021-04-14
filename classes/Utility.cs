using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace flightgearExtension.classes
{
    class Utility
    {
        static SettingsView settings = new SettingsView();
        public static string[] getVariableNamesFromXML(string XMLPath)
        {
            XmlDocument d = new XmlDocument();
            d.Load(XMLPath);
            XmlNode input = d.GetElementsByTagName("input")[0];
            XmlNodeList x = input.SelectNodes("chunk");
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


        // math functions
        public static double avg(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; sum += x[i], i++) ;
            return sum / x.Length;
        }

        // returns the variance of X and Y
        public static double var(double[] x)
        {
            double av = avg(x);
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * x[i];
            }
            return sum / x.Length - av * av;
        }

        // returns the covariance of X and Y
        public static double cov(double[] x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];
            sum /= x.Length;

            return sum - avg(x) * avg(y);
        }


        // returns the Pearson correlation coefficient of X and Y
        public static double pearson(double[] x, double[] y)
        {
            double c = cov(x,y);
            double svx = Math.Sqrt(var(x));
            double svy = Math.Sqrt(var(y));
            return cov(x, y) / (Math.Sqrt(var(x)) * Math.Sqrt(var(y)));
        }

        // performs a linear regression and returns the line equation
        public static Tuple<double, double> linear_reg(double[] x, double[] y)
        {
            List<DataPoint> l = new List<DataPoint>();
            double a = cov(x, y) / var(x);
            double b = avg(y) - a * avg(x);

            return new Tuple<double, double>(a, b);
        }
        /*
        // returns the deviation between point p and the line equation of the points
        double dev(Point p, Point** points)
        {
            Line l = linear_reg(points, x.Length);
            return dev(p, l);
        }

        // returns the deviation between point p and the line
        double dev(Point p, Line l)
        {
            return abs(p.y - l.f(p.x));
        }
        */
    }
}
