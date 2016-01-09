using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LanguageDetectApp.Model
{
    public class ImageModel : INotifyPropertyChanged
    {
        #region Private Attributes
        private WriteableBitmap _image;
        private StorageFile _storageFile;
        #endregion

        #region Property
        public StorageFile File
        {
            get { return _storageFile; }
            set {
                if (_storageFile != value)
                {
                    _storageFile = value;
                    OnPropertyChanged("File");
                }
               }
        }
        public WriteableBitmap Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
        }
        #endregion

        private string _path;

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }
        
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
