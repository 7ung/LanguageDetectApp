using LanguageDetectApp.Common;
using LanguageDetectApp.Model;
using LanguageDetectApp.ViewModels;
using LanguageDetectApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WindowsPreview.Media.Ocr;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace LanguageDetectApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;

        public const string FEATURE_NAME = "Demo Detach Image";
        
        private ImageRecognizeViewModel _imageViewModel;
        
        List<Scenario> scenarios = new List<Scenario>
        {
        };
        
        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }
        
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Current = this;
            
            // gán static resouce bên mainpage.xaml để xài
            _imageViewModel = Resources["imageDataContext"] as ImageRecognizeViewModel; 
        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Frame frame = Window.Current.Content as Frame;
            SuspensionManager.RegisterFrame(ScenarioFrame, "scenarioFrame");

            if (ScenarioFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the ScenarioList 
                if (!ScenarioFrame.Navigate(typeof(ScenarioList)))
                {
                    throw new Exception("Failed to create scenario list");
                }
            }

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
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

        // được gọi khi chọn được hình ảnh và chuyển lại vào page này
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Any() == true)
	        {
                imageView.Source = await Util.LoadImage(args.Files.First());
                
                // set lại scale nhỏ nhất
                CaculateMinScale(true);
            }
        }

        private async void WhatTheNumberButton_Click(object sender, RoutedEventArgs e)
        {
            // crop hình
            CropImage();

            // đọc hình
            await _imageViewModel.RecognizeImage();
            
            Frame.Navigate(typeof(TextContent));
        }
        
        private void crop_Click(object sender, RoutedEventArgs e)
        {
            // Crop hình
            CropImage();

            // Tính lại scale nhỏ nhất
            CaculateMinScale();
        }

        private void CropImage()
        {
            var cropRect = new Rect();
            cropRect.X = scrollViewer.HorizontalOffset / scrollViewer.ZoomFactor;
            cropRect.Y = scrollViewer.VerticalOffset / scrollViewer.ZoomFactor;
            cropRect.Width = scrollViewer.ActualWidth / scrollViewer.ZoomFactor;
            cropRect.Height = scrollViewer.ActualHeight / scrollViewer.ZoomFactor;
            
            // Crop hình
            _imageViewModel.CropImage(cropRect);
        }
        
        /// <summary>
        /// Không cho tấm hình scale quá nhỏ
        /// </summary>
        /// <param name="setScale">Có set scale lại không</param>
        private void CaculateMinScale(bool setScale = false)
        {
            var minImage = Math.Max(_imageViewModel.Image.PixelWidth, _imageViewModel.Image.PixelHeight);
            var minView = Math.Max(scrollViewer.ActualWidth, scrollViewer.ActualHeight);

            scrollViewer.MinZoomFactor = (float)minView / minImage;

            if(setScale)
                scrollViewer.ChangeView(0, 0, scrollViewer.MinZoomFactor);
        }
    }

}
