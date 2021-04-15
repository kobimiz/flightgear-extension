using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HybridDetector
{
    public class SimpleDetector : AnomalyDetector
    {
		protected CorrelatedFeature[] correlated;

		public virtual AnomalyReport[] detectAnomalies(CsvDocument analyze)
        {
			List<AnomalyReport> anomalyReports = new List<AnomalyReport>();
            for (int i = 0; i < correlated.Length; i++)
				anomalyReports.AddRange(detectRegressionAnomaly(analyze, i));
			return anomalyReports.ToArray();
		}

        public virtual void learnFlight(CsvDocument learningModel)
        {
			double[][] columns = learningModel.Headers.Select((header, index) => getFeatureArray(learningModel, index)).ToArray();

			correlated = columns.Select((column, index) => getCorrelatedFeature(columns, index)).ToArray();

			correlated = correlated.Where(maxPearson => isAboveThreshold(maxPearson.pearson)).ToArray();
		}

		protected double getMaxDistance(double[] xVals, double[] yVals, Tuple<double, double> line)
		{
			return xVals.Select((x, i) => Utility.dev(new OxyPlot.DataPoint(x, yVals[i]), line)).Max();
		}
		protected List<AnomalyReport> detectRegressionAnomaly(CsvDocument analyze, int index)
		{
			int f1Index = correlated[index].feature1Index;
			int f2Index = correlated[index].feature2Index;

			List<AnomalyReport> reports = new List<AnomalyReport>();

            for (int j = 0; j < analyze.Items.Count; j++)
            {
				double x = double.Parse(analyze.Items[j][f1Index]);
				double y = double.Parse(analyze.Items[j][f2Index]);

				if (Utility.dev(new OxyPlot.DataPoint(x, y), correlated[index].linReg) >= correlated[index].maxDistance * 1.1)
					reports.Add(new AnomalyReport(f1Index, f2Index, j));
            }
			return reports;
		}
		protected bool isAboveThreshold(double d)
		{
			return d >= 0.9;
		}

		protected virtual CorrelatedFeature getCorrelatedFeature(double[][] columns, int columnIndex)
		{
			double[] pearsons = columns.Select((column, index) => Utility.pearson(columns[columnIndex], columns[index])).Where((column, index) => index != columnIndex).ToArray();
			double maxPearson = pearsons.Max();
			int indexOfMax = Array.FindIndex(pearsons, pearson => pearson == maxPearson);
			if (double.IsNaN(maxPearson))
			{
				indexOfMax = 0;
			}

			Tuple<double,double> linReg = Utility.linear_reg(columns[columnIndex], columns[indexOfMax]);
			double maxDistance = getMaxDistance(columns[columnIndex], columns[indexOfMax], linReg);
			return new CorrelatedFeature(columnIndex, indexOfMax, maxPearson, maxDistance, linReg);
		}

		protected double[] getFeatureArray(CsvDocument csv, int index)
		{
			return csv.Items.Select(row => double.Parse(row[index])).ToArray();
		}
    }
}
