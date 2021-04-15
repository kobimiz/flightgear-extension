using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace HybridDetector
{
    public class Utility
    {
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

        // math functions
        public static double avg(double[] x)
        {
            return x.Average();
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
        // returns the deviation between point p and the line equation of the points
        public static double dev(DataPoint p, double[] x, double[] y)
        {
            Tuple<double, double> line = linear_reg(x, y);
            return dev(p, line);
        }

        // returns the deviation between point p and the line
        public static double dev(DataPoint p, Tuple<double, double> line)
        {
            double lineEvaledAtPx = line.Item1 * p.X + line.Item2;
            return Math.Abs(p.Y - lineEvaledAtPx);
        }
    }
}
