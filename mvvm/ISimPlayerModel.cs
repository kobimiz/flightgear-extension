using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace flightgearExtension.mvvm
{
    // Simulator player
    interface ISimPlayerModel :INotifyPropertyChanged
    {
        public int frameIndex { get; set; }
        public double FPS { get; set; }
        public string[] Data { get; set; }
        public void NotifyPropertyChanged(string propName);
    }
}
