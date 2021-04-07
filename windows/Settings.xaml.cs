using flightgearExtension.classes;
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
    public partial class Settings : Window
    {
        ISettingsConfigurator settings;
        // map buttons to labels and line index in config file
        private Dictionary<object, Label> buttonLabelMap;

        public Settings()
        {
            InitializeComponent();

            // initialize button labal map for change path function
            buttonLabelMap = new Dictionary<object, Label>();
            buttonLabelMap[openFG] = fgPath;
            buttonLabelMap[openXML] = xmlPath;
            buttonLabelMap[openCSV] = csvPath;

            settings = new SettingsConfigurator();
            updateLabels();
        }

        private void changeSettings(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files |*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == false)
            {
                return;
            }
            
            settings.Set(buttonLabelMap[sender].Name, dlg.FileName);
            settings.saveToFile();
            updateLabels();
        }
        private void updateLabels()
        {
            foreach (var item in buttonLabelMap)
            {
                string value = settings.Get(item.Value.Name);
                item.Value.Content = value == "" ? "Not set" : value;
            }
        }

        public string getSettingValue(string key)
        {
            return settings.Get(key);
        }
    }
}
