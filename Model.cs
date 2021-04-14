using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Threading;
using OxyPlot;

namespace flightgearExtension.viewModels
{
    public class Model
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_frameIndex;
        private double m_FPS;
        private string[] m_Data;
        private IEnumerable<DataPoint> m_parsedData;
        private string[] m_headings;
        private string m_csvPath;

        public int frameIndex
        {
            get => m_frameIndex;
            set
            {
                m_frameIndex = value;
                NotifyPropertyChanged("frameIndex");
            }
        }
        public double FPS
        {
            get => m_FPS;
            set
            {
                if (value > 0)
                {
                    m_FPS = value;
                    NotifyPropertyChanged("FPS");
                }
            }
        }
        public string[] Data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                NotifyPropertyChanged("Data");
            }
        }
        public string[] headings
        {
            get => m_headings;
            set
            {
                m_headings = value;
                NotifyPropertyChanged("headings");
            }
        }
        public IEnumerable<DataPoint> parsedData
        {
            get => m_parsedData;
            set
            {
                m_parsedData = value;
                NotifyPropertyChanged("parsedData");
            }
        }
        // TODO: consider removing
        public string csvPath
        {
            get => m_csvPath;
            set
            {
                m_csvPath = value;
                NotifyPropertyChanged("csvPath");
            }
        }

        public Model()
        {
            // initialize
            frameIndex = 0;
            FPS = 30;
            Data = null;
            headings = null;
            csvPath = null;
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
