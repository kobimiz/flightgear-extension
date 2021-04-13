using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace flightgearExtension.viewModels
{
    public class SettingsViewModel : ViewModel
    {
        public CsvDocument csv;

        private ISettingsConfigurator settings;
        // map buttons to labels and line index in config file
        public Dictionary<object, Label> buttonLabelMap;

        public SettingsViewModel(Model model) : base(model)
        {
            csv = null;
            // initialize button labal map for change path function
            buttonLabelMap = new Dictionary<object, Label>();

            settings = new SettingsConfigurator();
        }
        public void changeSettings(object sender)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == false)
                return;

            settings.Set(buttonLabelMap[sender].Name, dlg.FileName);
            settings.saveToFile();
            NotifyPropertyChanged(buttonLabelMap[sender].Name);
            updateLabels();
        }
        public void changeFGLocation()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == false)
                return;

            settings.Set("fgPath", dlg.FileName);
            settings.saveToFile();
            updateLabels();
        }
        public void changeCSVLocation()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == false)
                return;

            settings.Set("csvPath", dlg.FileName);
            settings.saveToFile();
            updateLabels();
        }
        public void updateLabels()
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
