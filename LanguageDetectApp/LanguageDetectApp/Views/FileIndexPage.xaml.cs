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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileIndexPage : Page
    {
        private FileViewModel _fileViewModel;
        private FileModel _saveFile;

        public FileIndexPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _fileViewModel = Resources["filesSource"] as FileViewModel;
            _saveFile = Resources["saveFileSource"] as FileModel;

            await _fileViewModel.GetStorageFolder();
            await _fileViewModel.GetFiles();

            var savefile = e.Parameter as FileModel;
            if (savefile != null)
            {
                _saveFile.Name = savefile.Name;
                _saveFile.Content = savefile.Content;
            }
            else
            {
                savePanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (savePanel.Visibility == Visibility.Visible)
            {
                await _fileViewModel.SaveFile(_saveFile);
                savePanel.Visibility = Visibility.Collapsed;
            }
            
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            savePanel.Visibility = Visibility.Collapsed;
            saveBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
        }
    }
}
