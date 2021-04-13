using flightgearExtension.viewModels;
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
    public partial class menuView : UserControl
    {
        public menuViewModel vm;

        public menuView()
        {
            InitializeComponent();
            vm = new menuViewModel(null);
        }

        private void launchFG(object sender, RoutedEventArgs e)
        {
            vm.launchFG(sender);
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            vm.openSettings();
        }

        public void startSim(object sender, RoutedEventArgs e)
        {
            vm.startSim();
        }
    }
}
