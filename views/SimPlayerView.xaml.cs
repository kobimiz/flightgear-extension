using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace flightgearExtension.viewModels
{
    /// <summary>
    /// Interaction logic for SimPlayerView.xaml
    /// </summary>
    public partial class SimPlayerView : UserControl
    {
        public viewModels.SimPlayerViewModel vm;

        public SimPlayerView()
        {
            InitializeComponent();

            vm = new SimPlayerViewModel(null);
            DataContext = vm;

            vm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "VM_frameIndex")
                {
                    try
                    {
                        if (vm.VM_Data != null && vm.isConnected() && vm.VM_frameIndex < vm.VM_Data.Length)
                            vm.sendCurrFrame();
                        else
                            vm.pause();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Simulation not running with the correct settings.");
                        vm.pause();
                    }
                }
            };
        }
        public void play(object sender, RoutedEventArgs e)
        {
            if (!vm.isConnected())
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.play();
        }
        public void pause(object sender, RoutedEventArgs e)
        {
            if (!vm.isConnected())
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.pause();
        }
        public void FiveSecFor(object sender, RoutedEventArgs e)
        {
            if (!vm.isConnected())
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.skip(Math.Min((int)(5 * vm.VM_FPS), vm.VM_Data.Length - vm.VM_frameIndex - 1));
        }

        public void FiveSecPrev(object sender, RoutedEventArgs e)
        {
            if (!vm.isConnected())
                MessageBox.Show("Please start the simulation first");
            else if (vm.VM_Data == null)
                MessageBox.Show("Please load a CSV first");
            else
                vm.skip(Math.Max((int)(-5 * vm.VM_FPS), -1 * vm.VM_frameIndex));
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
