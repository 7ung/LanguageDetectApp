using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    public class Util
    {
        #region Test Value
        public static Point FrenchPosition = new Point(48.851835, 2.304811);
        public static Point SpainPosition = new Point(40.272313, -5.017622);
        public static Point KoreaPosition = new Point(36.142783, 128.027796);
        public static Point JapanPosition = new Point(35.8887233, 137.188617);
        public static Point ChinaPosition = new Point(34.8649535, 90.6391346);
        #endregion
        public static string GGApiCheckCoord = "http://maps.googleapis.com/maps/api/geocode/json?latlng=latitude,longitude";

        public static Dictionary<string, OcrLanguage> AvailableCountries = new Dictionary<string, OcrLanguage>()
        {
            {"CN",OcrLanguage.ChineseTraditional},
            {"JP",OcrLanguage.Japanese},
            {"KR",OcrLanguage.Korean},
            {"ES",OcrLanguage.Spanish},
            {"FR",OcrLanguage.French},
        };

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
        
        /// <summary>
        /// Dịch chuỗi
        /// </summary>
        /// <param name="text">chuỗi cần dịch</param>
        /// <param name="lang">ngôn ngữ dịch (vd: en-vi)</param>
        /// <returns></returns>
        public static async Task<string> Translate(string text, string lang)
        {
            int flagstatus = 0;
            string msg = string.Empty;
            try
            {
                string yandexAPIKey = "trnsl.1.1.20160106T143429Z.e255279ccf4f6c1f.2d1e4c6ed478b5dae506c464a2dd1be9a37b904c";
                
                var uri = string.Format(
                                    "https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang={2}",
                                    yandexAPIKey, text, lang);

                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.Method = "POST";

                var response = await webRequest.GetResponseAsync();

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();

                    var json = JObject.Parse(responseFromServer);

                    var listResult = json["text"].ToList();
                    string result = string.Empty;

                    foreach (var item in listResult)
                    {
                        result += item.ToString() + " ";
                    }
                    
                    reader.Dispose();
                    stream.Dispose();
                    response.Dispose();


                    return result;
                }
            }
            catch (WebException ex)
            {
                flagstatus = 1;
                msg = ex.Message;
            }
            catch (Exception ex)
            {
                flagstatus = 2;
                msg = ex.Message;

            }

            if (flagstatus ==1)
            {
                MessageDialog dialog = new MessageDialog("Please check your network.\n" + msg, "Something wrong.");
                await dialog.ShowAsync();

                //return string.Empty;
            }
            else if (flagstatus == 2)
            {
                MessageDialog dialog = new MessageDialog(msg, "Error");
                await dialog.ShowAsync();

                //return string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Lấy toạ độ địa lý
        /// </summary>
        /// <returns>Cặp giá trị vĩ độ, kinh độ</returns>
        public static async Task<Point> GetGeo2Coordinates()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                var latitude = geoposition.Coordinate.Point.Position.Latitude;
                var longitude = geoposition.Coordinate.Point.Position.Longitude;
                return new Point(latitude, longitude);
            }
            catch (Exception)
            {
                return new Point(0.0, 0.0);
            }
        }
    }

}
