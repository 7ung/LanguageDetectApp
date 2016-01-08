using LanguageDetectApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace LanguageDetectApp.ViewModels
{
    public class FileViewModel : ObservableCollection<FileModel>
    {
        public FileViewModel()
        {
            // test
            for (int i = 0; i < 10; i++)
            {
                this.Add(new FileModel("This is a file " + i));
            }
        }

        private StorageFolder _storageFolder;

        public StorageFolder SaveFolder
        {
            get { return _storageFolder; }
            set
            {
                _storageFolder = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SaveFolder"));
            }
        }

        public async Task GetStorageFolder()
        {
            var folder = KnownFolders.RemovableDevices;
            StorageFolder sdCard = await (await folder.GetFoldersAsync()).FirstOrDefault().GetFolderAsync("Documents");

            SaveFolder = await sdCard.CreateFolderAsync("VinhApp", CreationCollisionOption.OpenIfExists);
        }

        public async Task GetFiles()
        {
            var files = await SaveFolder.GetFilesAsync();
            if(files.Any())
            {
                foreach (var item in files)
                {
                    this.Add(new FileModel(item.Name));
                }
            }
        }
    }
}
