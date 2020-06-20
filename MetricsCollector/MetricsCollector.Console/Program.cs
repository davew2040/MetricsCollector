using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsCollector.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CollectionConfiguration config = new CollectionConfiguration()
            {
                CollectionMethod = CollectionMethod.AttachedNuget,
                OutputFile = @"C:\temp\output.xml",
                MsBuildPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin",
                RootDirectory = @"C:\Dev\MetricsCollection\MetricsCollector\TestSolutions"
            };

            var collector = new MetricsCollector();
            await collector.Run(config);
        }
    }
}
