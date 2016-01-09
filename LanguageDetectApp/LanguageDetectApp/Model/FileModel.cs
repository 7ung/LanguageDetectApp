using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageDetectApp.Model
{
    public class FileModel : INotifyPropertyChanged
    {
        public FileModel()
        { }

        public FileModel(string name)
        {
            Name = name;
        }

        public FileModel(string name, string content)
        {
            Name = name;
            Content = content;
        }

        private string _content;
        private string _path;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                onPropertyChanged("Name");
            }
        }


        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                onPropertyChanged("Content");
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                onPropertyChanged("Path");
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
