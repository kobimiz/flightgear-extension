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

namespace flightgearExtension.viewModels
{
    public class GraphViewModel : ViewModel
    {
        private PlotModel selectedGraph;
        private PlotModel correlatedGraph;
        private PlotModel regressionGraph;

        private int selectedGraphIndex;
        private int correlatedGraphIndex;
        private int regressionGraphIndex;

        private CsvDocument csv;

        public int SelectedGraphIndex
        {
            get { return selectedGraphIndex; }
            set { 
                selectedGraphIndex = value;
                // TODO: modify reg and cor graph indices
                NotifyPropertyChanged("SelectedGraphIndex");
                if (csv != null)
                {
                    correlatedGraphIndex = mostCorrelativeIndex();

                    SelectedGraph = createGraphFromIndex(selectedGraphIndex, VM_headings[selectedGraphIndex]);
                    CorrelatedGraph = createGraphFromIndex(correlatedGraphIndex, "Correlated: " + VM_headings[correlatedGraphIndex]);
                    model.NotifyPropertyChanged("frameIndex");
                }
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
            regressionGraphIndex = 2;
            csv = new CsvDocument();
        }

        public override void setModel(Model m)
        {
            base.setModel(m);
            if (m == null)
                return;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "frameIndex")
                {
                    updatePoints(SelectedGraph, selectedGraphIndex, VM_frameIndex);
                    updatePoints(CorrelatedGraph, correlatedGraphIndex, VM_frameIndex);
                    updatePoints(regressionGraph, regressionGraphIndex, VM_frameIndex);
                    selectedGraph.InvalidatePlot(true);
                    correlatedGraph.InvalidatePlot(true);
                    regressionGraph.InvalidatePlot(true);

                }
                else if (e.PropertyName == "Data")
                {
                    SettingsViewModel vm = new SettingsViewModel(model);
                    if (csv.Load(vm.getSettingValue("csvPath")))
                    {
                        SelectedGraph = createGraphFromIndex(selectedGraphIndex, VM_headings[selectedGraphIndex]);
                        CorrelatedGraph = createGraphFromIndex(correlatedGraphIndex, "Correlated: " + VM_headings[correlatedGraphIndex]);
                        RegressionGraph = createGraphFromIndex(regressionGraphIndex, "Linear regression between the two");
                    }
                    else
                        MessageBox.Show("There was a problem loading the csv file. Make sure it is not opened and can be read.");
                }
            };
            PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "SelectedGraphIndex")
                {
                    updatePoints(SelectedGraph, selectedGraphIndex, VM_frameIndex);
                    updatePoints(CorrelatedGraph, selectedGraphIndex, VM_frameIndex);
                    updatePoints(regressionGraph, selectedGraphIndex, VM_frameIndex);
                    selectedGraph.InvalidatePlot(true);
                    correlatedGraph.InvalidatePlot(true);
                    regressionGraph.InvalidatePlot(true);
                }
            };
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

                lock (this)
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
            SettingsViewModel vm = new SettingsViewModel(model);
            
            PlotModel tmp = new PlotModel
            {
                Title = name,
                PlotMargins = new OxyThickness(50, 0, 0, 40)
            };

            var ls = new LineSeries { Title = csv.Headers[index] };
            tmp.Series.Add(ls);

            // TODO handle non header csv
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            return tmp;
        }

        // returns the index of the most correlated feature to the corrently selected feature
        private int mostCorrelativeIndex()
        {
            double[] currFeatureArray = getFeatureArray(selectedGraphIndex);
            double[] pearsons = VM_headings.Select((heading, index) => Utility.pearson(getFeatureArray(index), currFeatureArray)).
                Where((item, index) => index != selectedGraphIndex).ToArray(); // remove pearson of feature with itself
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
