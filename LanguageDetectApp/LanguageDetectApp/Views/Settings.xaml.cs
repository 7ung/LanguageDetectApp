using LanguageDetectApp.Model;
using LanguageDetectApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            listpickerflyout1.ItemsSource = Util.AvailableCountries.Values;
            OcrLanguage language = (OcrLanguage)Enum.Parse(
                 typeof(OcrLanguage),
                 LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey).ToString());
            langRecog.Content = Util.AvailableCountries.Where(country => country.Value == language).First().Key;

            listpickerflyout2.ItemsSource = Util.SupportedLanguages.Keys;
            string lang = LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.LanguageTranslateTo).ToString();
            langtoTranslate.Content = Util.SupportedLanguages[lang];


            geoLocalSwitchToggle.IsOn = Convert.ToBoolean(
                LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.AllowGPSKey)
                );

            SdCardSwitchToggle.IsOn = Convert.ToBoolean(
                LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.SaveOnSdCard)
                );
        }

        private void ListPickerFlyOut1Picker(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var items = args.AddedItems;
            if (items.Any() == false)
	            return;
            OcrLanguage language = (OcrLanguage)Enum.Parse(
                 typeof(OcrLanguage),
                 items.First().ToString());
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.RecogLanguageKey, (int)language);

            string key = Util.AvailableCountries.Where(country => country.Value == language).First().Key;
            langRecog.Content = key;
        }

        private void ListPickerFlyOut2Picker(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var items = args.AddedItems;
            if (items.Any() == false)
                return;
            string lang = items.First().ToString();
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.LanguageTranslateTo, lang);
            langtoTranslate.Content = Util.SupportedLanguages[lang];
        }

        private void GeoLocateToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            bool allowGeoLocate = (sender as ToggleSwitch).IsOn;
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.AllowGPSKey, allowGeoLocate);
        }

        private void SaveSDCardSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            bool allowGeoLocate = (sender as ToggleSwitch).IsOn;
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.SaveOnSdCard, allowGeoLocate);

        }
    }
}
