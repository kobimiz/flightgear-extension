using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace flightgearExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings;

        public MainWindow()
        {
            InitializeComponent();

            settings = new Settings();
        }

        private void openXML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files |*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
            }
        }

        private void launchFG(object sender, RoutedEventArgs e)
        {
            // TODO make sure path leads to flightgear.
            try
            {
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
                            "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fg-root=\"" + dataDir + "\"");

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Not a valid path of Flightgear. Please change it in the settings window.");
            }
            catch (Exception)
            {
                MessageBox.Show("Unknown error occured.");
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

            // TODO: the application hangs when starting the simulation. fix this maybe by using a different thread.

            // check if flightgear is running
            // setting name is the name of the label that it is shown at in the windows settings.
            // TODO: think of a better way
            Process[] pname = Process.GetProcessesByName(getProcessName(settings.getSettingValue("fgPath")));
            if (pname.Length == 0)
            {
                // TODO: explain to the user what constitutes as correct settings 
                MessageBox.Show("Please make sure flightgear is running with the correct settings or run via the 'Launch flightgear' button.");
                return;
            }

            try
            {
                TcpClient client = new TcpClient("localhost", 5400);
                StreamReader input = new StreamReader(settings.getSettingValue("csvPath"));
                StreamWriter output = new StreamWriter(client.GetStream());
                string line;
                while ((line = input.ReadLine()) != null)
                {
                    output.WriteLine(line);
                    output.Flush();
                    Thread.Sleep(10);
                }
                output.Close();
                input.Close();
                client.Close();
            }
            catch (SocketException)
            {
                // TODO: explain to the user what constitutes as correct settings 
                MessageBox.Show("Please make sure flightgear is running with the correct settings or run via the 'Launch flightgear' button.");
            }
            catch (Exception)
            {
                MessageBox.Show("An unknown error has occured");
            }

        }
    }
}
