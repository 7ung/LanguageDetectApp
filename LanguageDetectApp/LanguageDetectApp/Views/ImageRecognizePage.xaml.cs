using LanguageDetachApp.Common;
using LanguageDetectApp.Model;
using LanguageDetectApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
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

        #region Constructor & OnNavigate
        public ImageRecognizePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            // gán static resouce bên xaml để xài
            _imageViewModel = Resources["imageDataContext"] as ImageRecognizeViewModel;

            listpickerflyout.ItemsSource = Util.AvailableCountries.Values;
            
            _imageViewModel.Language = (OcrLanguage)Enum.Parse(
                typeof(OcrLanguage),
                LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.RecogLanguageKey).ToString());

            HardwareButtons.BackPressed += hardwareButtons_BackPressed;
        }

        private void hardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                if (_imageViewModel.CurrentState == eState.Crop)
                {
                    _imageViewModel.CurrentState = eState.Scale;
                    cropControl.End();
                    e.Handled = true;
                }
                else if (rootFrame.CanGoBack == true)
                {
                    rootFrame.GoBack();
                    e.Handled = true;
                }
                else
                {
                    Application.Current.Exit();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CharacterRecognizeModel.Clear();

            // quay về home không cho nó quay lại frame trước nữa
            Frame.BackStack.Clear();

            if (imageView.Source == null)
            {
                regcognizeBtn.IsEnabled = false;
                cropBtn.IsEnabled = false;
            }
            else
            {
                regcognizeBtn.IsEnabled = true;
                cropBtn.IsEnabled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _imageViewModel.CurrentState = eState.Scale;
        }

        // được gọi khi chọn được hình ảnh và chuyển lại vào page này
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Any() == true)
            {
                _imageViewModel.Path = args.Files.First().Path;
                imageView.Source = await Util.LoadImage(args.Files.First());

                // enable btn
                cropBtn.IsEnabled = true;
                regcognizeBtn.IsEnabled = true;
                previewImage.Visibility = Visibility.Collapsed;

                // set lại scale nhỏ nhất
                CaculateMinScale(true);
            }
        }
        #endregion

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

        private async void regcognizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_imageViewModel.Image == null)
            {
#if DEBUG
                Debug.WriteLine("Không có hình");
#endif
                return;
        }
            (sender as Button).IsEnabled = false;

            // crop hình
            CropImage();

            // ẩn
            _imageViewModel.CurrentState = eState.Scale;
            cropControl.End();

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
                cropControl.Start();
            }
            else
            {
                _imageViewModel.CurrentState = eState.Scale;
                cropControl.End();
            }
        }

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

        private void scrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            CaculateMinScale(true);
        }
    }
}
