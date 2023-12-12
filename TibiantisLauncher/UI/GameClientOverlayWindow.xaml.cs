using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using TibiantisLauncher.Clients;

namespace TibiantisLauncher
{
    public partial class GameClientOverlayWindow : Window
    {
        private DispatcherTimer _playerStatsTimer;
        private DispatcherTimer _windowTimer;
        private ExperienceCalculator? _experienceCalculator;
        private PingMeter _pingMeter;

        public GameClientOverlayWindow()
        {
            InitializeComponent();
            VersionLabel.Content = $"v{App.Version}";
            _pingMeter = new PingMeter();

            RefreshWindowState();

            _windowTimer = new DispatcherTimer();
            _windowTimer.Interval = TimeSpan.FromMilliseconds(250);
            _windowTimer.Tick += (sender, e) => RefreshWindowState();
            _windowTimer.Start();

            _playerStatsTimer = new DispatcherTimer();
            _playerStatsTimer.Interval = TimeSpan.FromSeconds(1);
            _playerStatsTimer.Tick += async (sender, e) => await RefreshPlayerStats();
            _playerStatsTimer.Start();
        }

        private bool IsWindowActive()
        {
            var activatedHandle = WinApi.GetForegroundWindow();

            if (activatedHandle == IntPtr.Zero)
                return false;

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            return windowHandle == activatedHandle;
        }

        private void RefreshWindowState()
        {
            GameClient gameClient = App.GameClient!;
            if (gameClient == null)
            {
                WindowState = WindowState.Minimized;
                return;
            }
            ClientWindow? window = gameClient.Window;

            var newWindowState = IsWindowActive() || window != null && window.IsActive ? WindowState.Normal : WindowState.Minimized;
            var windowRect = window?.GetRect();

            if (windowRect != null)
            {
                if (windowRect.Value.Width < 1000)
                    newWindowState = WindowState.Minimized;

                Left = windowRect.Value.Left + 6;
                Top = windowRect.Value.Top + 50;
                Width = windowRect.Value.Width * 0.11;
            }

            if (WindowState != newWindowState)
            {
                WindowState = newWindowState;
                if (newWindowState == WindowState.Normal && Keyboard.FocusedElement != CharacterSearchInput)
                    window?.Activate();
            }
        }

        private async Task RefreshPlayerStats()
        {
            var gameClient = App.GameClient;
            if (gameClient == null)
            {
                if (_experienceCalculator != null)
                    _experienceCalculator = null;

                return;
            }

            int? experience = await gameClient.GetPlayerExperience();

            _experienceCalculator?.Tick(experience);

            Application.Current.Dispatcher.Invoke(() =>
            {
                PingLabel.Content = string.Format("{0} ms", _pingMeter.CurrentPing > 1000 ? ">1000" : _pingMeter.CurrentPing);
                LevelLabel.Content = _experienceCalculator?.ExperienceStats.Level?.ToString() ?? "-";
                ExperienceLabel.Content = FormatExperience(_experienceCalculator?.ExperienceStats.Experience);
                LevelProgressBar.Minimum = _experienceCalculator?.ExperienceStats.ExperienceForLevel ?? 0;
                LevelProgressBar.Maximum = _experienceCalculator?.ExperienceStats.ExperienceForNextLevel ?? 1;
                LevelProgressBar.Value = _experienceCalculator?.ExperienceStats.Experience ?? 0;
                ExperienceRemainingLabel.Content = FormatExperience(_experienceCalculator?.ExperienceStats.RemainingExperience);
                ExperiencePerHourLabel.Content = FormatExperience(_experienceCalculator?.ExperienceStats.ExperiencePerHour);

                int hours = _experienceCalculator?.ExperienceStats.RemainingTotalMinutes >= 60 ? (int)Math.Floor((double)_experienceCalculator!.ExperienceStats.RemainingTotalMinutes / 60) : 0;
                int minutes = (_experienceCalculator?.ExperienceStats.RemainingTotalMinutes ?? 0) % 60;
                var remaingTime = string.Empty;

                if (hours > 0)
                    remaingTime = hours + "h ";

                remaingTime += minutes + "m";

                if (hours + minutes == 0)
                    remaingTime = "< 1m";

                LevelTimeRemainingLabel.Content = _experienceCalculator?.ExperienceStats.RemainingTotalMinutes != null ? remaingTime : "-";
                LevelUpTimeLabel.Content = _experienceCalculator?.ExperienceStats.EstimatedAdvanceTime?.ToShortTimeString() ?? "-";
            });
        }

        private string FormatExperience(int? experience) => experience?.ToString("N0") ?? "-";

        private void ToggleCounterButton_Click(object sender, RoutedEventArgs e)
        {
            bool newState = _experienceCalculator == null;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                await SetExperienceCounterActive(newState);
            });
        }

        private async Task SetExperienceCounterActive(bool active)
        {
            var gameClient = App.GameClient;

            if (gameClient == null)
                active = false;

            int? experience = active ? await gameClient!.GetPlayerExperience() : null;

            _experienceCalculator = active ? new ExperienceCalculator(experience) : null;
            ToggleCounterButton.Content = active ? "Stop counter" : "Start counter";
            ResetCounterButton.IsEnabled = active;
        }

        private void ResetCounterButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await SetExperienceCounterActive(false);
                await RefreshPlayerStats();
                await SetExperienceCounterActive(true);
            });
        }

        private void CharacterSearchButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterSearchSubmit();
        }

        private void InfoMapViewerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.info/library/map/");
        }

        private void NetMapViewerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.net/map_viewer/");
        }

        private void TibiantisInfoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.info");
        }

        private void TibiantisXyzButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.xyz");
        }

        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private void OverlayWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Keyboard.FocusedElement != CharacterSearchInput)
            {
                App.GameClient?.Window?.Activate();
            }
        }

        private void CharacterSearchInput_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CharacterSearchInput.Clear();
        }

        private void CharacterSearchInput_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
            //    e.Key >= Key.F1 && e.Key <= Key.F12 ||
            //     ||
            //    e.Key == Key.Escape)
            //{
            //    App.GameClient?.Window?.Activate();
            //}
        }

        private void CharacterSearchSubmit()
        {
            if (!string.IsNullOrWhiteSpace(CharacterSearchInput.Text))
            {
                string encodedName = HttpUtility.UrlEncode(CharacterSearchInput.Text.ToLower());
                OpenUrl($"https://tibiantis.online/index.php?page=character&name={encodedName}");
                Task.Delay(1000).Wait();
            }
            
            App.GameClient?.Window?.Activate();
        }

        private void CharacterSearchInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CharacterSearchSubmit();
            }

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                e.Key >= Key.F1 && e.Key <= Key.F12 ||
                e.Key >= Key.Left && e.Key <= Key.Down ||
                e.Key == Key.Escape)
            {
                App.GameClient?.Window?.Activate();
            }
        }
    }
}
