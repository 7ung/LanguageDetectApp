using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageDetectApp
{
    public class LocalSettingHelper
    {
        public static readonly string AllowGPSKey = "AllowGPS";
        public static readonly string RecogLanguageKey = "RecognizeLanguage";
        public static readonly string LanguageTranslateTo = "TranslateTo";
        public static readonly string SaveOnSdCard = "SaveOnSdCard";
        
        public static bool IsExistsLocalSettingKey(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values.ContainsKey(key);
        }

        public static void SetLocalSettingKeyValue(string key, object value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;
        }

        public static object GetLocalSettingValue(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values[key];
        }

    }
}
