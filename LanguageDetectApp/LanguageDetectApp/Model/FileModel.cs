using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LanguageDetectApp.Model
{
    public class FileModel : INotifyPropertyChanged
    {
        public FileModel()
        {
        }

        public FileModel(string name)
        {
            Name = name;
            Content = String.Empty;
            File = null;
        }

        public FileModel(StorageFile file, string name, string content)
        {
            Name = name;
            Content = content;
            File = file;
        }

        private string _content;
        private string _path;
        private string _name;

        private StorageFile _storageFile;

        public StorageFile File
        {
            get { return _storageFile; }
            set
            {
                if (_storageFile != value)
                {
                    _storageFile = value;
                    onPropertyChanged("File");
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    onPropertyChanged("Name");
                }
            }
        }


        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    onPropertyChanged("Content");                    
                }
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path  != value)
                {
                    _path = value;
                    onPropertyChanged("Path");             
                }

            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
