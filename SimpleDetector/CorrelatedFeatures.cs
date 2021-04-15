using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDetector
{
    public class CorrelatedFeature
    {
        public int feature1Index;
        public int feature2Index;
        public double pearson;
        public double maxDistance;
        public Tuple<double, double> linReg;

        public CorrelatedFeature(int feature1Index, int feature2Index, double pearson, double maxDistance, Tuple<double, double> linReg)
        {
            this.feature1Index = feature1Index;
            this.feature2Index = feature2Index;
            this.pearson = pearson;
            this.maxDistance = maxDistance;
            this.linReg = linReg;
        }
    }
}
