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
        List<string> _listPhoneNumber;
        ContactModel _contactModel;

        Windows.Phone.PersonalInformation.ContactStore _contactStore;
        StoredContact _contact;
        RemoteIdHelper _remoteIDHelper;

        public AddContact()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _contactModel = e.Parameter as ContactModel;

            txtboxName.DataContext = _contactModel;
            txtboxFamilyName.DataContext = _contactModel;
            txtboxNickName.DataContext = _contactModel;
            txtBlockMobile.DataContext = _contactModel;
            txtBlockAltMobile.DataContext = _contactModel;
            dtPickerBirthDay.DataContext = _contactModel;
            txtboxAddress.DataContext = _contactModel;
            txtboxPhone.DataContext = _contactModel;
            txtboxStreet.DataContext = _contactModel;
            txtboxRegion.DataContext = _contactModel;
            txtboxCountry.DataContext = _contactModel;
            thumbnail.DataContext = _contactModel;
            txtBlockEmail.DataContext = _contactModel;
            _contactModel.PageCollapsed = true;
 
        }

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

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            #region Demo
            if (false)
            {
                // remote id là id của contact khi sync lên store
                _contact.RemoteId = await _remoteIDHelper.GetTaggedRemoteId(_contactStore, "675");
                _contact.GivenName = "Tung";
                _contact.FamilyName = "Ho";

                IDictionary<string, object> props = await _contact.GetPropertiesAsync();


                //ContactPhone contactphone = new ContactPhone()
                //{
                //    Kind = ContactPhoneKind.Home,
                //    Description = "This is sample description",
                //    Number = "75728"
                //};

                props.Add(KnownContactProperties.AlternateMobileTelephone, "75728");
                props.Add(KnownContactProperties.Nickname, "7ung");

                var address = new Windows.Phone.PersonalInformation.ContactAddress()
                {
                    Country = "Việt Nam",
                    Locality = "Thủ Đúc",
                    PostalCode = "70000",
                    StreetAddress = "Xa lộ Hà Nội"
                };

                string addressString = address.ToString();
                props.Add(KnownContactProperties.Address, address);

                props.Add(KnownContactProperties.MobileTelephone, "8908"); // cái này mới có text sms
                // Add thời gian vào không dùng được DateTime mà phải dùng DateTimeOffset
                // DateTimeOffset có một độ lệch với DateTime nến đặt offset bằng độ lệch là TimeSpan(1,0,0) (1 hour 0 munite 0 second)
                props.Add(KnownContactProperties.Birthdate, new DateTimeOffset(new DateTime(1995, 11, 26), new TimeSpan(1, 0, 0)));

                await _contact.SaveAsync();
            
            }

            #endregion
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
    }
}
