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
        private const string ResultsFilename = "Results.xml";
        private readonly string MetricsExePath = "included_files" + Path.DirectorySeparatorChar + "metrics" + Path.DirectorySeparatorChar + "metrics.exe";
        private readonly string ProjectMetricsOutputDirectory = RootOutputDirectory + Path.DirectorySeparatorChar + "PerProject";
        private readonly Action<string> statusUpdater;

        public MetricsCollector(Action<string> statusUpdater)
        {
            this.statusUpdater = statusUpdater;
        }

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

            this.statusUpdater("Completed reading all projects.");

            var metrics = await MetricsCompiler.CompileFromDirectory(this.GetPerProjectOutputPath());

            var resultsPath = this.GetResultsPath();
            CompletedMetricsHandler.WriteMetrics(metrics, resultsPath);
            this.statusUpdater($"Wrote results to {resultsPath}.");
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
            if (!Directory.Exists(this.GetPerProjectOutputPath()))
            {
                Directory.CreateDirectory(this.GetPerProjectOutputPath());
            }

            this.ClearExistingOutputFiles();

            if (config.CollectionMethod == CollectionMethod.ProjectNuGet)
            {
                await this.CollectMetricsMsBuildAsync(projPath, config);
            }
            else if (config.CollectionMethod == CollectionMethod.ProvidedMetricsExe)
            {
                await this.CollectMetricsWithIncludedMetricsExeAsync(projPath, config);
            }
            else
            {
                throw new ArgumentException($"Unrecognized CollectionMethod value: {config.CollectionMethod}");
            }
        }


        private async Task CollectMetricsWithIncludedMetricsExeAsync(string projPath, CollectionConfiguration config)
        {
            var exe = this.GetMetricsExePath();
            var args = $"/project:{projPath} /out:{this.GetPerProjectOutputFile(projPath, config)}";

            // running from NuGet ex - msbuild /t:Metrics /p:MetricsOutputFile=<filename>
            ProcessStartInfo psi = new ProcessStartInfo(exe);
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var p = new Process();
            p.StartInfo = psi;
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.Start();

            await p.WaitForExitAsync();

            this.statusUpdater($"Gathered metrics for {projPath}.");
        }

        private async Task CollectMetricsMsBuildAsync(string projPath, CollectionConfiguration config)
        {
            var exe = Path.Combine(config.MsBuildPath, MsBuildExe);
            var args = $"/t:Metrics /p:MetricsOutputFile={this.GetPerProjectOutputFile(projPath, config)} {projPath}";

            // running from NuGet ex - msbuild /t:Metrics /p:MetricsOutputFile=<filename>
            ProcessStartInfo psi = new ProcessStartInfo(MsBuildExe);
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var p = new Process();
            p.StartInfo = psi;
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.Start();

            await p.WaitForExitAsync();

            this.statusUpdater($"Gathered metrics for {projPath}.");
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"ERROR: {e.Data.ToString()}");
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data.ToString());
        }

        private void HandleCollectedMetrics(IEnumerable<MetricsRow> metrics)
        {
            CompletedMetricsHandler.WriteMetrics(metrics, this.GetPerProjectOutputPath());
        }

        private void ClearExistingOutputFiles()
        {
            var outputPath = this.GetPerProjectOutputPath();

            foreach (var file in Directory.GetFiles(outputPath))
            {
                if (file.EndsWith(".xml"))
                {
                    File.Delete(file);
                }
            }
        }

        private string GetExecutingPath()
        {
            var appDllLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var appLocation = appDllLocation.Directory;

            return appLocation.FullName;
        }

        private string GetResultsPath()
        {
            return Path.Combine(this.GetExecutingPath(), RootOutputDirectory, ResultsFilename);
        }

        private string GetPerProjectOutputPath()
        {
            return Path.Combine(this.GetExecutingPath(), ProjectMetricsOutputDirectory);
        }

        private string GetMetricsExePath()
        {
            return Path.Combine(this.GetExecutingPath(), MetricsExePath);
        }

        private string GetPerProjectOutputFile(string csProjPath, CollectionConfiguration config)
        {
            var smallerPath = csProjPath.Substring(config.RootDirectory.Length); // Strip out the leading path, i.e. remove "C:\myProjects\" from "C:\myProjects\project1.csproj"
            string escapedPath = smallerPath
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(":", "_")
                .Replace(".", "_");

            var fullOutputPath = Path.Combine(this.GetPerProjectOutputPath(), escapedPath + ".xml");

            return fullOutputPath;
        }
    }
}
