using LanguageDetectApp.Common;
using LanguageDetectApp.Model;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {

        public static MainPage Current;

        public const string FEATURE_NAME = "Demo Detach Image";
        ImageModel imageModel;
        OcrEngine ocrEngine;
                    

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
            imageModel = new ImageModel();

            imageView.DataContext = imageModel;

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
           // var dt = DateTime.Now.TimeOfDay.TotalMilliseconds;
            var language = await CharacterRecognizeModel.InitLanguage();
           // System.Diagnostics.Debug.WriteLine((DateTime.Now.TimeOfDay.TotalMilliseconds - dt).ToString());
         
            ocrEngine = new OcrEngine(language);
            Debug.WriteLine(language.ToString());
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
                imageModel.Image = await Util.LoadImage(args.Files.First());
                imageModel.File = args.Files.First();
	        }
        }

        private async void WhatTheNumberButton_Click(object sender, RoutedEventArgs e)
        {
            // Hình ảnh muốn lấy được text phải dạng gray scale
            WriteableBitmap temp = ImageBehavior.GrayScale(imageModel.Image);
            standardImageForRecog(ref temp);
            // Bắt đầu tính toán nhận diện chữ.
            // Pixel Width / Height phải trong khoảng 40 đến 2600
            OcrResult result = null;
            try
            {
                result = await ocrEngine.RecognizeAsync(
                    (uint)temp.PixelHeight,
                    (uint)temp.PixelWidth,
                    temp.PixelBuffer.ToArray());
            }
            catch (Exception msg)
            {
                Debug.WriteLine(msg);
                Frame.Navigate(typeof(TextContent));
            }
            try
            {
                if (result.Lines != null)
                    foreach (var item in result.Lines)
                    {
                        if (item.Words == null)
                            continue;
                        foreach (var word in item.Words)
                        {
                            if (word.Text == null)
                                continue;
                            Rect bound = new Rect()
                            {
                                X = word.Left,
                                Y = word.Top,
                                Width = word.Width,
                                Height = word.Height
                            };

                            CharacterRecognizeModel.PairWords.Add(
                                new KeyValuePair<string, Rect>(word.Text, bound));
#if DEBUG
                            Debug.WriteLine(word.Text);
#endif

                            // WriteableBitmap.DrawRectangle là phương thức mở rộng từ lớp WriteableBitmapExtension. của thư viện WriteableEx
                            imageModel.Image.DrawRectangle(
                                (int)bound.Left,
                                (int)bound.Top,
                                (int)bound.Right,
                                (int)bound.Bottom,
                                Windows.UI.Color.FromArgb(255, 110, 210, 255));
                        }
                    }
            }
            catch (Exception msg)
            {
                // Khi không nhận được ảnh thì quăng lỗi nên catch để tránh bị break
                Debug.WriteLine(msg);
            }

            Frame.Navigate(typeof(TextContent), imageModel);

        }

        private void standardImageForRecog(ref WriteableBitmap temp)
        {
            int width = temp.PixelWidth;
            int height = temp.PixelHeight;
            bool flag = false;

            if (width < 40)
            {
                width = 40;
                flag = (flag == false) ? true : flag;
            }
            if (width > 2600)
            {
                flag = (flag == false) ? true : flag;
            }
            if (height < 40)
            {
                width = 40;
                flag = (flag == false) ? true : flag;
            }
            if (height > 2600)
            {
                flag = (flag == false) ? true : flag;
            }

            if (flag == true)
            {
                temp = temp.Resize(
                    width, 
                    height, 
                    WriteableBitmapExtensions.Interpolation.Bilinear);
            }
        }


    }

}
