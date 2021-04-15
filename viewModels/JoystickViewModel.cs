using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace flightgearExtension.viewModels
{
    public class JoystickViewModel : ViewModel
    {
        //public float joystick_aileron=125;
        //public float joystick_elevator=125;
        public float joystick_aileron
        {
            get;set;
        }
        public float joystick_elevator
        {
            get;set;
        }



        public JoystickViewModel(Model model) : base(model)
        {
            joystick_aileron = 125;
            joystick_elevator = 125;
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
                    NotifyPropertyChanged("rudderLabel");
                    NotifyPropertyChanged("aileronLabel");
                    NotifyPropertyChanged("throttleLabel");
                    NotifyPropertyChanged("elevatorLabel");
                    joystick_aileron = float.Parse(aileronLabel.ToString()) * 60 + 125;
                    joystick_elevator = float.Parse(elevatorLabel.ToString()) * 60 + 125;
                    NotifyPropertyChanged("joystick_aileron");
                    NotifyPropertyChanged("joystick_elevator");


                }
            };

        }
        public string rudderLabel
        {
            get => getDataFromCurrFrameByName("rudder");
            set {
                NotifyPropertyChanged("rudderLabel");
            }
        }
        public string aileronLabel
        {
            get => getDataFromCurrFrameByName("aileron");
            set {
                NotifyPropertyChanged("aileronLabel");
            }
        }
        public string throttleLabel
        {
            get => getDataFromCurrFrameByName("throttle");
            set
            {
                NotifyPropertyChanged("throttleLabel");
            }
        }
        public string elevatorLabel
        {
            get => getDataFromCurrFrameByName("elevator");
            set
            {
                NotifyPropertyChanged("elevatorLabel");
            }
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
