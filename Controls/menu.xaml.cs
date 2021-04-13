using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace flightgearExtension.Controls
{
    /// <summary>
    /// Interaction logic for menu.xaml
    /// </summary>
    public partial class menu : UserControl
    {
        private Settings settings;
        private mvvm.SimPlayerView simPlayer;

        public menu()
        {
            InitializeComponent();
            settings = new Settings();
        }

        public void setSimPlayer(mvvm.SimPlayerView simPlayer)
        {
            this.simPlayer = simPlayer;
        }

        private void launchFG(object sender, RoutedEventArgs e)
        {
            // TODO make sure path leads to flightgear.
            try
            {
                if (!simPlayer.vm.loadCSV(settings.getSettingValue("csvPath")))
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

                if (!simPlayer.vm.loadXML(protocolDir + "\\playback_small.xml"))
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

        private void openSettings(object sender, RoutedEventArgs e)
        {
            settings.ShowDialog();
            settings.Close();
        }

        public void startSim(object sender, RoutedEventArgs e)
        {
            // check if flightgear is running
            // setting name is the name of the label that it is shown at in the windows settings.
            // TODO: think of a better way
            if (simPlayer.vm.VM_Data == null)
            {
                MessageBox.Show("Please load a CSV file first.");
                return;
            }
            try
            {
                Process[] pname = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(settings.getSettingValue("fgPath")));
                if (pname.Length == 0)
                {
                    // TODO: explain to the user what constitutes as correct settings 
                    MessageBox.Show("Please make sure flightgear is running with the correct settings or run via the 'Launch flightgear' button.");
                    return;
                }
                simPlayer.vm.startSim();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please open flightgear first or wait for it to load properly.");
                simPlayer.vm.pause();
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("An unknown message has occured.");
                simPlayer.vm.pause();
            }
        }
    }
}
