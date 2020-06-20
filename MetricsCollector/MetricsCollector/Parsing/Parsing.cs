using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetricsCollector.Parsing
{
    public static class Parsing
    {
        public static class Config
        {
            public static string WriteXml(CollectionConfiguration config)
            {
                var xmlConfig = new XElement("Config",
                    new XElement(nameof(config.CollectionMethod), config.CollectionMethod.ToString()),
                    new XElement(nameof(config.OutputFile), config.OutputFile),
                    new XElement(nameof(config.MsBuildPath), config.MsBuildPath),
                    new XElement(nameof(config.RootDirectory), config.RootDirectory)
                );

                return xmlConfig.ToString();
            }

            public static async Task SaveConfigToFile(CollectionConfiguration config, string path)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    var text = WriteXml(config);
                    await file.WriteLineAsync(text);
                }
            }

            public static async Task<CollectionConfiguration> LoadConfig(string path)
            {
                string text;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {
                    text = await reader.ReadToEndAsync();
                }

                var root = XElement.Parse(text);

                var config = new CollectionConfiguration();

                config.MsBuildPath = root.Element(nameof(config.MsBuildPath)).Value;
                config.OutputFile = root.Element(nameof(config.OutputFile)).Value;
                config.RootDirectory = root.Element(nameof(config.RootDirectory)).Value;

                string collectionStringValue = root.Element(nameof(config.CollectionMethod)).Value;

                config.CollectionMethod = (CollectionMethod)Enum.Parse(typeof(CollectionMethod), collectionStringValue);

                return config;
            }
        }
    
        public static class MetricsRecords
        {
            public static async Task<MetricsRow> ReadMetrics(string path)
            {
                string text;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {
                    text = await reader.ReadToEndAsync();
                }

                var root = XElement.Parse(text);

                var target = root.Element("Targets").Element("Target");

                var assemblyName = target.Attribute("Name").Value;

                var metricsElement = target.Element("Assembly").Element("Metrics");

                return new MetricsRow()
                {
                    ProjectName = assemblyName,
                    ClassCoupling = ReadMetricsValue(metricsElement, "ClassCoupling"),
                    CyclomaticComplexity = ReadMetricsValue(metricsElement, "CyclomaticComplexity"),
                    MaintainabilityIndex = ReadMetricsValue(metricsElement, "MaintainabilityIndex"),
                    DepthOfInheritance = ReadMetricsValue(metricsElement, "DepthOfInheritance"),
                    SourceLines = ReadMetricsValue(metricsElement, "SourceLines")
                };
            }

            private static int ReadMetricsValue(XElement metricsElement, string key)
            {
                var value = metricsElement.Elements("Metric").First(e => e.Attribute("Name").Value == key).Attribute("Value").Value;

                return int.Parse(value);
            }
        }
    }
}
