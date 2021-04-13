using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace flightgearExtension.mvvm
{
    public class dataDisplayViewModel : ViewModel
    {
        public dataDisplayViewModel(Model model) : base(model)
        {
            /*
            <Label Name="altitudeLabel" Content="{Binding Data[0]}"/>
            <Label Name="arispeedLabel"/>
            <Label Name="orientationLabel"/>
            <Label Name="rollLabel"/>
            <Label Name="pitchLabel"/>
            <Label Name="yawLabel"/>
             */
        }

        public override void setModel(Model m)
        {
            base.setModel(m);
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "frameIndex")
                {
                    NotifyPropertyChanged("altitudeLabel");
                    NotifyPropertyChanged("arispeedLabel");
                    NotifyPropertyChanged("orientationLabel");
                    NotifyPropertyChanged("rollLabel");
                    NotifyPropertyChanged("pitchLabel");
                    NotifyPropertyChanged("yawLabel");
                }
            };
        }

        public string altitudeLabel {
            get => getDataFromCurrFrameByName("altitude-ft");
        }
        public string arispeedLabel
        {
            get => getDataFromCurrFrameByName("airspeed-kt");
        }
        public string orientationLabel
        {
            get => getDataFromCurrFrameByName("heading-deg");
        }
        public string rollLabel
        {
            get => getDataFromCurrFrameByName("roll-deg");
        }
        public string pitchLabel
        {
            get => getDataFromCurrFrameByName("pitch-deg");
        }
        public string yawLabel
        {
            get => getDataFromCurrFrameByName("side-slip-deg");
        }

        private string getDataFromCurrFrameByName(string name)
        {
            if (model.Data == null || model.headings == null)
                return null;
            int index = Array.FindIndex(model.headings, (value) => value == name);
            if (index == -1)
                return null;
            if (model.frameIndex >= model.Data.Length)
                return model.Data[model.Data.Length - 1].Split(",")[index];
            return model.Data[model.frameIndex].Split(",")[index];
        }
    }
}
