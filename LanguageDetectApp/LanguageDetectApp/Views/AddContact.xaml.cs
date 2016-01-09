using LanguageDetectApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.PersonalInformation;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddContact : Page, IFileOpenPickerContinuable
    {

        ContactModel _contactModel;

        #region Constuctor & OnNavigatedTo
        public AddContact()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _contactModel = e.Parameter as ContactModel;

            fullstackpanel.DataContext = _contactModel;

            _contactModel.PageCollapsed = true;
 
        }
        #endregion

        #region ImagePicker
        private void AddImageClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileopenpicker = new FileOpenPicker();
            fileopenpicker.ViewMode = PickerViewMode.Thumbnail;

            // Mở ở thư mục thư viện hình ảnh
            fileopenpicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            // Fill chỉ nhận file ảnh
            fileopenpicker.FileTypeFilter.Add(".jpg");
            fileopenpicker.FileTypeFilter.Add(".jpeg");
            fileopenpicker.FileTypeFilter.Add(".png");

            //// Pick One
            fileopenpicker.PickSingleFileAndContinue();
        }



        public async void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Any() == true)
            {
                _contactModel.Thumbnail = args.Files.First();
                thumbnail.Source = await Util.LoadImage(args.Files.First());
            }
        }
        #endregion

        #region Event Handle
        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            bool success = false;

            try
            {
                var contact = await _contactModel.ToStoredContact();
                await contact.SaveAsync();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            if (success == true)
            {
                MessageDialog messageDialog = new MessageDialog("Success");
                await messageDialog.ShowAsync();
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Fail");
                await messageDialog.ShowAsync();
            }
        }

        private void CollapseBtnClick(object sender, RoutedEventArgs e)
        {
            _contactModel.PageCollapsed = !_contactModel.PageCollapsed;
        }
        #endregion
    }
}
