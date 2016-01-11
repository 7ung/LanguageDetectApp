using LanguageDetachApp.Common;
using LanguageDetectApp.Model;
using LanguageDetectApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.PersonalInformation;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace LanguageDetectApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    /// 
    /// 
    public sealed partial class App : Application
    {
        public ContinuationManager ContinuationManager { get; private set; }

        public static ContactStore ContactStore;

        public static RemoteIdHelper RemoteIdHelper;

        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            HardwareButtons.BackPressed +=HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                if (rootFrame.CanGoBack == true)
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

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await initRecognizeLanguage();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "scenarioFrame");
                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                //if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                //{
                //    throw new Exception("Failed to create initial page");
                //}

                if (!rootFrame.Navigate(typeof(ImageRecognizePage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            await initContactStore();


            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task initRecognizeLanguage()
        {
            // Kiểm tra key có tồn tại hay không.
            // Nếu có => return khôgn làm gì
            // Nếu không => Hỏi có cho dùng GPS không
            // ____________ Nếu khôgn cho thì lưu key với value là english
            // ____________ Nếu cho thì gọi CharacterRecognizeModel.InitLanguage(); => lưu key

            if (LocalSettingHelper.IsExistsLocalSettingKey(LocalSettingHelper.RecogLanguageKey) == false)
            {
                bool isAllowGPS = await AskForUseGPS();
                OcrLanguage language = OcrLanguage.English;
                if (isAllowGPS == true)
                {
                    language = await CharacterRecognizeModel.InitLanguage();
                }
                else
                {
                    language = OcrLanguage.English;
                }
                LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.RecogLanguageKey, (int)language);
            }

        }

        private async Task initContactStore()
        {
            // Khởi tạo Id để có thể generate thành Id của Contact trên Store khi sync 
            App.ContactStore = await Windows.Phone.PersonalInformation.ContactStore.CreateOrOpenAsync(
                ContactStoreSystemAccessMode.ReadWrite,
                ContactStoreApplicationAccessMode.LimitedReadOnly);

            App.RemoteIdHelper = new RemoteIdHelper();
            await RemoteIdHelper.SetRemoteIdGuid(App.ContactStore);
        }

        private async Task<bool> AskForUseGPS()
        {
            bool isAllowed = false;
            if (LocalSettingHelper.IsExistsLocalSettingKey(LocalSettingHelper.AllowGPSKey) == false)
            {
                // Nếu khôgn có key setting
                MessageDialog msgbox = new MessageDialog("Do you want to use GPS?");
                msgbox.Commands.Add(new UICommand("No") { Id = 0 });
                msgbox.Commands.Add(new UICommand("Yes") { Id = 1 });

               var result =  await msgbox.ShowAsync() as UICommand;
               int id = Convert.ToInt32(result.Id);
               switch (id)
               {
                   case 1:
                       isAllowed = true;
                      // localSettings.Values["AllowsGPS"] = true;
                       break;
                   default:
                       isAllowed = false;
                      // localSettings.Values["AllowsGPS"] = false;
                       break;
               }
               //localSettings.Values["AllowsGPS"] = isAllowed;
               LocalSettingHelper.SetLocalSettingKeyValue(LocalSettingHelper.AllowGPSKey, isAllowed);
            }
            else
            {
                //isAllowed = Convert.ToBoolean(localSettings.Values["AllowsGPS"]);
                isAllowed = Convert.ToBoolean(LocalSettingHelper.GetLocalSettingValue(LocalSettingHelper.AllowGPSKey));
            }
            return isAllowed;
        }


        
        protected async override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            ContinuationManager = new ContinuationManager();

            Frame rootFrame = CreateRootFrame();
            await RestoreStatusAsync(e.PreviousExecutionState);

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(ImageRecognizePage));
            }

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                //Frame scenarioFrame = MainPage.Current.FindName("ScenarioFrame") as Frame;
                if (rootFrame != null)
                {
                    // Call ContinuationManager to handle continuation activation
                    ContinuationManager.Continue(continuationEventArgs, rootFrame);
                }
            }

            Window.Current.Activate();

        }

        #region Stuff
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content, 
            // just ensure that the window is active 
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate 
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state. 
                    //Assume there is no state and continue 
                }
            }
        } 
 
        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content, 
            // just ensure that the window is active 
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page 
                rootFrame = new Frame();

                // Set the default language 
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
               
                // Place the frame in the current Window 
                Window.Current.Content = rootFrame;
            }
            return rootFrame; 
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        #endregion
    }
}