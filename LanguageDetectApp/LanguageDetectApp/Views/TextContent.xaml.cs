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
using LanguageDetectApp.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Phone.PersonalInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using LanguageDetectApp.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextContent : Page
    {
        TextContentViewModel _textContentVM;

        public TextContent()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _textContentVM = Resources["textContentSource"] as TextContentViewModel;
            initShareUI();
        }


        #region Share Data
        private void initShareUI()
        {
            DataTransferManager datatransfer = DataTransferManager.GetForCurrentView();
            datatransfer.DataRequested += DataTransfer_DataRequested;
            datatransfer.TargetApplicationChosen += DataTransfer_TargetApplicationChosen;
        }

        private void DataTransfer_TargetApplicationChosen(DataTransferManager sender, TargetApplicationChosenEventArgs args)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(args.ApplicationName);
            
#endif
        }

        private async void DataTransfer_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest datarequest = args.Request;
            datarequest.Data.Properties.Title = "Share from 7ung app";
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    datarequest.Data.SetText(textContent.Text);


                });

        }

        private async void ShareFacebookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        DataTransferManager.ShowShareUI();

                    });
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);       
#endif   
            }
        }
        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CharacterRecognizeModel.PairWords.Clear();
        }

        private void addContactClick(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("\\d+");
            Regex whitespace = new Regex("[ ()-.]");

            // format lại chuỗi: xoá các khoảng trắng các dấu
            string temp = whitespace.Replace(_textContentVM.Content, String.Empty);
            MatchCollection matches = regex.Matches(temp);

            List<string> listPhoneNumber = new List<string>();
            int countStr = matches.Count;
            for (int i = 0; i < countStr; i++)
			{
               listPhoneNumber.Add(matches[i].Value);
			}

            ContactModel contactmodel = new ContactModel();
            contactmodel.Mobilephone = getMobilePhone(listPhoneNumber);
            contactmodel.AlternateMobilePhone = getAlternateMobilePhone(listPhoneNumber);

            contactmodel.GivenName = getGivenName();

            contactmodel.Email = getEmail();

            Frame.Navigate(typeof(AddContact), contactmodel);
        }

        private string getEmail()
        {
            if (CharacterRecognizeModel.PairWords.Any() == false)
	        {
                return String.Empty;
	        }
            var listemail = CharacterRecognizeModel.PairWords.Where(
                word => word.Key.Contains('@')
                );
            if (listemail.Any())
            {
                return listemail.OrderByDescending(email => email.Key.Length).First().Key;

            }
            return String.Empty;
        }

        private string getGivenName()
        {
            if (CharacterRecognizeModel.PairWords.Any() == false)
            {
                return String.Empty;
            }
            var listmaxHeight = CharacterRecognizeModel.PairWords.OrderByDescending(
                word => word.Value.Height
                );
            return listmaxHeight.First().Key;
        }

        private string getMobilePhone(List<string> listPhoneNumber)
        {
            var temp = listPhoneNumber.OrderByDescending(item => item.Length);
            if (temp.Any())
            {
                return temp.First();
            }
            return String.Empty;
        }

        private string getAlternateMobilePhone(List<string> listPhoneNumber)
        {
            var temp = listPhoneNumber.OrderByDescending(item => item.Length);
            if (temp.Count() >= 2)
            {
                return temp.ElementAt(1);
            }
            return String.Empty;
        }

        private async void translateBtn_Click(object sender, RoutedEventArgs e)
        {
            translatePanel.Visibility = Visibility.Visible;

            await _textContentVM.TranslateContent();
        }

        private void closeTranslateBtn_Click(object sender, RoutedEventArgs e)
        {
            translatePanel.Visibility = Visibility.Collapsed;
            translateContent.Text = string.Empty;
            translateBtn.Focus(FocusState.Programmatic);
        }
    }
 

}
