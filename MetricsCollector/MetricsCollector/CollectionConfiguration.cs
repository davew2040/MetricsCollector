using System;
using System.Collections.Generic;
using System.Text;

namespace MetricsCollector
{
    public enum CollectionMethod
    {
        AttachedNuget,
        ProvidedMetricsExe
    }

    public class CollectionConfiguration
    {
        public string RootDirectory { get; set; }
        public string OutputFile { get; set; }
        public CollectionMethod CollectionMethod { get; set; }
    }
}
