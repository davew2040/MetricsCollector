using MetricsCollector;
using MetricsCollector.Parsing;
using MetricsCollectorWpf.ViewModels;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetricsCollectorWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ViewModel = new MainViewModel();

            this.DataContext = this.ViewModel;
        }

        public MainViewModel ViewModel { get; set; }

        private void BrowseMsBuild(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            if (folderBrowser.ShowDialog() == true)
            {
                this.ViewModel.MsBuildPath = folderBrowser.SelectedPath;
            }
        }

        private void BrowseRootSearchDirectory(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            if (folderBrowser.ShowDialog() == true)
            {
                this.ViewModel.RootSearchDirectory = folderBrowser.SelectedPath;
            }
        }

        private async void CollectMetricsAsync(object sender, RoutedEventArgs e)
        {
            CollectionConfiguration config = new CollectionConfiguration();

            config.RootDirectory = this.ViewModel.RootSearchDirectory;
            config.MsBuildPath = this.ViewModel.MsBuildPath;
            config.CollectionMethod = this.ViewModel.CollectionMethod;

            await Parsing.Config.SaveConfigToFile(config, Parsing.Config.DefaultConfigFilename);

            var collector = new MetricsCollector.MetricsCollector(statusUpdater: value =>
            {
                this.ViewModel.ToolOutput += value;
            });

            await collector.Run(config);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ToolOutput = "test1\ntest2";

            await this.LoadConfig();
        }

        private async Task LoadConfig()
        {
            var config = await Parsing.Config.LoadConfig(Parsing.Config.DefaultConfigFilename);

            if (config == null)
            {
                return;
            }

            this.ViewModel.RootSearchDirectory = config.RootDirectory;
            this.ViewModel.MsBuildPath = config.MsBuildPath;
            this.ViewModel.CollectionMethod = config.CollectionMethod;
        }
    }
}
