using System;
using System.Collections.Generic;
using System.Text;

namespace MetricsCollector
{
    public enum CollectionMethod
    {
        AttachedNuget,
        ProvidedMetricsExe // See https://docs.microsoft.com/en-us/visualstudio/code-quality/how-to-generate-code-metrics-data?view=vs-2019#metricsexe
    }

    public class CollectionConfiguration
    {
        public string RootDirectory { get; set; }
        public string MsBuildPath { get; set; }
        public CollectionMethod CollectionMethod { get; set; }
    }
}
