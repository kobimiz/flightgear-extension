using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace flightgearExtension.viewModels
{
    abstract public class ViewModel : INotifyPropertyChanged
    {
        public Model model;
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel(Model model)
        {
            setModel(model);
        }
        public virtual void setModel(Model model)
        {
            this.model = model;

            if (model != null)
            {
                model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
            }
        }

        public int VM_frameIndex
        {
            get => model.frameIndex;
            set => model.frameIndex = value;
        }
        public double VM_FPS
        {
            get => model.FPS;
            set => model.FPS = value;
        }
        public string[] VM_Data
        {
            get => model.Data;
            set => model.Data = value;
        }
        public string[] VM_headings
        {
            get => model.headings;
            set => model.headings = value;
        }
        public string VM_csvPath
        {
            get => model.csvPath;
            set => model.csvPath = value;
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
