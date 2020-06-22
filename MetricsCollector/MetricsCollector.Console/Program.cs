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
            var config = await Parsing.Parsing.Config.LoadConfig(Parsing.Parsing.Config.DefaultConfigFilename);

            var collector = new MetricsCollector(statusUpdater: 
                update => System.Console.WriteLine(update)
            );
            await collector.Run(config);

            System.Console.WriteLine("Metrics collection complete.");
        }
    }
}
