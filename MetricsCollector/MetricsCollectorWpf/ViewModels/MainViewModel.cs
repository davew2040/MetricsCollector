using MetricsCollector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MetricsCollectorWpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _msBuildPath;
        public string MsBuildPath 
        { 
            get { return this._msBuildPath;  }
            set
            {
                _msBuildPath = value;
                OnPropertyChanged(nameof(MsBuildPath));
                OnPropertyChanged(nameof(CanCollectMetrics));
            }
        }

        private string _rootSearchDirectory;
        public string RootSearchDirectory
        {
            get { return this._rootSearchDirectory; }
            set
            {
                _rootSearchDirectory = value;
                OnPropertyChanged(nameof(RootSearchDirectory));
                OnPropertyChanged(nameof(CanCollectMetrics));
            }
        }

        private CollectionMethod _collectionMethod;
        public CollectionMethod CollectionMethod
        {
            get { return this._collectionMethod; }
            set
            {
                _collectionMethod = value;
                OnPropertyChanged(nameof(CollectionMethod));
            }
        }

        private string _toolOutput;
        public string ToolOutput
        {
            get { return this._toolOutput; }
            set
            {
                _toolOutput = value;
                OnPropertyChanged(nameof(ToolOutput));
            }
        }

        public bool CanCollectMetrics
        {
            get
            {
                return !string.IsNullOrEmpty(this.RootSearchDirectory) && !string.IsNullOrEmpty(this.MsBuildPath);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
 