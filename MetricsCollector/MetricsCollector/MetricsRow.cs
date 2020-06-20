using System;
using System.Collections.Generic;
using System.Text;

namespace MetricsCollector
{
    public class MetricsRow
    {
        public string ProjectName { get; set; }
        
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int SourceLines { get; set; }
        public int ExecutableLines { get; set; }
    }
}
