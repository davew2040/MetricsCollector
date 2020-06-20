using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsCollector.Console
{
    class Program
    {
        private const string ConfigPath = "config.xml";

        static async Task Main(string[] args)
        {
            var config = await Parsing.Parsing.Config.LoadConfig(ConfigPath);

            var collector = new MetricsCollector(statusUpdater: 
                update => System.Console.WriteLine(update)
            );
            await collector.Run(config);

            System.Console.WriteLine("Metrics collection complete.");
            System.Console.ReadKey();
        }
    }
}
