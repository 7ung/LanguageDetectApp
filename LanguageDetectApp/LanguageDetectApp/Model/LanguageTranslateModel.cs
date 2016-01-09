using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.Model
{
    public class LanguageTranslateModel : ObservableCollection<String>
    {
        public LanguageTranslateModel()
        {
            foreach (var item in Util.SupportedLanguages)
            {
                this.Add(item.Key);
            }

            // default
            //From = "English";
            //To = "Vietnamese";
            //From = getRecognizeLanguage();
            //To = getLocalityLanguage();
        }

        private string _languageTranslate = string.Empty;
        private string _fromLanguage = string.Empty;
        private string _toLanguage = string.Empty;

        public string To
        {
            get { return _toLanguage; }
            set
            {
                if (value == null)
                    return;

                _toLanguage = value;
                if(Util.SupportedLanguages.Keys.Contains(value) && Util.SupportedLanguages.Keys.Contains(From))
                {
                    LanguageTranslate = Util.SupportedLanguages[From] + "-" + Util.SupportedLanguages[value];
                }
                if (value != String.Empty)
                    LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.LanguageTranslateTo, value);
                OnPropertyChanged(new PropertyChangedEventArgs("To"));
            }
        }

        public string From
        {
            get { return _fromLanguage; }
            set
            {
                if (value == null)
                    return;

                _fromLanguage = value;
                if (Util.SupportedLanguages.Keys.Contains(value) && Util.SupportedLanguages.Keys.Contains(To))
                {
                    LanguageTranslate = Util.SupportedLanguages[value] + "-" + Util.SupportedLanguages[To];
                }

                OnPropertyChanged(new PropertyChangedEventArgs("From"));
            }
        }


        public string LanguageTranslate
        {
            get { return _languageTranslate; }
            set
            {
                _languageTranslate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LanguageTranslate"));
            }
        }

        #region Init Language for Translate
        public static string GetRecognizeLanguage()
        {
            string languagert = "English";
            if (LocalSettingHelper.IsExistsLocalSettingKey(LocalSettingHelper.RecogLanguageKey))
            {
                // Ngôn ngữ được sử dụng để phân tích hình ảnh
                var ocrlanguage = (OcrLanguage)LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey);

                if (ocrlanguage == OcrLanguage.English)
                {
                    return "English";
                }
                // Lấy ra các key của các countries ví dụ es, jp
                var keycode = Util.AvailableCountries.Where(country => country.Value == ocrlanguage).Select(c => c.Key).First();

                var temp = Util.CountryLanguagePair.Where(country => country.Value == keycode);
                
                if (temp.Any())
                {
                    var list = temp.Select(lang => lang.Key);
                    foreach (var item in list)
                    {
                        if (Util.SupportedLanguages.Values.Contains(item))
                        {
                            languagert = Util.SupportedLanguages.Where(lang => lang.Value == item).Select(l => l.Key).First();
                            break;
                        }
                    }
                }

            }
            return languagert;
        }

        public static async Task<string> GetLocalityLanguage()
        {
            string langreturn = String.Empty;
            if (LocalSettingHelper.IsExistsLocalSettingKey(LocalSettingHelper.LanguageTranslateTo) == false)
            {
                bool isAllowGPS = false;

                isAllowGPS = Convert.ToBoolean(LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.AllowGPSKey));
                if (isAllowGPS == true)
                {
                    Point coordinates = await Util.GetGeo2Coordinates();
                    string local = await Util.GetLocalCountryCode(coordinates);
                    langreturn = getLanguageFromCountryCode(local);
                }
                else
                {
                    langreturn = "English";
                }
                
            }
            else
            {
                langreturn = Convert.ToString( LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.LanguageTranslateTo));
            }
            return langreturn;
        }

        private static string getLanguageFromCountryCode(string local)
        {
            var temp = Util.CountryLanguagePair.Where(country => country.Value == local);

            if (temp.Any())
            {
                var list = temp.Select(lang => lang.Key);
                foreach (var item in list)
                {
                    if (Util.SupportedLanguages.Values.Contains(item))
                    {
                        return Util.SupportedLanguages.Where(lang => lang.Value == item).Select(l => l.Key).First();
                    }
                }
            }
            return "English";
        }
        #endregion
    }
}
