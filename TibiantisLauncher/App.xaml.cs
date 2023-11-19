using Serilog;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TibiantisLauncher.Clients;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILogger logger;
        internal static GameClient? GameClient { get; set; }
        internal static CamPlayer? CamPlayer { get; set; }

        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File("tibiantis-launcher.log")
                .CreateLogger();

            logger = Log.ForContext<App>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;

            try
            {
                LauncherValidator.ValidateLauncherNotRunning();
                ProfileManager.Instance.CreateProfilesDirectory();
                GameClientValidator.ValidateClientExistence();
                GameClientValidator.ValidateClientVersion();
                GameClientValidator.ValidateClientNotRunning();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(0x1);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show("Access denied while attempting to create profiles folder in client directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(0x5);
                return;
            }

            try
            {
                Current.MainWindow = new ProfileListWindow();
                if (GameClient != null)
                    GameClient.Exit += (sender, e) => Dispatcher.Invoke(Shutdown);
                Current.MainWindow.Show();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show("Tibiantis Launcher terminated unexpectedly. Please report this issue to k.standarski@gmail.com.\r\nPlease remember to attach tibiantis-launcher.log file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(0x1);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.CloseAndFlush();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e.Exception, "Application terminated unexpectedly");
            MessageBox.Show("Tibiantis Launcher terminated unexpectedly. Please report this issue to k.standarski@gmail.com.\r\nPlease remember to attach tibiantis-launcher.log file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(0x1);
        }
    }
}
