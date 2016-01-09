using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            From = "English";
            To = "Vietnamese";
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

    }
}
