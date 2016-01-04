
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace LanguageDetectApp.Model
{
    class ImageBehavior
    {
        public static  WriteableBitmap GrayScale(WriteableBitmap writablebitmap)
        {
            return writablebitmap.Gray();
        }

    }
}
