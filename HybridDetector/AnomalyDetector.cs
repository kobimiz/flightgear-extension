using flightgearExtension.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HybridDetector
{
    public interface AnomalyDetector
    {
        void learnFlight(CsvDocument learningModel);
        AnomalyReport[] detectAnomalies(CsvDocument analyze);
    }
}
