using LanguageDetectApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace LanguageDetectApp.ViewModels
{
    public class TextContentViewModel : INotifyPropertyChanged
    {
        public TextContentViewModel()
        {
            GetContent();

            TranslateLanguage = "en-vi";
            IsTranslating = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _content;
        private string _translatedContent;
        private string _translateLanguage;
        private bool _isTranslating;

        /// <summary>
        /// Ngôn ngữ dịch (vd: en-vi)
        /// </summary>
        public string TranslateLanguage
        {
            get { return _translateLanguage; }
            set
            {
                _translateLanguage = value;
                onPropertyChanged("TranslateLanguage");
            }
        }
        
        public string TranslatedContent
        {
            get { return _translatedContent; }
            set
            {
                _translatedContent = value;
                onPropertyChanged("TranslatedContent");
            }
        }
        
        public bool IsTranslating
        {
            get { return _isTranslating; }
            set
            {
                _isTranslating = value;
                onPropertyChanged("IsTranslating");
            }
        }
        
        public string Content
        {
            get {
                return _content; 
            }
            set
            {
                _content = value;
                onPropertyChanged("Content");
            }
        }
        
        public async Task TranslateContent()
        {
            IsTranslating = true;
            
            TranslatedContent = await Util.Translate(Content, TranslateLanguage);

            IsTranslating = false;
        }

        public void GetContent()
        {
            if (CharacterRecognizeModel.PairWords.Any())
            {
                // Warning
                Content = String.Join(" ", CharacterRecognizeModel.PairWords.ToList().Select(word => word.Key));

                if (Content == string.Empty)
                {
                    Content = "No Content Found.";
                }
            }
            else
            {
                Content = string.Empty;
            }

        }


        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
