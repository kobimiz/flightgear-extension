using flightgearExtension.classes;
using flightgearExtension.viewModels;
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
using System.Windows.Shapes;

namespace flightgearExtension
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsViewModel vm;

        public SettingsView()
        {
            InitializeComponent();
            vm = new SettingsViewModel(null);

            vm.buttonLabelMap[openFG] = fgPath;
            vm.buttonLabelMap[openCSV] = csvPath;

            vm.updateLabels();
        }
        private void changeSettings(object sender, RoutedEventArgs e)
        {
            vm.changeSettings(sender);
        }
        private void changeFGLocation(object sender, RoutedEventArgs e)
        {
            vm.changeFGLocation();
        }
        private void changeCSVLocation(object sender, RoutedEventArgs e)
        {
            vm.changeCSVLocation();
        }

        public string getSettingValue(string key)
        {
            return vm.getSettingValue(key);
        }
    }
}
