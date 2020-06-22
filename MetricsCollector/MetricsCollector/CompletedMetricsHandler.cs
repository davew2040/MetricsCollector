using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MetricsCollector
{
    public static class CompletedMetricsHandler
    {
        public static void WriteMetrics(IEnumerable<MetricsRow> metrics, string outputFile)
        {
            var xml = GetMetricsXml(metrics);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputFile))
            {
                file.WriteLine(xml);
            }
        }

        public static string GetMetricsXml(IEnumerable<MetricsRow> metrics)
        {
            var xmlOutput = new XElement("Output",
                new XElement("RunDetails",
                    new XElement("DateRun", DateTime.Now.ToString()),
                    new XElement("MetricsSetsCollected", metrics.Count()),
                    new XElement("Metrics",
                        new XElement("AvgMaintainabilityIndex", metrics.Average(m => m.MaintainabilityIndex)),
                        new XElement("AvgCyclomaticComplexity", metrics.Average(m => m.CyclomaticComplexity)),
                        new XElement("AvgClassCoupling", metrics.Average(m => m.ClassCoupling)),
                        new XElement("AvgDepthOfInheritance", metrics.Average(m => m.DepthOfInheritance)),
                        new XElement("SourceLines", metrics.Sum(m => m.SourceLines))
                    )
                ),
                new XElement("ResultSets",
                    GetAllResultSets(metrics)
                )
            );

            return xmlOutput.ToString();
        }

        private static IEnumerable<XElement> GetAllResultSets(IEnumerable<MetricsRow> metrics)
        {
            List<XElement> resultSets = new List<XElement>();

            foreach (var runResult in metrics)
            {
                var resultNode = new XElement("ResultSet",
                    new XElement("ProjectName", runResult.ProjectName),
                    new XElement("MaintainabilityIndex", runResult.MaintainabilityIndex),
                    new XElement("CyclomaticComplexity", runResult.CyclomaticComplexity),
                    new XElement("ClassCoupling", runResult.ClassCoupling),
                    new XElement("DepthOfInheritance", runResult.DepthOfInheritance),
                    new XElement("SourceLines", runResult.SourceLines)
                );

                resultSets.Add(resultNode);
            }

            return resultSets;
        }
    }
}
