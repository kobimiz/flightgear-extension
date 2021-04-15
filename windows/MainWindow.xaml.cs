using flightgearExtension.classes;
using flightgearExtension.viewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace flightgearExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private SettingsView settings;
        private viewModels.Model model;
        private AnomalyReportViewModel arvm;

        public MainWindow()
        {
            InitializeComponent();

            arvm = new AnomalyReportViewModel();
            arvm.setModel(model);
            anomalyReports.DataContext = arvm;


            settings = new SettingsView();
            model = new viewModels.Model();
            settings.vm.setModel(model);
            joystick.vm.setModel(model);
            simPlayer.vm.setModel(model);
            dataDisplay.vm.setModel(model);
            data.vm.setModel(model);
            menu.vm.setModel(model);
            joystick.vm.setModel(model);

            menu.vm.setSimPlayerVM(simPlayer.vm);
            menu.vm.setSettingsVM(settings.vm);
            menu.vm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "csvPath")
                {
                    SettingsView settings = new SettingsView();
                    simPlayer.vm.loadCSV(settings.getSettingValue("csvPath"));
                }
                    //data.vm.NotifyPropertyChanged("csvPath");
            };
            settings.vm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "csvPath")
                    simPlayer.vm.loadCSV(settings.getSettingValue("csvPath"));
                if (e.PropertyName == "dllPath") 
                {
                    try
                    {
                        loadAnomalies();
                    }
                    catch (Exception)
                    { }
                }
            };


            try
            {
                // preload if xml file is in place
                data.addVariableList();
            } catch (Exception) { }

            try
            { 
               if (!simPlayer.vm.loadXML(classes.Utility.getProtocolDir() + "\\playback_small.xml"))
                {
                    MessageBox.Show("Please make sure there is an xml file first and restart the application.");
                    return;
                }

                // preload if csv is set
                simPlayer.vm.loadCSV(settings.getSettingValue("csvPath"));
            } catch (Exception) { }
        }

        private void loadAnomalies()
        {
            Assembly DLL = Assembly.LoadFile(settings.getSettingValue("dllPath"));
            Type SimpleDetector = Array.Find(DLL.GetExportedTypes(), item => item.Name == "Detector");
            MethodInfo learnFlight = Array.Find(SimpleDetector.GetMethods(), method => method.Name == "learnFlight");
            MethodInfo detect = Array.Find(SimpleDetector.GetMethods(), method => method.Name == "detectAnomalies");

            var c = Activator.CreateInstance(SimpleDetector);
            CsvDocument learnDoc = new CsvDocument();
            learnDoc.Load(settings.getSettingValue("csvPath"));
            CsvDocument detectDoc = new CsvDocument();
            detectDoc.Load(settings.getSettingValue("anomPath"));

            learnFlight.Invoke(c, new object[] { learnDoc });

            AnomalyReport[] anomalies = (AnomalyReport[])(object[])detect.Invoke(c, new object[] { detectDoc });
            ObservableCollection<AnomalyReport> l = new ObservableCollection<AnomalyReport>();
            foreach (var anomaly in anomalies)
            {
                l.Add(anomaly);
            }
            arvm.Reports = l;
            lengthLabel.Content = "Count: " + anomalies.Length.ToString();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            simPlayer.vm.closeConnections();
        }

        private void anomalyReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int frameNumber = int.Parse(anomalyReports.SelectedItem.ToString().Split(" ")[0]);
            simPlayer.vm.skip(frameNumber - simPlayer.vm.VM_frameIndex);
        }
    }
}
