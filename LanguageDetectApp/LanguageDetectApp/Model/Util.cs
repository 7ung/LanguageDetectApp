using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    class Util
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


        public const uint MAX_ID_LENGHT = 2;
    }

}
