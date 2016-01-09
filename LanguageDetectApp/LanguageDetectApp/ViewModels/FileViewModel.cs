using LanguageDetectApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        }

        private StorageFolder _storageFolder;

        public StorageFolder SavedFolder
        {
            get { return _storageFolder; }
            set
            {
                _storageFolder = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SavedFolder"));
            }
        }

        public async Task GetStorageFolder()
        {
            var folder = KnownFolders.RemovableDevices;
            StorageFolder sdCard = await (await folder.GetFoldersAsync()).FirstOrDefault().GetFolderAsync("Documents");

            SavedFolder = await sdCard.CreateFolderAsync("VinhApp", CreationCollisionOption.OpenIfExists);
        }

        public async Task GetFiles()
        {
            try
            {

                var files = await SavedFolder.GetFilesAsync();
                if (files.Any())
                {
                    foreach (var item in files)
                    {
                        var buffer = await FileIO.ReadBufferAsync(item);
                        Encoding encoding = Encoding.UTF8;

                        var data = buffer.ToArray();

                        var content = encoding.GetString(data, 0, data.Length);

                        this.Add(new FileModel(item.Name, content));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task SaveFile(FileModel model)
        {
            try
            {
                var file = await SavedFolder.CreateFileAsync(model.Name + ".txt");
                await FileIO.WriteTextAsync(file, model.Content);

                this.Add(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
