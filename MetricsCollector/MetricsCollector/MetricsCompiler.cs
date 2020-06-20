using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsCollector
{
    public class MetricsCompiler
    {
        public static async Task<IEnumerable<MetricsRow>> CompileFromDirectory(string path)
        {
            List<Task<MetricsRow>> rowTasks = new List<Task<MetricsRow>>();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                var task = Parsing.Parsing.MetricsRecords.ReadMetrics(file);
                rowTasks.Add(task);
            }

            await Task.WhenAll(rowTasks);

            return rowTasks.Select(t => t.Result);
        }
    }
}
