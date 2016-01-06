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
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LanguageDetectApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextContent : Page
    {
        CharacterRecognizeModel ocr = new CharacterRecognizeModel();

        public TextContent()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // CharacterRecognizeModel is static object 
            textContent.DataContext = ocr;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CharacterRecognizeModel.PairWords.Clear();
        }
        
        private async void translateBtn_Click(object sender, RoutedEventArgs e)
        {
            translatePanel.Visibility = Visibility.Visible;
            translateContent.Text = await Util.Translate(textContent.Text, "en-vi");
        }

        private void closeTranslateBtn_Click(object sender, RoutedEventArgs e)
        {
            translatePanel.Visibility = Visibility.Collapsed;
            translateContent.Text = string.Empty;

            translateBtn.Focus(FocusState.Programmatic);
        }
    }
}
