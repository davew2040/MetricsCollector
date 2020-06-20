using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MetricsCollector
{
    public class MetricsCollector
    {
        private const string CsProjExtension = "csproj";
        private const string MsBuildExe = "msbuild.exe";
        private const string RootOutputDirectory = "Output";
        private readonly string ProjectMetricsOutputDirectory = RootOutputDirectory + Path.DirectorySeparatorChar + "PerProject";

        public async Task Run(CollectionConfiguration config)
        {
            bool rootExists = Directory.Exists(config.RootDirectory);

            if (!rootExists)
            {
                throw new ArgumentException($"Could not locate directory: {config.RootDirectory}");
            }

            List<Task> metricsCollectionTasks = new List<Task>();

            this.Traverse(new DirectoryInfo(config.RootDirectory), config, metricsCollectionTasks);

            await Task.WhenAll(metricsCollectionTasks);
        }

        public void Traverse(DirectoryInfo currentDirectory, CollectionConfiguration config, List<Task> tasks)
        {
            foreach (string file in Directory.EnumerateFiles(currentDirectory.ToString()))
            {
                if (file.ToLower().EndsWith("." + CsProjExtension))
                {
                    tasks.Add(this.CollectMetricsAsync(file, config));
                }
            }

            foreach (var childDirectory in currentDirectory.EnumerateDirectories())
            {
                this.Traverse(childDirectory, config, tasks);
            }
        }

        private async Task CollectMetricsAsync(string projPath, CollectionConfiguration config)
        {
            var exe = Path.Combine(config.MsBuildPath, MsBuildExe);
            var args = $"/t:Metrics /p:MetricsOutputFile={this.GetProjectOutputFile(projPath, config)} {projPath}";

            // running from NuGet ex - msbuild /t:Metrics /p:MetricsOutputFile=<filename>
            ProcessStartInfo psi = new ProcessStartInfo(MsBuildExe);
            psi.Arguments = args;
            var p = Process.Start(exe, args);

            await p.WaitForExitAsync();  
        }

        private string GetProjectOutputFile(string fullPath, CollectionConfiguration config)
        {
            var smallerPath = fullPath.Substring(config.RootDirectory.Length); // Strip out the leading path, i.e. remove "C:\myProjects\" from "C:\myProjects\project1.csproj"
            string escapedPath = smallerPath
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(":", "_")
                .Replace(".", "_");

            return $"{ProjectMetricsOutputDirectory}{Path.DirectorySeparatorChar}{escapedPath}";
        }
    }
}
