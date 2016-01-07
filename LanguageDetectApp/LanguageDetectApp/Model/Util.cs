using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LanguageDetectApp.Model
{
    public class Util
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
        
        /// <summary>
        /// Dịch chuỗi
        /// </summary>
        /// <param name="text">chuỗi cần dịch</param>
        /// <param name="lang">ngôn ngữ dịch (vd: en-vi)</param>
        /// <returns></returns>
        public static async Task<string> Translate(string text, string lang)
        {
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
                MessageDialog dialog = new MessageDialog("Please check your network.\n" +  ex.Message, "Something wrong.");
                await dialog.ShowAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, "Error");
                await dialog.ShowAsync();

                return string.Empty;
            }
        }
    }

}
