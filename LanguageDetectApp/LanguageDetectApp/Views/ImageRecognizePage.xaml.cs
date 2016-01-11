using LanguageDetectApp.Common;
using LanguageDetectApp.Model;
using LanguageDetectApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WindowsPreview.Media.Ocr;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageRecognizePage : Page, IFileOpenPickerContinuable
    {
        private ImageRecognizeViewModel _imageViewModel;

        //// rect để tính
        //Rect _cropRect;
        //// rect để vẽ
        //Rectangle _cropDrawnRect;

        //Point _startPoint;
        //Point _endPoint;

        public ImageRecognizePage()
        {
            this.InitializeComponent();

            // gán static resouce bên xaml để xài
            _imageViewModel = Resources["imageDataContext"] as ImageRecognizeViewModel;

            listpickerflyout.ItemsSource = Util.AvailableCountries.Values;
            
            _imageViewModel.Language = (OcrLanguage)Enum.Parse(
                typeof(OcrLanguage),
                LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey).ToString());
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _imageViewModel.CurrentState = eState.Scale;

            //if (_cropDrawnRect != null && drawCanvas.Children.Contains(_cropDrawnRect))
            //    drawCanvas.Children.Remove(_cropDrawnRect);
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
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

        private async void regcognizeBtn_Click(object sender, RoutedEventArgs e)
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

            var _cropRect = cropControl.CropRectangle;

            if (_imageViewModel.CurrentState == eState.Crop && _cropRect != null)
            {
                // vị trí của tấm hình trong scrollviewer
                var pos = imageView.TransformToVisual(scrollViewer).TransformPoint(new Point(0, 0));
                double width, height;

                // kt xem nó ở trong hay lớn ra ngoài
                if (pos.X > 0)
                {
                    width = (_cropRect.X - pos.X);
                }
                else
                {
                    width = (scrollViewer.HorizontalOffset + _cropRect.X);
                }

                if (pos.Y > 0)
                {
                    height = _cropRect.Y - pos.Y;
                }
                else
                {
                    height = (scrollViewer.VerticalOffset + _cropRect.Y);
                }

                cropRect.X = width / scrollViewer.ZoomFactor;
                cropRect.Y = height / scrollViewer.ZoomFactor;
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

        private void CaculateMinScale(bool setScale = false)
        {
            double ratio;

            if (_imageViewModel.Image.PixelWidth >= _imageViewModel.Image.PixelHeight)
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
        

        private void cropBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_imageViewModel.CurrentState == eState.Scale)
            {
                _imageViewModel.CurrentState = eState.Crop;

                //_cropRect = new Rect();
                //_cropDrawnRect = new Rectangle()
                //{
                //    Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 255, 255)),
                //    Width = _cropRect.Width,
                //    Height = _cropRect.Height,
                //    StrokeThickness = 1,
                //    Stroke = new SolidColorBrush(Windows.UI.Colors.White)
                //};

                //drawCanvas.Children.Add(_cropDrawnRect);

                cropControl.Start();
            }
            else
            {
                _imageViewModel.CurrentState = eState.Scale;

                //drawCanvas.Children.Remove(_cropDrawnRect);
                cropControl.End();
            }
        }

        //private void drawCanvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    if (e.Handled)
        //        return;

        //    _startPoint = e.Position;

        //    // reset rect
        //    Canvas.SetLeft(_cropDrawnRect, _startPoint.X);
        //    Canvas.SetTop(_cropDrawnRect, _startPoint.Y);
        //    _cropDrawnRect.Width = 0;
        //    _cropDrawnRect.Height = 0;
        //}

        //private void cropPoint_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        //{
        //    var point = sender as Ellipse;
            
        //    Canvas.SetLeft(cropPoint1, Canvas.GetLeft(cropPoint1) + e.Delta.Translation.X);
        //    Canvas.SetTop(cropPoint1, Canvas.GetTop(cropPoint1) + e.Delta.Translation.Y);

        //    Canvas.SetLeft(_cropDrawnRect, Canvas.GetLeft(_cropDrawnRect) + e.Delta.Translation.X);
        //    Canvas.SetTop(_cropDrawnRect, Canvas.GetTop(_cropDrawnRect) + e.Delta.Translation.Y);
        //}

        //private void cropPoint_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    e.Handled = true;
            
        //}

        //private void drawCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        //{
        //    if (e.Handled)
        //        return;

        //    _endPoint = e.Position;

        //    if (_startPoint.X > _endPoint.X && _startPoint.Y > _endPoint.Y)
        //    {
        //        _cropRect.X = _endPoint.X;
        //        _cropRect.Width = _startPoint.X - _endPoint.X;
        //        _cropRect.Y = _endPoint.Y;
        //        _cropRect.Height = _startPoint.Y - _endPoint.Y;
        //    }
        //    else if (_startPoint.X < _endPoint.X && _startPoint.Y < _endPoint.Y)
        //    {
        //        _cropRect.X = _startPoint.X;
        //        _cropRect.Width = _endPoint.X - _startPoint.X;
        //        _cropRect.Y = _startPoint.Y;
        //        _cropRect.Height = _endPoint.Y - _startPoint.Y;
        //    }
        //    else if (_startPoint.X > _endPoint.X && _startPoint.Y < _endPoint.Y)
        //    {
        //        _cropRect.X = _endPoint.X;
        //        _cropRect.Width = _startPoint.X - _endPoint.X;
        //        _cropRect.Y = _startPoint.Y;
        //        _cropRect.Height = _endPoint.Y - _startPoint.Y;
        //    }
        //    else if (_startPoint.X < _endPoint.X && _startPoint.Y > _endPoint.Y)
        //    {
        //        _cropRect.X = _startPoint.X;
        //        _cropRect.Width = _endPoint.X - _startPoint.X;
        //        _cropRect.Y = _endPoint.Y;
        //        _cropRect.Height = _startPoint.Y - _endPoint.Y;
        //    }

        //    _cropDrawnRect.Width = _cropRect.Width;
        //    _cropDrawnRect.Height = _cropRect.Height;

        //    Canvas.SetLeft(_cropDrawnRect, _cropRect.Left);
        //    Canvas.SetTop(_cropDrawnRect, _cropRect.Top);
        //}

        //private void drawCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    if (e.Handled)
        //        return;

        //    cropPoint1.Visibility = Visibility.Visible;
        //    Canvas.SetLeft(cropPoint1, _cropRect.Left - cropPoint1.Width / 2);
        //    Canvas.SetTop(cropPoint1, _cropRect.Top - cropPoint1.Height / 2);
        //}

        private void ListPickerFlyOutPicker(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var items = args.AddedItems;
            if (items.Any() == false)
                return;
            LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.RecogLanguageKey, (int)items.First());
            //settingbtn.Content = items.First().ToString();
            _imageViewModel.Language = (OcrLanguage)Enum.Parse(
                 typeof(OcrLanguage),
                 items.First().ToString());
        }
    }
}
