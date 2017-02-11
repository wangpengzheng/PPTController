using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PPTController.Infrastructure;
using PPTController.ViewModel;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace PPTController
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        private ViewModelLocator ViewModelLocator
        {
            get { return (ViewModelLocator)Resources["ViewModelLocator"]; }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ViewModelLocator.NavigationService.ApplicationIsLoading = true;

            if (!Settings.Contains("DefaultIP"))
            {
                Settings.Add(settingsKey.DefaultIP.ToString(), "192.168.");
            }

            if (!Settings.Contains("ShakeByAction"))
            {
                Settings.Add(settingsKey.ShakeByAction.ToString(), true);
            }

            if (!Settings.Contains("FirstLaunch"))
            {
                Settings.Add(settingsKey.FirstLaunch.ToString(), true);
            }

            if (!Settings.Contains("HotSpotIP"))
            {
                Settings.Add(settingsKey.HotSpotIP.ToString(), "192.168.173.1");
            }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                PhoneApplicationService.Current.State.Clear();
            }
            else
            {
                ViewModelLocator.NavigationService.RecoveredFromTombstoning = true;
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Settings.Save();

            ViewModelLocator.Dispose();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            bool shouldExit;

            if (e.ExceptionObject is AppException)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => System.Windows.MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}", e.ExceptionObject.Message)));

                shouldExit = ((AppException)e.ExceptionObject).ShouldExit;
            }

            e.Handled = true;
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        public static IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;

        public enum settingsKey
        {
            DefaultIP,
            ShakeByAction,
            FirstLaunch,
            HotSpotIP
        }
    }
}