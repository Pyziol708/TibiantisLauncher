using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using TibiantisLauncher.Clients;
using TibiantisLauncher.UI;

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

            _pingMeter = new PingMeter();

            RefreshWindowState();

            _windowTimer = new DispatcherTimer();
            _windowTimer.Interval = TimeSpan.FromMilliseconds(250);
            _windowTimer.Tick += (sender, e) => RefreshWindowState();
            _windowTimer.Start();

            _playerStatsTimer = new DispatcherTimer();
            _playerStatsTimer.Interval = TimeSpan.FromSeconds(2);
            _playerStatsTimer.Tick += async (sender, e) => await RefreshPlayerStats();
            _playerStatsTimer.Start();
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

            var newWindowState = WindowState.Normal;

            if (IsFocused)
                window?.Activate();

            if (window != null && window.IsActive)
                newWindowState = WindowState.Normal;
            else
                newWindowState = WindowState.Minimized;

            if (WindowState != newWindowState)
            {
                WindowState = newWindowState;
                if (newWindowState == WindowState.Normal)
                    window?.Activate();
            }

            var windowRect = window?.GetRect();
            if (windowRect != null)
            {
                Left = windowRect.Value.Left + 6;
                Top = windowRect.Value.Top + 50;
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

            if (_experienceCalculator == null)
                _experienceCalculator = new ExperienceCalculator(experience);
            else
                _experienceCalculator?.Tick(experience);

            Application.Current.Dispatcher.Invoke(() =>
            {
                PingLabel.Content = string.Format("{0} ms", _pingMeter.CurrentPing > 1000 ? ">1000" : _pingMeter.CurrentPing);
                LevelLabel.Content = _experienceCalculator!.ExperienceStats.Level?.ToString() ?? "-";
                ExperienceLabel.Content = _experienceCalculator!.ExperienceStats.Experience == null ? "-" : string.Format("{0:0,0}", _experienceCalculator!.ExperienceStats.Experience);
                LevelProgressBar.Minimum = _experienceCalculator!.ExperienceStats.ExperienceForLevel ?? 0;
                LevelProgressBar.Maximum = _experienceCalculator!.ExperienceStats.ExperienceForNextLevel ?? 1;
                LevelProgressBar.Value = _experienceCalculator!.ExperienceStats.Experience ?? 0;
                ExperienceRemainingLabel.Content = _experienceCalculator!.ExperienceStats.RemainingExperience == null ? "-" : string.Format("{0:0,0}", _experienceCalculator!.ExperienceStats.RemainingExperience);
                ExperiencePerHourLabel.Content = _experienceCalculator!.ExperienceStats.ExperiencePerHour == null ? "-" : string.Format("{0:0,0}", _experienceCalculator.ExperienceStats.ExperiencePerHour);
                int hours = _experienceCalculator!.ExperienceStats.RemainingTotalMinutes >= 60 ? (int)Math.Floor((double)_experienceCalculator!.ExperienceStats.RemainingTotalMinutes / 60) : 0;
                int minutes = (_experienceCalculator.ExperienceStats.RemainingTotalMinutes ?? 0) % 60;
                var remaingTime = string.Empty;
                
                if (hours > 0)
                    remaingTime = hours + "h ";
                
                if (minutes > 0)
                    remaingTime += minutes + "m";
                
                if (hours + minutes == 0)
                    remaingTime = "< 1m";

                LevelTimeRemainingLabel.Content = _experienceCalculator.ExperienceStats.RemainingTotalMinutes != null ? remaingTime : "-";
            });
        }

        private void ResetCounterButton_Click(object sender, RoutedEventArgs e)
        {
            _experienceCalculator?.Reset();
        }

        private void CharacterSearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.online/?page=character");
        }

        private void InfoMapViewerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl($"https://tibiantis.info/library/map");
        }

        private void NetMapViewerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl($"https://tibiantis.net/map_viewer/");
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
            App.GameClient?.Window?.Activate();
        }
    }
}
