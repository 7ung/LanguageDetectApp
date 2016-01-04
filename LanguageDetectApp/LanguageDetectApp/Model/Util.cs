using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LanguageDetectApp.Model
{
    class Util
    {
        public static async Task<WriteableBitmap> LoadImage(StorageFile file)
        {
            ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();

            using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap writeablebitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                writeablebitmap.SetSource(imgStream);
                return writeablebitmap;
            } 
        }

    }

}
