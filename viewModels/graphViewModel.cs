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
                    updatePoints(CorrelatedGraph, selectedGraphIndex, VM_frameIndex);
                    updatePoints(regressionGraph, selectedGraphIndex, VM_frameIndex);
                    selectedGraph.InvalidatePlot(true);
                    correlatedGraph.InvalidatePlot(true);
                    regressionGraph.InvalidatePlot(true);

                }
                else if (e.PropertyName == "Data")
                {
                    SettingsViewModel vm = new SettingsViewModel(model);
                    csv.Load(vm.getSettingValue("csvPath"));

                    selectedGraph = createGraphFromIndex(selectedGraphIndex);
                    correlatedGraph = createGraphFromIndex(correlatedGraphIndex);
                    regressionGraph = createGraphFromIndex(regressionGraphIndex);
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
        public PlotModel createGraphFromIndex(int index)
        {
            SettingsViewModel vm = new SettingsViewModel(model);
            
            PlotModel tmp = new PlotModel
            {
                Title = Path.GetFileNameWithoutExtension(vm.getSettingValue("csvPath")),
                PlotMargins = new OxyThickness(50, 0, 0, 40)
            };

            var ls = new LineSeries { Title = csv.Headers[index] };
            tmp.Series.Add(ls);

            // TODO handle non header csv
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = csv.Headers[0] });
            return tmp;
        }
    }
}
