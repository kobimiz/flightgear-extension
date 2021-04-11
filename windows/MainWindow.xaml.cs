using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace flightgearExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;
        private mvvm.SimPlayerViewModel vm;
        TcpClient client;
        StreamWriter writer;


        public MainWindow()
        {
            InitializeComponent();

            settings = new Settings();
            vm = new mvvm.SimPlayerViewModel(new mvvm.SimPlayerModel());
            DataContext = vm;
            client = null;
            writer = null;

            vm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "VM_frameIndex")
                {
                    try
                    {
                        if (vm.VM_Data != null && writer != null)
                        {
                            if (vm.VM_frameIndex < vm.VM_Data.Length)
                            {
                                writer.WriteLine(vm.VM_Data[vm.VM_frameIndex]);
                                writer.Flush();
                            }
                            else
                            {
                                vm.pause();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Simulation not running with the correct settings.");
                        vm.pause();
                    }
                }
            };
        }

        private void launchFG(object sender, RoutedEventArgs e)
        {
            // TODO make sure path leads to flightgear.
            try
            {
                if (!vm.loadCSV(settings.getSettingValue("csvPath")))
                {
                    MessageBox.Show("Please load a CSV file first.");
                    return;
                }
                // NOTE this does not work if the user changes the dir name from data (or if changed the dir protocol)
                // TODO: fix this

                // creates a folder in the data dir, this may have authorization issues
                string fgPath = settings.getSettingValue("fgPath");
                string dataDir = Directory.GetParent(Path.GetDirectoryName(fgPath)) + "\\data";
                string protocolDir = dataDir + "\\protocol";
                if (!System.IO.File.Exists(protocolDir + "/playback_small.xml"))
                {
                    // TODO: give the user a nice way to do this
                    MessageBox.Show("Please make sure the directory '" + protocolDir + "' contains the flight information file as the file 'playback_small.xml'");
                    return;
                }

                Process p = Process.Start(fgPath,
                            "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fg-root=\"" + dataDir + "\" --timeofday=noon");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Not a valid path of Flightgear. Please change it in the settings window.");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Not a valid path of Flightgear. Please change it in the settings window.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Not a valid path of Flightgear. Please change it in the settings window.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown error occured."  + ex.Message);
            }
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            settings.ShowDialog();
            settings.Close();
        }

        private string getProcessName(string processPath)
        {
            int index = processPath.LastIndexOf('\\');
            if (index == -1)
                index = processPath.LastIndexOf('/');
            string outout = processPath.Substring(index + 1);
            int extensionIndex = outout.LastIndexOf('.');
            if (extensionIndex == -1)
                extensionIndex = outout.Length;
            return outout.Substring(0, extensionIndex);
        }

        public void startSim(object sender, RoutedEventArgs e)
        {
            // check if flightgear is running
            // setting name is the name of the label that it is shown at in the windows settings.
            // TODO: think of a better way
            if (vm.VM_Data == null)
            {
                MessageBox.Show("Please load a CSV file first.");
                return;
            }
            try
            {
                Process[] pname = Process.GetProcessesByName(getProcessName(settings.getSettingValue("fgPath")));
                if (pname.Length == 0)
                {
                    // TODO: explain to the user what constitutes as correct settings 
                    MessageBox.Show("Please make sure flightgear is running with the correct settings or run via the 'Launch flightgear' button.");
                    return;
                }

                client = new TcpClient("localhost", 5400);
                writer = new StreamWriter(client.GetStream());
                vm.VM_frameIndex = 0;
                vm.play();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please open flightgear first or wait for it to load properly.");
                vm.pause();
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("An unknown message has occured.");
                vm.pause();
            }
        }

        public void play(object sender, RoutedEventArgs e)
        {
            if (client == null)
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.play();
        }
        public void pause(object sender, RoutedEventArgs e)
        {
            if (client == null)
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.pause();
        }
        public void FiveSecFor(object sender, RoutedEventArgs e)
        {
            if (client == null)
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.skip(5);
        }

        public void FiveSecPrev(object sender, RoutedEventArgs e)
        {
            if (client == null)
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.skip(-5);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (writer != null)
            {
                writer.Close();
                client.Close();
            }
        }

        private void speedTb_PrevTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                TextBox origin = sender as TextBox;
                if (origin.Text + e.Text == "" || (e.Text == "." && origin.Text.IndexOf(".") == -1))
                    return;
                // try to parse
                int speed = int.Parse(origin.Text + e.Text);
                // limits to speed
                if (speed <= 0.0) throw new Exception("Invalid speed");
            }
            catch (Exception)
            {
                // if failed, dont allow edit
                e.Handled = true;
            }
        }
    }
}
