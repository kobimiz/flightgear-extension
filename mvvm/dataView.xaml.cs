using System;
using System.Collections.Generic;
using System.IO;
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

namespace flightgearExtension.mvvm
{
    /// <summary>
    /// Interaction logic for dataView.xaml
    /// </summary>
    public partial class dataView : UserControl
    {
        public dataView()
        {
            InitializeComponent();
        }
        public void addVariableList()
        {
            // load variables from xml file
            string[] variableNames = classes.Utility.getVariableNamesFromXML(classes.Utility.getProtocolDir() + "\\playback_small.xml");
            foreach (var item in variableNames)
            {
                var i = new ListBoxItem();
                i.Content = item;

                variableList.Items.Add(i);
            }
        }
    }
}
