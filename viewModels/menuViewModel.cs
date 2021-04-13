using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace flightgearExtension.viewModels
{
    public class menuViewModel : ViewModel
    {
        private SimPlayerViewModel simPlayerVM;
        public menuViewModel(Model model) : base(model)
        {
        }
        public void setSimPlayerVM(SimPlayerViewModel vm)
        {
            simPlayerVM = vm;
        }
        public void launchFG(object sender)
        {
            // TODO make sure path leads to flightgear.
            try
            {
                SettingsView settings = new SettingsView();
                settings.vm.setModel(model);

                if (!simPlayerVM.loadCSV(settings.getSettingValue("csvPath")))
                {
                    MessageBox.Show("Please load a CSV file first.");
                    return;
                }


                Process[] pname = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(settings.getSettingValue("fgPath")));
                if (pname.Length == 1)
                {
                    // TODO: explain to the user what constitutes as correct settings 
                    MessageBox.Show("Flightgear already running.");
                    return;
                }

                string protocolDir = classes.Utility.getProtocolDir();
                if (!System.IO.File.Exists(protocolDir + "/playback_small.xml"))
                {
                    // TODO: give the user a nice way to do this
                    MessageBox.Show("Please make sure the directory '" + protocolDir + "' contains the flight information file as the file 'playback_small.xml'");
                    return;
                }

                if (!simPlayerVM.loadXML(protocolDir + "\\playback_small.xml"))
                {
                    MessageBox.Show("Please make sure there is an xml file first.");
                    return;
                }
                Process p = Process.Start(classes.Utility.getFgPath(),
                            "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fg-root=\"" + classes.Utility.getDataDir() + "\" --timeofday=noon");
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is ArgumentException)
                    MessageBox.Show("Not a valid path of Flightgear. Please change it in the settings window.");
                else
                    MessageBox.Show("Unknown error occured." + ex.Message);
            }
        }
        public void openSettings()
        {
            SettingsView settings = new SettingsView();
            settings.vm.setModel(model);
            settings.vm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "csvPath")
                    simPlayerVM.loadCSV(settings.getSettingValue("csvPath"));
            };
            settings.ShowDialog();
            settings.Close();
        }

        public void startSim()
        {
            // check if flightgear is running
            // setting name is the name of the label that it is shown at in the windows settings.
            // TODO: think of a better way
            if (VM_Data == null)
            {
                MessageBox.Show("Please load a CSV file first.");
                return;
            }
            try
            {
                SettingsView settings = new SettingsView();
                settings.vm.setModel(model);

                Process[] pname = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(settings.getSettingValue("fgPath")));
                if (pname.Length == 0)
                {
                    // TODO: explain to the user what constitutes as correct settings 
                    MessageBox.Show("Please make sure flightgear is running with the correct settings or run via the 'Launch flightgear' button.");
                    return;
                }
                simPlayerVM.startSim();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please open flightgear first or wait for it to load properly.");
                simPlayerVM.pause();
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("An unknown message has occured.");
                simPlayerVM.pause();
            }
        }

    }
}
