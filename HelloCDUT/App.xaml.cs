﻿using HelloCDUT.Helper;
using HelloCDUT.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.PushNotifications;
using Microsoft.WindowsAzure.Messaging;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Notifications;
using System.Threading.Tasks;
using HelloCDUT.Unity;
using HelloCDUT.Lifecycle;
using Windows.Globalization;
using Microsoft.Practices.ServiceLocation;
using HelloCDUT.Facades;

namespace HelloCDUT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException; 
        }

        

        #region Handle global unhandled exceptions
        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Application Unhandled Exception:\r\n" + e.Exception.Message).ShowAsync();
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private async void SynchronizationContext_UnhandledException(object sender, Helper.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Synchronization Context Unhandled Exception:\r\n" + e.Exception.Message, ResourceLoaderHelper.Instance.GetString("MessageDialog_Embarrassed"))
                .ShowAsync();
        }

        #endregion

       

        private async void InitNotificationAsync()
        {
            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                var hub = new NotificationHub("HelloCDUTNotificationHub", "Endpoint=sb://hellocdutnotification.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=hZCf14UTl3kxd2XZgfuL3nonCKF9TYEaN8AmkMg2RoQ=");
                var result = await hub.RegisterNativeAsync(channel.Uri);

                if (result.RegistrationId != null)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Registration successful : {result.RegistrationId}");
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"InitNotificationAsync : {e.Message}");
#endif
            }

        }

        #region System's Event

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //Frame rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            //if (rootFrame == null)
            //{
            //    // Create a Frame to act as the navigation context and navigate to the first page
            //    rootFrame = new Frame();

            //    rootFrame.NavigationFailed += OnNavigationFailed;
            //    rootFrame.Navigated += OnNavigated;

            //    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //    {
            //        //TODO: Load state from previously suspended application
            //    }

            //    // Place the frame in the current Window
            //    Window.Current.Content = rootFrame;
            //}


            var shell = await Initialize();

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                var facade = ServiceLocator.Current.GetInstance<INavigationFacade>();

                if (AppLaunchCounter.IsFirstLaunch())
                {
                    facade.NavigateToWelcomeView();
                }
                else
                {
                    facade.NavigateToSignInView();
                }
            }

            AppLaunchCounter.RegisterLaunch();

            RegisterExceptionHandlingSynchronizationContext();

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            InitNotificationAsync();

            Window.Current.Activate();
            //if (e.PrelaunchActivated == false)
            //{
            //    if (rootFrame.Content == null)
            //    {
            //        // When the navigation stack isn't restored navigate to the first page,
            //        // configuring the new page by passing required information as a navigation
            //        // parameter
            //        rootFrame.Navigate(typeof(SignInPage), e.Arguments);
            //    }
            //    // Ensure the current window is active
            //    Window.Current.Activate();
            //}
              
            // Refresh launch counter, needs to be done
            // after AppLaunchCounter.IsFirstLaunch() is being checked.
            
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if(rootFrame != null)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            var shell = Window.Current.Content as AppShell;

            if (shell == null)
            {
                shell = await Initialize();
            }

            Window.Current.Content = shell;

            RegisterExceptionHandlingSynchronizationContext();
        }
        #endregion


        /// <summary>
        /// Initialize the App launch.
        /// </summary>
        /// <returns>The AppShell of the app.</returns>
        private async Task<AppShell> Initialize()
        {
            AppShell shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                UnityBootstrapper.Init();
                UnityBootstrapper.ConfigureRegistries();

                await AppInitialization.DoInitializations();

                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();

                // Set the default language
                shell.Language = ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;
            }

            return shell;
        }
    }
}
