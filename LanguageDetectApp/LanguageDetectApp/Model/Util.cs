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
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    public static class Util
    {
        #region Test Value
        public static Point FrenchPosition = new Point(48.851835, 2.304811);
        public static Point SpainPosition = new Point(40.272313, -5.017622);
        public static Point KoreaPosition = new Point(36.142783, 128.027796);
        public static Point JapanPosition = new Point(35.8887233, 137.188617);
        public static Point ChinaPosition = new Point(34.8649535, 90.6391346);
        #endregion
        public static string GGApiCheckCoord = "http://maps.googleapis.com/maps/api/geocode/json?latlng=latitude,longitude";

        public static string YandexTranslateQuery = "https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang={2}";

        private static string YandexAPIKey = "trnsl.1.1.20160106T143429Z.e255279ccf4f6c1f.2d1e4c6ed478b5dae506c464a2dd1be9a37b904c";


        public static Dictionary<string, OcrLanguage> AvailableCountries = new Dictionary<string, OcrLanguage>()
        {
            {"CN",OcrLanguage.ChineseTraditional},
            {"JP",OcrLanguage.Japanese},
            {"KR",OcrLanguage.Korean},
            {"ES",OcrLanguage.Spanish},
            {"FR",OcrLanguage.French},
            {"GB",OcrLanguage.English},
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
                var uri = string.Format(
                    Util.YandexTranslateQuery, Util.YandexAPIKey,
                    text, lang);

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

            if (flagstatus == 1)
            {
                MessageDialog dialog = new MessageDialog("Please check your network.\n" + msg, "Something wrong.");
                await dialog.ShowAsync();

            }
            else if (flagstatus == 2)
            {
                MessageDialog dialog = new MessageDialog(msg, "Error");
                await dialog.ShowAsync();

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

        public static Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>()
        {
            { "Albanian", "sq" },
            { "English", "en" },
            { "Arabic", "ar" },
            { "Armenian", "hy" },
            { "Azerbaijan", "az" },
            { "Afrikaans", "af" },
            { "Basque", "eu" },
            { "Belarusian", "be" },
            { "Bulgarian", "bg" },
            { "Bosnian", "bs" },
            { "Welsh", "cy" },
            { "Vietnamese", "vi" },
            { "Hungarian", "hu" },
            { "Haitian (Creole)", "ht" },
            { "Galician", "gl" },
            { "Dutch", "nl" },
            { "Greek", "el" },
            { "Georgian", "ka" },
            { "Danish", "da" },
            { "Yiddish", "he" },
            { "Indonesian", "id" },
            { "Irish", "ga" },
            { "Italian", "it" },
            { "Icelandic", "is" },
            { "Spanish", "es" },
            { "Kazakh", "kk" },
            { "Catalan", "ca" },
            { "Kyrgyz", "ky" },
            { "Chinese", "zh" },
            { "Korean", "ko" },
            { "Latin", "la" },
            { "Latvian", "lv" },
            { "Lithuanian", "lt" },
            { "Malagasy", "mg" },
            {" Malay", "ms" },
            {" Maltese", "mt" },
            {" Macedonian", "mk" },
            {" Mongolian", "mn" },
            {" German", "de" },
            {" Norwegian", "no" },
            {" Persian", "fa" },
            {" Polish", "pl" },
            {" Portuguese", "pt" },
            {" Romanian", "ro" },
            {" Russian", "ru" },
            {" Serbian", "sr" },
            {" Slovakian", "sk" },
            {" Slovenian", "sl" },
            {" Swahili", "sw" },
            {" Tajik", "tg" },
            {" Thai", "th" },
            {" Tagalog", "tl" },
            {" Tatar", "tt" },
            {" Turkish", "tr" },
            {" Uzbek", "uz" },
            {" Ukrainian", "uk" },
            {" Finish", "fi" },
            {" French", "fr" },
            {" Croatian", "hr" },
            {" Czech", "cs" },
            {" Swedish", "sv" },
            {" Estonian", "et" },
            {" Japanese", "ja" }
        };

        #region Pair Country Language
        // Map các nước với ngôn ngữ sử dụng
        public static readonly List<KeyValuePair<string, string>> CountryLanguagePair = new List<KeyValuePair<string, string>>()
        {
new KeyValuePair<string, string>("af","ZA"),
new KeyValuePair<string, string>( "am","ET"),
new KeyValuePair<string, string>( "ar","AE"),
new KeyValuePair<string, string>( "ar","BH"),
new KeyValuePair<string, string>( "ar","DZ"),
new KeyValuePair<string, string>( "ar","EG"),
new KeyValuePair<string, string>( "ar","IQ"),
new KeyValuePair<string, string>( "ar","JO"),
new KeyValuePair<string, string>( "ar","KW"),
new KeyValuePair<string, string>( "ar","LB"),
new KeyValuePair<string, string>( "ar","LY"),
new KeyValuePair<string, string>( "ar","MA"),
new KeyValuePair<string, string>( "arn","CL"),
new KeyValuePair<string, string>( "ar","OM"),
new KeyValuePair<string, string>( "ar","QA"),
new KeyValuePair<string, string>( "ar","SA"),
new KeyValuePair<string, string>( "ar","SY"),
new KeyValuePair<string, string>( "ar","TN"),
new KeyValuePair<string, string>( "ar","YE"),
new KeyValuePair<string, string>( "as","IN"),
new KeyValuePair<string, string>( "az-Cyrl","AZ"),
new KeyValuePair<string, string>( "az-Latn","AZ"),
new KeyValuePair<string, string>( "ba","RU"),
new KeyValuePair<string, string>( "be","BY"),
new KeyValuePair<string, string>( "bg","BG"),
new KeyValuePair<string, string>( "bn","BD"),
new KeyValuePair<string, string>( "bn","IN"),
new KeyValuePair<string, string>( "bo","CN"),
new KeyValuePair<string, string>( "br","FR"),
new KeyValuePair<string, string>( "bs-Cyrl","BA"),
new KeyValuePair<string, string>( "bs-Latn","BA"),
new KeyValuePair<string, string>( "ca","ES"),
new KeyValuePair<string, string>( "co","FR"),
new KeyValuePair<string, string>( "cs","CZ"),
new KeyValuePair<string, string>( "cy","GB"),
new KeyValuePair<string, string>( "da","DK"),
new KeyValuePair<string, string>( "de","AT"),
new KeyValuePair<string, string>( "de","CH"),
new KeyValuePair<string, string>( "de","DE"),
new KeyValuePair<string, string>( "de","LI"),
new KeyValuePair<string, string>( "de","LU"),
new KeyValuePair<string, string>( "dsb","DE"),
new KeyValuePair<string, string>( "dv","MV"),
new KeyValuePair<string, string>( "el","GR"),
new KeyValuePair<string, string>( "en","029"),
new KeyValuePair<string, string>( "en","AU"),
new KeyValuePair<string, string>( "en","BZ"),
new KeyValuePair<string, string>( "en","CA"),
new KeyValuePair<string, string>( "en","GB"),
new KeyValuePair<string, string>( "en","IE"),
new KeyValuePair<string, string>( "en","IN"),
new KeyValuePair<string, string>( "en","JM"),
new KeyValuePair<string, string>( "en","MY"),
new KeyValuePair<string, string>( "en","NZ"),
new KeyValuePair<string, string>( "en","PH"),
new KeyValuePair<string, string>( "en","SG"),
new KeyValuePair<string, string>( "en","TT"),
new KeyValuePair<string, string>( "en","US"),
new KeyValuePair<string, string>( "en","ZA"),
new KeyValuePair<string, string>( "en","ZW"),
new KeyValuePair<string, string>( "es","AR"),
new KeyValuePair<string, string>( "es","BO"),
new KeyValuePair<string, string>( "es","CL"),
new KeyValuePair<string, string>( "es","CO"),
new KeyValuePair<string, string>( "es","CR"),
new KeyValuePair<string, string>( "es","DO"),
new KeyValuePair<string, string>( "es","EC"),
new KeyValuePair<string, string>( "es","ES"),
new KeyValuePair<string, string>( "es","GT"),
new KeyValuePair<string, string>( "es","HN"),
new KeyValuePair<string, string>( "es","MX"),
new KeyValuePair<string, string>( "es","NI"),
new KeyValuePair<string, string>( "es","PA"),
new KeyValuePair<string, string>( "es","PE"),
new KeyValuePair<string, string>( "es","PR"),
new KeyValuePair<string, string>( "es","PY"),
new KeyValuePair<string, string>( "es","SV"),
new KeyValuePair<string, string>( "es","US"),
new KeyValuePair<string, string>( "es","UY"),
new KeyValuePair<string, string>( "es","VE"),
new KeyValuePair<string, string>( "et","EE"),
new KeyValuePair<string, string>( "eu","ES"),
new KeyValuePair<string, string>( "fa","IR"),
new KeyValuePair<string, string>( "fi","FI"),
new KeyValuePair<string, string>( "fil","PH"),
new KeyValuePair<string, string>( "fo","FO"),
new KeyValuePair<string, string>( "fr","BE"),
new KeyValuePair<string, string>( "fr","CA"),
new KeyValuePair<string, string>( "fr","CH"),
new KeyValuePair<string, string>( "fr","FR"),
new KeyValuePair<string, string>( "fr","LU"),
new KeyValuePair<string, string>( "fr","MC"),
new KeyValuePair<string, string>( "fy","NL"),
new KeyValuePair<string, string>( "ga","IE"),
new KeyValuePair<string, string>( "gd","GB"),
new KeyValuePair<string, string>( "gl","ES"),
new KeyValuePair<string, string>( "gsw","FR"),
new KeyValuePair<string, string>( "gu","IN"),
new KeyValuePair<string, string>( "ha-Latn","NG"),
new KeyValuePair<string, string>( "he","IL"),
new KeyValuePair<string, string>( "hi","IN"),
new KeyValuePair<string, string>( "hr","BA"),
new KeyValuePair<string, string>( "hr","HR"),
new KeyValuePair<string, string>( "hsb","DE"),
new KeyValuePair<string, string>( "hu","HU"),
new KeyValuePair<string, string>( "hy","AM"),
new KeyValuePair<string, string>( "id","ID"),
new KeyValuePair<string, string>( "ig","NG"),
new KeyValuePair<string, string>( "ii","CN"),
new KeyValuePair<string, string>( "is","IS"),
new KeyValuePair<string, string>( "it","CH"),
new KeyValuePair<string, string>( "it","IT"),
new KeyValuePair<string, string>( "iu-Cans","CA"),
new KeyValuePair<string, string>( "iu-Latn","CA"),
new KeyValuePair<string, string>( "ja","JP"),
new KeyValuePair<string, string>( "ka","GE"),
new KeyValuePair<string, string>( "kk","KZ"),
new KeyValuePair<string, string>( "kl","GL"),
new KeyValuePair<string, string>( "km","KH"),
new KeyValuePair<string, string>( "kn","IN"),
new KeyValuePair<string, string>( "kok","IN"),
new KeyValuePair<string, string>( "ko","KR"),
new KeyValuePair<string, string>( "ky","KG"),
new KeyValuePair<string, string>( "lb","LU"),
new KeyValuePair<string, string>( "lo","LA"),
new KeyValuePair<string, string>( "lt","LT"),
new KeyValuePair<string, string>( "lv","LV"),
new KeyValuePair<string, string>( "mi","NZ"),
new KeyValuePair<string, string>( "mk","MK"),
new KeyValuePair<string, string>( "ml","IN"),
new KeyValuePair<string, string>( "mn","MN"),
new KeyValuePair<string, string>( "mn-Mong","CN"),
new KeyValuePair<string, string>( "moh","CA"),
new KeyValuePair<string, string>( "mr","IN"),
new KeyValuePair<string, string>( "ms","BN"),
new KeyValuePair<string, string>( "ms","MY"),
new KeyValuePair<string, string>( "mt","MT"),
new KeyValuePair<string, string>( "nb","NO"),
new KeyValuePair<string, string>( "ne","NP"),
new KeyValuePair<string, string>( "nl","BE"),
new KeyValuePair<string, string>( "nl","NL"),
new KeyValuePair<string, string>( "nn","NO"),
new KeyValuePair<string, string>( "nso","ZA"),
new KeyValuePair<string, string>( "oc","FR"),
new KeyValuePair<string, string>( "or","IN"),
new KeyValuePair<string, string>( "pa","IN"),
new KeyValuePair<string, string>( "pl","PL"),
new KeyValuePair<string, string>( "prs","AF"),
new KeyValuePair<string, string>( "ps","AF"),
new KeyValuePair<string, string>( "pt","BR"),
new KeyValuePair<string, string>( "pt","PT"),
new KeyValuePair<string, string>( "qut","GT"),
new KeyValuePair<string, string>( "quz","BO"),
new KeyValuePair<string, string>( "quz","EC"),
new KeyValuePair<string, string>( "quz","PE"),
new KeyValuePair<string, string>( "rm","CH"),
new KeyValuePair<string, string>( "ro","RO"),
new KeyValuePair<string, string>( "ru","RU"),
new KeyValuePair<string, string>( "rw","RW"),
new KeyValuePair<string, string>( "sah","RU"),
new KeyValuePair<string, string>( "sa","IN"),
new KeyValuePair<string, string>( "se","FI"),
new KeyValuePair<string, string>( "se","NO"),
new KeyValuePair<string, string>( "se","SE"),
new KeyValuePair<string, string>( "si","LK"),
new KeyValuePair<string, string>( "sk","SK"),
new KeyValuePair<string, string>( "sl","SI"),
new KeyValuePair<string, string>( "sma","NO"),
new KeyValuePair<string, string>( "sma","SE"),
new KeyValuePair<string, string>( "smj","NO"),
new KeyValuePair<string, string>( "smj","SE"),
new KeyValuePair<string, string>( "smn","FI"),
new KeyValuePair<string, string>( "sms","FI"),
new KeyValuePair<string, string>( "sq","AL"),
new KeyValuePair<string, string>( "sr-Cyrl","BA"),
new KeyValuePair<string, string>( "sr-Cyrl","CS"),
new KeyValuePair<string, string>( "sr-Cyrl","ME"),
new KeyValuePair<string, string>( "sr-Cyrl","RS"),
new KeyValuePair<string, string>( "sr-Latn","BA"),
new KeyValuePair<string, string>( "sr-Latn","CS"),
new KeyValuePair<string, string>( "sr-Latn","ME"),
new KeyValuePair<string, string>( "sr-Latn","RS"),
new KeyValuePair<string, string>( "sv","FI"),
new KeyValuePair<string, string>( "sv","SE"),
new KeyValuePair<string, string>( "sw","KE"),
new KeyValuePair<string, string>( "syr","SY"),
new KeyValuePair<string, string>( "ta","IN"),
new KeyValuePair<string, string>( "te","IN"),
new KeyValuePair<string, string>( "tg-Cyrl","TJ"),
new KeyValuePair<string, string>( "th","TH"),
new KeyValuePair<string, string>( "tk","TM"),
new KeyValuePair<string, string>( "tn","ZA"),
new KeyValuePair<string, string>( "tr","TR"),
new KeyValuePair<string, string>( "tt","RU"),
new KeyValuePair<string, string>( "tzm-Latn","DZ"),
new KeyValuePair<string, string>( "ug","CN"),
new KeyValuePair<string, string>( "uk","UA"),
new KeyValuePair<string, string>( "ur","PK"),
new KeyValuePair<string, string>( "uz-Cyrl","UZ"),
new KeyValuePair<string, string>( "uz-Latn","UZ"),
new KeyValuePair<string, string>( "vi","VN"),
new KeyValuePair<string, string>( "wo","SN"),
new KeyValuePair<string, string>( "xh","ZA"),
new KeyValuePair<string, string>( "yo","NG"),
new KeyValuePair<string, string>( "zh","CN"),
new KeyValuePair<string, string>( "zh","HK"),
new KeyValuePair<string, string>( "zh","MO"),
new KeyValuePair<string, string>( "zh","SG"),
new KeyValuePair<string, string>( "zh","TW"),
new KeyValuePair<string, string>( "zu","ZA")
        };
        #endregion

        public static async Task<string> GetLocalCountryCode(Point geolocation)
        {
            string uri = Util.GGApiCheckCoord;
            uri = uri.Replace("latitude", geolocation.X.ToString());
            uri = uri.Replace("longitude", geolocation.Y.ToString());

            // Sử dụng http request để nhận thông tin từ google
            HttpWebRequest httprequest = System.Net.HttpWebRequest.Create(uri) as HttpWebRequest;
            httprequest.Credentials = CredentialCache.DefaultCredentials;
            httprequest.Method = "POST";
            httprequest.ContentType = "text/html";

            string countryreturn = "en";
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
                            countryreturn = item.First.First.First["short_name"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return countryreturn;

        }

        //public static void DrawRectangle(this WriteableBitmap bmp, Rect bound, Color color, int thickness)
        //{
        //    int top = (int) bound.Top;          //min y
        //    int bottom = (int) bound.Bottom;    //max y
        //    int left = (int) bound.Left;        //min x
        //    int right = (int) bound.Right;      //max x
        //    bmp.DrawLineAa(left, top, right, top, color, thickness);            // top
        //    bmp.DrawLineAa(left, bottom, right, bottom, color, thickness);      // bot
        //    bmp.DrawLineAa(left, top, left, bottom, color, thickness);          // left
        //    bmp.DrawLineAa(right, top, right, bottom, color, thickness);        // right
        //}
    }

}
