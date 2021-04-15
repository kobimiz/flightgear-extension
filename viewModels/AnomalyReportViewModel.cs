using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace flightgearExtension.viewModels
{
    public class AnomalyReportViewModel : ViewModel
    {
        private ObservableCollection<AnomalyReport> reports;
        public AnomalyReportViewModel() : base(null)
        {
            Reports = null;
        }


        public ObservableCollection<AnomalyReport> Reports {
            get => reports;
            set {
                reports = value;
                NotifyPropertyChanged("Reports");
            }
        }

        public int length
        {
            get => reports.Count;
            set
            {
                NotifyPropertyChanged("length");
            }
        }
    }
}
