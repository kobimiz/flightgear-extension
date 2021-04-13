using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace flightgearExtension.viewModels
{
    /// <summary>
    /// Interaction logic for dataDisplayView.xaml
    /// </summary>
    public partial class DataDisplayView : UserControl
    {
        public dataDisplayViewModel vm;
        public DataDisplayView()
        {
            InitializeComponent();
            vm = new dataDisplayViewModel(null);
            DataContext = vm;
        }
    }
}
