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
                if (_storageFolder != value)
                {
                    _storageFolder = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SavedFolder"));                    
                }
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
                // Tùng: for each thì không cần check any
                //if (files.Any())
                {
                    foreach (var item in files)
                    {
                        var buffer = await FileIO.ReadBufferAsync(item);
                        Encoding encoding = Encoding.UTF8;

                        var data = buffer.ToArray();

                        var content = encoding.GetString(data, 0, data.Length);

                        this.Add(new FileModel(item, item.Name, content));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task Delete(FileModel model)
        {
            try
            {
                if (model.File == null)
                    return;
                await model.File.DeleteAsync(StorageDeleteOption.PermanentDelete);
                var file = this.Where(f => f.Name == model.Name);
                if (file.Any())
                {
                    var storagefile = file.First();
                    this.Remove(storagefile);
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
                if (model.Name.Contains(".txt") == false)
                {
                    model.Name += ".txt";
                }
                //var allfile = await _storageFolder.GetFilesAsync();
                //IEnumerable<StorageFile> foundfile = null;
                //if (allfile.Any())
                //    foundfile = allfile.Where(f => f.Name == model.Name);
                //if (foundfile.Any())
                //{
                //    await FileIO.WriteTextAsync(foundfile.First(), model.Content);
                //}
                //else
                //{                
                //    var file = await SavedFolder.CreateFileAsync(model.Name);
                //    await FileIO.WriteTextAsync(file, model.Content);
                //}
                if (model.File == null)
                {
                    var file = await SavedFolder.CreateFileAsync(model.Name);
                    model.File = file;
                    await FileIO.WriteTextAsync(file, model.Content);
                    this.Add(model);
                 
                }
                else
                {
                    await FileIO.WriteTextAsync(model.File, model.Content);
                    var file = this.Where(f => f.Name == model.Name);
                    if (file.Any())
                    {
                        var storagefile = file.First();
                        storagefile.Content = model.Content;
                        storagefile.File = model.File;
                    }
                }

                //if(!this.Contains(model))
                //{
                //    this.Add(model);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
