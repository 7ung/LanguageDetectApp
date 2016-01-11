using LanguageDetectApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LanguageDetectApp.ViewModels
{
    public class StateToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((eState)value == eState.Crop)
            {
                if ((string)parameter == "CropElement")
                    return true;
                else
                    return false;
            }
            else if ((eState)value == eState.Scale)
            {
                if ((string)parameter == "ScaleElement")
                    return true;
                else
                    return false;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // để tạm
            return eState.Scale;
        }
    }

    public class StateToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((eState)value == eState.Crop)
            {
                if ((string)parameter == "CropElement")
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else if ((eState)value == eState.Scale)
            {
                if ((string)parameter == "ScaleElement")
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return eState.Scale;
        }
    }
}
