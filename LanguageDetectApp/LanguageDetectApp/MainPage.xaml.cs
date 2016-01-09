using LanguageDetectApp.Common;
using LanguageDetectApp.Model;
using LanguageDetectApp.ViewModels;
using LanguageDetectApp.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
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
    public enum eState
    {
        Crop,
        Scale
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;

        public const string FEATURE_NAME = "Demo Detach Image";
                    
        private ImageRecognizeViewModel _imageViewModel;

        // rect để tính
        Rect _cropRect;
        // rect để vẽ
        Rectangle _cropDrawnRect;
        
        Point _startPoint;
        Point _endPoint;

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
            listpickerflyout.ItemsSource = Util.AvailableCountries.Values;
            //int language = Convert.ToInt32(LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey));
            //settingbtn.Content = Enum.Parse (typeof(OcrLanguage),LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey).ToString());
            _imageViewModel.Language = (OcrLanguage) Enum.Parse(
                typeof(OcrLanguage),
                LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey).ToString());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           // var dt = DateTime.Now.TimeOfDay.TotalMilliseconds;
          //  var language = await CharacterRecognizeModel.InitLanguage();
           // System.Diagnostics.Debug.WriteLine((DateTime.Now.TimeOfDay.TotalMilliseconds - dt).ToString());
         
         //   ocrEngine = new OcrEngine(language);
        //    Debug.WriteLine(language.ToString());
        //    Frame frame = Window.Current.Content as Frame;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _imageViewModel.CurrentState = eState.Scale;
            createCropRect.Opacity = 1.0;
            
            if (_cropDrawnRect != null && drawCanvas.Children.Contains(_cropDrawnRect))
                drawCanvas.Children.Remove(_cropDrawnRect);
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
                _imageViewModel.Path = args.Files.First().Path;
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
            
            Frame.Navigate(typeof(TextContent), _imageViewModel);
        }
        
        private void CropImage()
            {
            var cropRect = new Rect();

            if (_imageViewModel.CurrentState == eState.Crop && _cropRect != null)
            {
                cropRect.X = (scrollViewer.HorizontalOffset + _cropRect.X) / scrollViewer.ZoomFactor;
                cropRect.Y = (scrollViewer.VerticalOffset + _cropRect.Y) / scrollViewer.ZoomFactor;
                cropRect.Width = _cropRect.Width / scrollViewer.ZoomFactor;
                cropRect.Height = _cropRect.Height / scrollViewer.ZoomFactor;

                // Crop hình
                _imageViewModel.CropImage(cropRect);
            }
            else
            {
                cropRect.X = scrollViewer.HorizontalOffset / scrollViewer.ZoomFactor;
                cropRect.Y = scrollViewer.VerticalOffset / scrollViewer.ZoomFactor;
                cropRect.Width = scrollViewer.ActualWidth / scrollViewer.ZoomFactor;
                cropRect.Height = scrollViewer.ActualHeight / scrollViewer.ZoomFactor;

                // Crop hình
                _imageViewModel.CropImage(cropRect);
            }
            
            // Tính lại scale nhỏ nhất
            CaculateMinScale(true);
        }
        
        /// <summary>
        /// Không cho tấm hình scale quá nhỏ
        /// </summary>
        /// <param name="setScale">Có set scale lại không</param>
        private void CaculateMinScale(bool setScale = false)
            {
            double ratio;

            if(_imageViewModel.Image.PixelWidth > _imageViewModel.Image.PixelHeight)
            {
                ratio = scrollViewer.ActualWidth / _imageViewModel.Image.PixelWidth;
            }
            else
            {
                ratio = scrollViewer.ActualHeight / _imageViewModel.Image.PixelHeight;
            }
            
            scrollViewer.MinZoomFactor = (float)ratio;

            if (setScale)
                scrollViewer.ChangeView(0, 0, scrollViewer.MinZoomFactor);
        }
        
        private void createCropRect_Click(object sender, RoutedEventArgs e)
                    {
            if (_imageViewModel.CurrentState == eState.Scale)
                        {
                _imageViewModel.CurrentState = eState.Crop;
                createCropRect.Opacity = 0.5;

                _cropRect = new Rect();
                _cropDrawnRect = new Rectangle()
                            {
                    Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 255, 255)),
                    Width = _cropRect.Width,
                    Height = _cropRect.Height
                            };

                drawCanvas.Children.Add(_cropDrawnRect);
            }
            else
            {
                _imageViewModel.CurrentState = eState.Scale;
                createCropRect.Opacity = 1.0;

                drawCanvas.Children.Remove(_cropDrawnRect);
            }
            }

        private void drawCanvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _startPoint = e.Position;

            // reset rect
            Canvas.SetLeft(_cropDrawnRect, _startPoint.X);
            Canvas.SetTop(_cropDrawnRect, _startPoint.Y);
            _cropDrawnRect.Width = 0;
            _cropDrawnRect.Height = 0;
        }

        private void drawCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _endPoint = e.Position;

            if (_startPoint.X > _endPoint.X && _startPoint.Y > _endPoint.Y)
            {
                _cropRect.X = _endPoint.X;
                _cropRect.Width = _startPoint.X - _endPoint.X;
                _cropRect.Y = _endPoint.Y;
                _cropRect.Height = _startPoint.Y - _endPoint.Y;
            }
            else if (_startPoint.X < _endPoint.X && _startPoint.Y < _endPoint.Y)
            {
                _cropRect.X = _startPoint.X;
                _cropRect.Width = _endPoint.X - _startPoint.X;
                _cropRect.Y = _startPoint.Y;
                _cropRect.Height = _endPoint.Y - _startPoint.Y;
            }
            else if (_startPoint.X > _endPoint.X && _startPoint.Y < _endPoint.Y)
            {
                _cropRect.X = _endPoint.X;
                _cropRect.Width = _startPoint.X - _endPoint.X;
                _cropRect.Y = _startPoint.Y;
                _cropRect.Height = _endPoint.Y - _startPoint.Y;
            }
            else if (_startPoint.X < _endPoint.X && _startPoint.Y > _endPoint.Y)
            {
                _cropRect.X = _startPoint.X;
                _cropRect.Width = _endPoint.X - _startPoint.X;
                _cropRect.Y = _endPoint.Y;
                _cropRect.Height = _startPoint.Y - _endPoint.Y;
            }

            _cropDrawnRect.Width = _cropRect.Width;
            _cropDrawnRect.Height = _cropRect.Height;

            Canvas.SetLeft(_cropDrawnRect, _cropRect.Left);
            Canvas.SetTop(_cropDrawnRect, _cropRect.Top);
        }

        private void drawCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ListPickerFlyOutPicker(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var items = args.AddedItems;
            if (items.Any() == false)
	        	 return;
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.RecogLanguageKey, (int)items.First());
            //settingbtn.Content = items.First().ToString();
            _imageViewModel.Language =(OcrLanguage)Enum.Parse(
                 typeof(OcrLanguage),
                 items.First().ToString());
        }

    }

}
