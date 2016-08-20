using LanguageDetachApp.Common;
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
        private NavigationHelper _navigationhelper;

        #region Constructor & OnNavigated
        public FileIndexPage()
        {
            this.InitializeComponent();

            _navigationhelper = new NavigationHelper(this);
            _navigationhelper.LoadState += Navigationhelper_LoadState;
            _navigationhelper.SaveState += Navigationhelper_SaveState;
        }

        private void Navigationhelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void Navigationhelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationhelper.OnNavigatedTo(e);

            _fileViewModel = Resources["filesSource"] as FileViewModel;
            _saveFile = Resources["saveFileSource"] as FileModel;

            _fileViewModel.Clear();
            var task1 = _fileViewModel.GetStorageFolder();
            var task2 = _fileViewModel.GetFiles();
            await Task.WhenAll(task1, task2);
            
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
            if (nameText.IsReadOnly == true)
            {
                nameText.IsReadOnly = false;                
            }

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationhelper.OnNavigatedFrom(e);
            //CharacterRecognizeModel.PairWords.Clear();
        }
        #endregion

        #region User Event Handled
        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (savePanel.Visibility == Visibility.Visible)
            {
                await _fileViewModel.SaveFile(_saveFile);
                savePanel.Visibility = Visibility.Collapsed;

                saveBtn.IsEnabled = false;
                cancelBtn.IsEnabled = false;
            }
            
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear frame trước khi chuyển tới home
            Frame.BackStack.Clear();
            Frame.Navigate(typeof(ImageRecognizePage));
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            savePanel.Visibility = Visibility.Collapsed;
            saveBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var panel = sender as StackPanel;
            if (panel != null)
            {
                var model = panel.DataContext as FileModel;
                if (model != null)
                {
                    _saveFile.Name = model.Name;
                    _saveFile.Content = model.Content;
                    _saveFile.File = model.File;

                    nameText.IsReadOnly = true;

                    savePanel.Visibility = Visibility.Visible;
                    saveBtn.IsEnabled = true;
                    cancelBtn.IsEnabled = true;
                }
            }
        }
        #endregion

        private async void StackPannel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var panel = sender as StackPanel;
            if (panel != null)
            {
                var model = panel.DataContext as FileModel;
                if (model != null)
                {
                    await _fileViewModel.Delete(model);
                }
            }
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }
    }
}
