using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    public class CharacterRecognizeModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public CharacterRecognizeModel()
        {

        }
        // micro soft optimine character recognize engine
        //ref : https://msdn.microsoft.com/en-us/library/windows/apps/windows.media.ocr.aspx
        private static OcrEngine _ocrEngine;

        private static OcrLanguage _language;

        //
        private static ObservableCollection<KeyValuePair<string, Rect>> _pairWords
                                    = new ObservableCollection<KeyValuePair<string, Rect>>();

        public static ObservableCollection<KeyValuePair<string, Rect>> PairWords
        {
            get { return _pairWords; }
            set { _pairWords = value; 
            
            }
        }
        public static OcrLanguage Language
        {
            //get { return _language; }
            //set { _language = value; }

            // hard code
            get { return OcrLanguage.English; }
            set { _language = OcrLanguage.English; }
        }
        public static OcrEngine OcrEngine
        {
            get { return _ocrEngine; }
            set { _ocrEngine = value; }
        }

        public static void Clear()
        {
            _pairWords.Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        
    }
}
