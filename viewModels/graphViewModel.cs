using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using flightgearExtension.classes;
using System.Threading;
using OxyPlot.Annotations;

namespace flightgearExtension.viewModels
{
    public class GraphViewModel : ViewModel
    {
        private PlotModel selectedGraph;
        private PlotModel correlatedGraph;
        private PlotModel regressionGraph;
        private List<DataPoint> regressionPoints;

        private static object selectedLock = new object();
        private static object correlatedLock = new object();
        private static object regressionLock = new object();

        private int selectedGraphIndex;
        private int correlatedGraphIndex;

        private CsvDocument csv;

        public int SelectedGraphIndex
        {
            get { return selectedGraphIndex; }
            set { 
                selectedGraphIndex = value;
                NotifyPropertyChanged("SelectedGraphIndex");
            }
        }

        public PlotModel SelectedGraph
        {
            get { return selectedGraph; }
            set { selectedGraph = value; NotifyPropertyChanged("SelectedGraph"); }
        }
        public PlotModel CorrelatedGraph
        {
            get { return correlatedGraph; }
            set { correlatedGraph = value; NotifyPropertyChanged("CorrelatedGraph"); }
        }
        public PlotModel RegressionGraph
        {
            get { return regressionGraph; }
            set { regressionGraph = value; NotifyPropertyChanged("RegressionGraph"); }
        }

        public GraphViewModel(Model model) : base(model)
        {
            SelectedGraphIndex = 0;
            correlatedGraphIndex = 1;
            csv = new CsvDocument();
            regressionPoints = new List<DataPoint>();
        }

