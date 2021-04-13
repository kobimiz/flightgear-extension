using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace flightgearExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingsView settings;
        private viewModels.Model model;

        //[DllImport("libAnomaly.dll")]
        //public static extern int _Z3addii(int x, int y);

        public MainWindow()
        {
            InitializeComponent();

            //MessageBox.Show(_Z3addii(1,2).ToString());

            model = new viewModels.Model();
            simPlayer.vm.setModel(model);
            dataDisplay.vm.setModel(model);
            data.vm.setModel(model);
            menu.vm.setModel(model);
            menu.vm.setSimPlayerVM(simPlayer.vm);

            settings = new SettingsView();
            try
            {
                // preload if xml file is in place
                data.addVariableList();
            } catch (Exception) { }

            try
            { 
                // preload if csv is set
                simPlayer.vm.loadCSV(settings.getSettingValue("csvPath"));

               if (!simPlayer.vm.loadXML(classes.Utility.getProtocolDir() + "\\playback_small.xml"))
                {
                    MessageBox.Show("Please make sure there is an xml file first.");
                    return;
                }
            } catch (Exception) { }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            simPlayer.vm.closeConnections();
        }
    }
}
