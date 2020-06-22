using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MetricsCollector.Wpf.ViewModels
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
 