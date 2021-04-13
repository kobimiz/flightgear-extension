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

namespace flightgearExtension.viewModels
{
    public class GraphViewModel : ViewModel
    {
        private PlotModel selectedGraph;
        private classes.CsvDocument csv;

        public PlotModel SelectedGraph
        {
            get { return selectedGraph; }
            set { selectedGraph = value; NotifyPropertyChanged("SelectedGraph"); }
        }
        private PlotModel correlatedGraph;
        public PlotModel CorrelatedGraph
        {
            get { return correlatedGraph; }
            set { correlatedGraph = value; NotifyPropertyChanged("CorrelatedGraph"); }
        }
        private PlotModel regressionGraph;
        public PlotModel RegressionGraph
        {
            get { return regressionGraph; }
            set { regressionGraph = value; NotifyPropertyChanged("RegressionGraph"); }
        }
        public GraphViewModel(Model model) : base(model)
        {
            SelectedGraph = Open("C:/Users/kobim/Desktop/pop.csv");
            CorrelatedGraph = Open("C:/Users/kobim/Desktop/temp.csv");
            RegressionGraph = Open("C:/Users/kobim/Desktop/riverflow.csv");

            csv = null;
        }

        public override void setModel(Model m)
        {
            base.setModel(m);
            if (m == null)
                return;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "csvPath")
                {
                    updatePoints(regressionGraph, 0, 0);
                }
            };
        }
        public void updatePoints(PlotModel graph, int index, int frameIndex)
        {
            LineSeries ls = graph.Series[0] as LineSeries;
            if (frameIndex > ls.Points.Count - 1)
            {
                // need to add points that are missing
                ls.Points.AddRange(
                    VM_Data.Select(line => new DataPoint(1.0,1.0))
                );
            }
            MessageBox.Show(ls.Points.Count.ToString());
        }
        public PlotModel Open(string file)
        {
            csv = new classes.CsvDocument();
            csv.Load(file);
            PlotModel tmp = new PlotModel
            {
                Title = Path.GetFileNameWithoutExtension(file),
                PlotMargins = new OxyThickness(50, 0, 0, 40)
            };

            for (int i = 1; i < csv.Headers.Length; i++)
            {
                var ls = new LineSeries { Title = csv.Headers[i] };
                foreach (var item in csv.Items)
                {
                    double x = this.ParseDouble(item[0]);
                    double y = this.ParseDouble(item[i]);
                    ls.Points.Add(new DataPoint(x, y));
                }

                tmp.Series.Add(ls);
            }

            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = csv.Headers[0] });
            return tmp;
        }

        // index is the index of the variable in the csv file in each row
        /*public PlotModel createGraphFromIndex(int index)
        {
            var doc = new classes.CsvDocument();
            //doc.Load();
            PlotModel tmp = new PlotModel
            {
                Title = Path.GetFileNameWithoutExtension(file),
                PlotMargins = new OxyThickness(50, 0, 0, 40)
            };

            for (int i = 1; i < doc.Headers.Length; i++)
            {
                var ls = new LineSeries { Title = doc.Headers[i] };
                foreach (var item in doc.Items)
                {
                    double x = this.ParseDouble(item[0]);
                    double y = this.ParseDouble(item[i]);
                    ls.Points.Add(new DataPoint(x, y));
                }

                tmp.Series.Add(ls);
            }

            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = doc.Headers[0] });
            return tmp;
        }*/

        private double ParseDouble(string s)
        {
            if (s == null)
            {
                return double.NaN;
            }
            s = s.Replace(',', '.');
            double result;
            if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return double.NaN;
        }
    }
}