        public override void setModel(Model m)
        {
            base.setModel(m);
            if (m == null)
                return;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "frameIndex")
                    updateGraphs();
                else if (e.PropertyName == "Data")
                {
                    SettingsViewModel vm = new SettingsViewModel(model);
                    if (csv.Load(vm.getSettingValue("csvPath")))
                    {
                        lock (regressionLock)
                        {
                            regressionGraph = createGraphFromIndex(0, "Linear regression between the two");
                        }
                        lock (selectedLock)
                        {
                            SelectedGraph = createGraphFromIndex(selectedGraphIndex, VM_headings[selectedGraphIndex]);
                        }
                        lock (correlatedLock)
                        {
                            CorrelatedGraph = createGraphFromIndex(correlatedGraphIndex, "Correlated: " + VM_headings[correlatedGraphIndex]);
                        }
                        if (VM_Data != null)
                        {
                            recalcRegressionPoints();
                            updateGraphs();
                        }
                    }
                    else
                        MessageBox.Show("There was a problem loading the csv file. Make sure it is not opened and can be read.");
                }
            };
            PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "SelectedGraphIndex")
                {
                    // TODO: modify reg and cor graph indices
                    // TOOD: consider recalculating cor feature per frame
                    if (csv != null)
                    {
                        correlatedGraphIndex = mostCorrelativeIndex();

                        lock (selectedLock)
                        {
                            SelectedGraph = createGraphFromIndex(selectedGraphIndex, VM_headings[selectedGraphIndex]);
                        }
                        lock (correlatedLock)
                        {
                            CorrelatedGraph = createGraphFromIndex(correlatedGraphIndex, "Correlated: " + VM_headings[correlatedGraphIndex]);
                        }
                        (regressionGraph.Series[0] as LineSeries).Points.Clear();
                        recalcRegressionPoints();
                        updateGraphs();
                    }
                }
            };
        }
        public void updateGraphs()
        {
            lock (selectedLock)
            {
                updatePoints(SelectedGraph, selectedGraphIndex, VM_frameIndex);
                selectedGraph.InvalidatePlot(true);
            }
            lock (correlatedLock)
            {
                updatePoints(CorrelatedGraph, correlatedGraphIndex, VM_frameIndex);
                correlatedGraph.InvalidatePlot(true);
            }
            lock (regressionLock)
            {
                lock (correlatedLock)
                {
                    lock (selectedLock)
                    {
                        updateRegression(VM_frameIndex);
                        regressionGraph.InvalidatePlot(true);
                    }
                }
            }
        }
        public void recalcRegressionPoints()
        {
            lock (regressionLock)
            {
                regressionPoints.Clear();
                // TODO: optimize
                Tuple<double, double> line = Utility.linear_reg(getFeatureArray(selectedGraphIndex), getFeatureArray(correlatedGraphIndex));
                for (int i = 0; i < VM_Data.Length; i++)
                {
                    double y = line.Item1 * i + line.Item2;
                    if (double.IsNaN(y))
                        y = 0;
                    regressionPoints.Add(new DataPoint(i, y));
                }
            }
        }
        public void updateRegression(int frameIndex)
        {
            //
            LineSeries ls1 = selectedGraph.Series[0] as LineSeries;
            LineSeries ls2 = correlatedGraph.Series[0] as LineSeries;
            try
            {
                RegressionGraph.Annotations.RemoveAt(0);
                RegressionGraph.Annotations.Add(new PointAnnotation
                {
                    X = ls1.Points[frameIndex].Y,
                    Y = ls2.Points[frameIndex].Y,
                    Shape = MarkerType.Circle,
                    Fill = OxyColors.LightGray,
                    Stroke = OxyColors.DarkGray,
                    StrokeThickness = 1,
                });
            }
            catch(Exception ex)
            {

            int from = Math.Max(frameIndex - 30, 0);
            int until = Math.Min(Math.Min(ls1.Points.Count, ls2.Points.Count), from + 30);
            // display 30 points max
            for (int i = frameIndex; i < frameIndex+1; i++)
            {
                try
                {
                    RegressionGraph.Annotations.Add(new PointAnnotation
                    {
                        X = ls1.Points[i].Y,
                        Y = ls2.Points[i].Y,
                        Shape = MarkerType.Circle,
                        Fill = OxyColors.LightGray,
                        Stroke = OxyColors.DarkGray,
                        StrokeThickness = 1,
                    });
                } catch(Exception ex)
                {

                }
            }*/
            if (frameIndex >= VM_Data.Length)
                return;
            LineSeries ls = RegressionGraph.Series[0] as LineSeries;
            if (frameIndex > ls.Points.Count - 1)
            {
                // need to add points that are missing
                List<DataPoint> range = new List<DataPoint>();
                // TODO: check index validity
                for (int i = ls.Points.Count; i < frameIndex; i++)
                {
                    double x = regressionPoints[i].X;
                    double y = regressionPoints[i].Y;
                    range.Add(new DataPoint(x, y));
                }
                ls.Points.AddRange(range);
            }
            else if (frameIndex < ls.Points.Count - 1)
            {
                // need to remove points
                if (frameIndex < 0)
                    return;
                ls.Points.RemoveRange(frameIndex, ls.Points.Count - frameIndex);
            }
        }


        public void updatePoints(PlotModel graph, int index, int frameIndex)
        {
            if (frameIndex >= VM_Data.Length)
                return;
            LineSeries ls = graph.Series[0] as LineSeries;
            if (frameIndex > ls.Points.Count - 1)
            {
                // need to add points that are missing
                List<DataPoint> range = new List<DataPoint>();
                // TODO: check index validity
                for (int i = ls.Points.Count; i < frameIndex; i++)
                    range.Add(new DataPoint(i, double.Parse(csv.Items[i][index])));

                ls.Points.AddRange(range);
            }
            else if (frameIndex < ls.Points.Count - 1)
            {
                // need to remove points
                if (frameIndex < 0)
                    return;
                ls.Points.RemoveRange(frameIndex, ls.Points.Count - frameIndex);
            }
        }


        // index is the index of the variable in the csv file in each row
        public PlotModel createGraphFromIndex(int index, string name)
        {
            PlotModel tmp = new PlotModel
            {
                Title = name,
                TitleFontSize = 11,
                PlotMargins = new OxyThickness(50, 0, 0, 40)
            };

            var ls = new LineSeries { Title = csv.Headers[index]};
            tmp.Series.Add(ls);

            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            return tmp;
        }

        // returns the index of the most correlated feature to the corrently selected feature
        private int mostCorrelativeIndex()
        {
            double[] currFeatureArray = getFeatureArray(selectedGraphIndex);
            double[] pearsons = VM_headings.Select((heading, index) => {
                double person = Utility.pearson(getFeatureArray(index), currFeatureArray);
                if (double.IsNaN(person))
                    return 0.0;
                return person;
                }).Where((item, index) => index != selectedGraphIndex).ToArray(); // remove pearson of feature with itself
            return pearsons.ToList().IndexOf(pearsons.Max());
        }

        // returns an array of the values of the feature in the given index throughout all frames
        private double[] getFeatureArray(int index)
        {
            //double[] res = csv.Items.Select(row => double.Parse(row[index])).ToArray();
            return csv.Items.Select(row => double.Parse(row[index])).ToArray();
        }
    }
}
