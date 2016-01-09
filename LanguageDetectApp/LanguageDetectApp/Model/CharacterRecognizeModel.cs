using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Shapes;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    public class CharacterRecognizeModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Constructor
        public CharacterRecognizeModel()
        {

        }
        #endregion

        #region Private Attribute
        // micro soft optimine character recognize engine
        //ref : https://msdn.microsoft.com/en-us/library/windows/apps/windows.media.ocr.aspx
        private static OcrEngine _ocrEngine;

        private static OcrLanguage _language;

        //
        private static ObservableCollection<KeyValuePair<string, Rect>> _pairWords
                                    = new ObservableCollection<KeyValuePair<string, Rect>>();
        #endregion

        #region Property

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

        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        public static void Clear()
        {
            _pairWords.Clear();
        }

        public static async Task<OcrLanguage> InitLanguage()
        {
            // Chắc là cái này nên gọi trong app.launch
            //var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //if (localSettings.Values.ContainsKey("AllowsGPS") == false)
            //{
            //    MessageDialog msgbox = new MessageDialog("Mày có cho tao xài GPS không");
            //    msgbox.Commands.Add(new UICommand("No") { Id = 0 });
            //    msgbox.Commands.Add(new UICommand("Yes") { Id = 1 });

            //    var result = await msgbox.ShowAsync() as UICommand;
            //    int id = Convert.ToInt32(result.Id);
            //    switch (id)
            //    {
            //        case 1: case 0:
            //            localSettings.Values["AllowsGPS"] = id;
            //            break;
            //        default:
            //            localSettings.Values["AllowsGPS"] = 0;
            //            break;
            //    }
            //}

            //if (Convert.ToInt32(localSettings.Values["AllowsGPS"]) == 0)
            //    return OcrLanguage.English;
            // Lấy toạ độ địa lý theo vĩ độ kinh độ
            Point coordinates = await Util.GetGeo2Coordinates();

            #if TEST
            coordinates = Util.FrenchPosition;
            #endif
            // link request google api có dạng ....?=latitude,longitude
            // Replace hai từ khoá trên để xác định thông tin toạ độ
            string uri = Util.GGApiCheckCoord;
            uri = uri.Replace("latitude", coordinates.X.ToString());
            uri = uri.Replace("longitude", coordinates.Y.ToString());

            // Sử dụng http request để nhận thông tin từ google
            HttpWebRequest httprequest = System.Net.HttpWebRequest.Create(uri) as HttpWebRequest;
            httprequest.Credentials = CredentialCache.DefaultCredentials;
            httprequest.Method = "POST";
            httprequest.ContentType = "text/html";

            try
            {
                // Phân tích dữ liệu từ file Json.
                // Tìm quốc gia của toạ độ hiện tại
                // Tìm quốc gia đó trong list AvailableCountries
                // Nếu không tìm thấy return English
                using (var response = await httprequest.GetResponseAsync())
                {
                    var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var json = JObject.Parse(rawJson);  //Turns your raw string into a key value lookup
                    var children = json["results"].Children();
                    foreach (var item in children)
                    {
                        if (item["types"].First.ToString() == "country")
                        {
                            string country = item.First.First.First["short_name"].ToString();
                            OcrLanguage languaue;
                            if (Util.AvailableCountries.TryGetValue(country,out languaue))
                            {
                                return languaue;
                            }
                            else
                            {
                                return OcrLanguage.English;
                            }
                        }
                    }
                }
                return OcrLanguage.English;
            }
            catch (Exception ex)
            {
                return OcrLanguage.English;
            }

        }
    }
}
