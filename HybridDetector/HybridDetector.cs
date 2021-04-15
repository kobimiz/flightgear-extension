using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HybridDetector
{
    public class Detector : SimpleDetector
    {
        public override AnomalyReport[] detectAnomalies(CsvDocument analyze)
        {
            List<AnomalyReport> res = new List<AnomalyReport>();

            for (int i = 0; i < correlated.Length; i++)
            {
                if (correlated[i].pearson >= 0.9)
                {
                    res.AddRange(detectRegressionAnomaly(analyze, i));
                }
                else if (correlated[i].pearson >= 0.5)
                {

                    double[][] columns = analyze.Headers.Select((header, index) => getFeatureArray(analyze, index)).ToArray();
                    double[] cf1Column = columns[correlated[i].feature1Index];
                    double[] cf2Column = columns[correlated[i].feature2Index];

                    Point[] points = getPoints(columns);
                    for (int j = 0; j < points.Length; j++)
                    {
                        Circle c = getMinCircle(columns);
                        if (!c.isInCircle(points[j].x, points[j].y))
                            res.Add(new AnomalyReport(correlated[i].feature1Index, correlated[i].feature2Index, j + 1));
                    }
                }
            }
            return res.ToArray();
        }

        private Circle getMinCircle(double[][] columns)
        {
            Point[] points = getPoints(columns);
            Circle c = Circle.findMinCircle(points);

            c.r *= 1.1;
            return c;
        }

        private static Point[] getPoints(double[][] columns)
        {
            return columns.Select((column, index) => new Point(index, column[index])).ToArray();
        }
    }
}
