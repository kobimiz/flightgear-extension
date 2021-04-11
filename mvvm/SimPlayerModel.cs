using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Threading;

namespace flightgearExtension.mvvm
{
    class SimPlayerModel : ISimPlayerModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_frameIndex;
        private double m_FPS;
        private string[] m_Data;

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

        public SimPlayerModel()
        {
            // initialize
            frameIndex = 0;
            FPS = 30;
            Data = null;
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
