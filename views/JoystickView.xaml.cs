using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace flightgearExtension.viewModels
{
    public partial class JoystickView : UserControl
    {
        public viewModels.JoystickViewModel vm;
        public JoystickView()
        {
            InitializeComponent();
            vm = new JoystickViewModel(null);
            DataContext = vm;
        }
        /*public void setValues()
        {
            // load variable values from xml file
            //string variableValues = classes.Utility.getVariableValue(classes.Utility.getProtocolDir() + "\\playback_small.xml","rudder");
            //rudderSlider.Value=(float.Parse(variableValues[2]));
        }*/
    }
}