using LanguageDetectApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace LanguageDetectApp.ViewModels
{
    public class ImageRecognizeViewModel : INotifyPropertyChanged
    {
        #region Constructor
        public ImageRecognizeViewModel()
        {
            _ocrEngine = new OcrEngine(OcrLanguage.English);
            _imageModel = new ImageModel();
        }

        public ImageRecognizeViewModel(OcrLanguage language)
        {
            _ocrEngine = new OcrEngine(language);
            _imageModel = new ImageModel();
        }
        #endregion

        #region Private Attributes
        private ImageModel _imageModel;

        //ref : https://msdn.microsoft.com/en-us/library/windows/apps/windows.media.ocr.aspx
        private OcrEngine _ocrEngine;

        private eState _currentState = eState.Scale;
        #endregion

        public string Path
        {
            get { return _imageModel.Path; }
            set
            {
                _imageModel.Path = value;
                onPropertyChanged("Path");
            }
        }

        #region Property
        public eState CurrentState
        {
            get { return _currentState; }
            set
            {
                SetProperty<eState>(ref _currentState, value, "CurrentState");
            }
        }
        
        public WriteableBitmap Image
        {
            get { return _imageModel.Image; }
            set {
                if (_imageModel.Image != value)
                {
                _imageModel.Image = value;
                onPropertyChanged("Image");
            }

            }
        }

        public OcrLanguage Language
        {
            get { return _ocrEngine.Language; }
            set
            {
                if (_ocrEngine.Language != value)
                {
                _ocrEngine.Language = value;
                onPropertyChanged("Language");
            }

            }
        }
        #endregion

        #region Method
        public void CropImage(Rect rect)
        {
            Image = Image.Crop(rect);
        }

        public void StandardImageForRecognize()
        {
            int width = Image.PixelWidth;
            int height = Image.PixelHeight;

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
                Image = Image.Resize(
                                width,
                                height,
                                WriteableBitmapExtensions.Interpolation.Bilinear);
            }
        }

        public async Task RecognizeImage()
        {
            // Hình ảnh muốn lấy được text phải dạng gray scale
            WriteableBitmap temp = ImageBehavior.GrayScale(this.Image);
            this.StandardImageForRecognize();

            // Bắt đầu tính toán nhận diện chữ.
            // Pixel Width / Height phải trong khoảng 40 đến 2600
            OcrResult result = null;

            try
            {
                result = await _ocrEngine.RecognizeAsync( (uint)temp.PixelHeight,
                                                          (uint)temp.PixelWidth,
                                                          temp.PixelBuffer.ToArray());
            }
            catch (Exception msg)
            {
                Debug.WriteLine(msg);
                return;
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

                            Debug.WriteLine(word.Text);

                            // WriteableBitmap.DrawRectangle là phương thức mở rộng từ lớp WriteableBitmapExtension. của thư viện WriteableEx
                            this.Image.DrawRectangle(
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
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (Object.Equals(field, value) == true)
            {
                return false;
            }
            field = value;
            onPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
