using System;
using System.Collections.Generic;
using System.Text;

namespace flightgearExtension.classes
{
    public class AnomalyReport
    {
        public int feature1Index;
        public int feature2Index;
        public int frameIndex;

        public AnomalyReport(int feature1Index, int feature2Index, int frameIndex)
        {
            this.feature1Index = feature1Index;
            this.feature2Index = feature2Index;
            this.frameIndex = frameIndex;
        }

        public override string ToString()
        {
            return frameIndex.ToString() + " " + feature1Index.ToString() + " " + feature2Index.ToString();
        }
    }
}
