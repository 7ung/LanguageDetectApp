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
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LanguageDetectApp.Views
{
    public sealed partial class CropControl : UserControl
    {
        // rect để tính
        Rect _cropRect;
        Point _startPoint;
        Point _endPoint;

        public Rect CropRectangle
        {
            get { return _cropRect; }
        }

        public Canvas DrawCanvas
        {
            get { return drawCanvas; }
        }

        public CropControl()
        {
            this.InitializeComponent();
        }

        public void Start()
        {
            _cropRect = new Rect();
        }

        public void End()
        {
            cropRect.Visibility = Visibility.Collapsed;

            //hide
            cropPoint1.Visibility = Visibility.Collapsed;
            cropPoint2.Visibility = Visibility.Collapsed;
        }
        
        private void drawCanvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //hide
            cropPoint1.Visibility = Visibility.Collapsed;
            cropPoint2.Visibility = Visibility.Collapsed;

            _startPoint = e.Position;

            // reset rect
            cropRect.Visibility = Visibility.Visible;
            Canvas.SetLeft(cropRect, _startPoint.X);
            Canvas.SetTop(cropRect, _startPoint.Y);
            cropRect.Width = 0;
            cropRect.Height = 0;
        }
        
        private void drawCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _endPoint = e.Position;

            UpdateRectangle();
            
            cropRect.Width = _cropRect.Width;
            cropRect.Height = _cropRect.Height;

            Canvas.SetLeft(cropRect, _cropRect.Left);
            Canvas.SetTop(cropRect, _cropRect.Top);
        }

        private void drawCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            cropPoint1.Visibility = Visibility.Visible;
            cropPoint2.Visibility = Visibility.Visible;
            
            UpdateCropPoint();
        }
        
        private void cropPoint_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var point = sender as Ellipse;
            if (point == null)
                return;

            if (point.Name == "cropPoint1")
            {
                _startPoint.X += e.Delta.Translation.X;
                _startPoint.Y += e.Delta.Translation.Y;
            }
            else if (point.Name == "cropPoint2")
            {
                _endPoint.X += e.Delta.Translation.X;
                _endPoint.Y += e.Delta.Translation.Y;
            }

            // cập nhật hcn để vẽ
            UpdateRectangle();
            
            // cập nhật lại start/end point
            _startPoint.X = _cropRect.X;
            _startPoint.Y = _cropRect.Y;

            _endPoint.X = _startPoint.X + _cropRect.Width;
            _endPoint.Y = _startPoint.Y + _cropRect.Height;

            // cập nhật nút kéo
            UpdateCropPoint();
            
            cropRect.Width = _cropRect.Width;
            cropRect.Height = _cropRect.Height;

            Canvas.SetLeft(cropRect, _cropRect.Left);
            Canvas.SetTop(cropRect, _cropRect.Top);
        }

        private void cropPoint_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {

        }

        private void UpdateRectangle()
        {
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
        }

        private void UpdateCropPoint()
        {
            Canvas.SetLeft(cropPoint1, _cropRect.Left - cropPoint1.Width / 2);
            Canvas.SetTop(cropPoint1, _cropRect.Top - cropPoint1.Height / 2);

            Canvas.SetLeft(cropPoint2, _cropRect.Left + _cropRect.Width - cropPoint2.Width / 2);
            Canvas.SetTop(cropPoint2, _cropRect.Top + _cropRect.Height - cropPoint2.Height / 2);
        }

        private void cropRect_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _cropRect.X += e.Delta.Translation.X;
            _cropRect.Y += e.Delta.Translation.Y;

            Canvas.SetLeft(cropRect, _cropRect.X);
            Canvas.SetTop(cropRect, _cropRect.Y);
            
            UpdateCropPoint();

            _startPoint.X = _cropRect.X;
            _startPoint.Y = _cropRect.Y;

            _endPoint.X = _startPoint.X + _cropRect.Width;
            _endPoint.Y = _startPoint.Y + _cropRect.Height;
        }
    }
}
