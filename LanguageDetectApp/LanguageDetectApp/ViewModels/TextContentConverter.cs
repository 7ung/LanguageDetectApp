using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;

namespace LanguageDetectApp.ViewModels
{
    public class TextContentConverter : IValueConverter
    {
        #region Private Attribute
        private static string _default = "No Content Found";
        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            #region Stuff
            var words = value as ObservableCollection<KeyValuePair<string, Rect>>;
            if (words == null)
            {
                return _default;
            }
            string temp = String.Empty;
            #endregion
            var listwords = words.ToList().Select(word => word.Key);
            if (listwords.Any())
	        {
                temp = String.Join(" ", words.ToList().Select(word => word.Key));
                return temp;		 
	        }
            return _default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new ObservableCollection<KeyValuePair<string, Rect>>();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}
