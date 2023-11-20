using Serilog;
using System;
using System.Diagnostics;
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
        private bool _locked = true;
        private PlayerStats _playerStats;
        private ExperienceCalculator? _experienceCalculator;
        private PingMeter _pingMeter;

        public GameClientOverlayWindow()
        {
            InitializeComponent();

            _pingMeter = new PingMeter();

            RefreshWindowState();
            RefreshPlayerStats();

            _windowTimer = new DispatcherTimer();
            _windowTimer.Interval = TimeSpan.FromMilliseconds(250);
            _windowTimer.Tick += (sender, e) => RefreshWindowState();
            _windowTimer.Start();

            _playerStatsTimer = new DispatcherTimer();
            _playerStatsTimer.Interval = TimeSpan.FromSeconds(1);
            _playerStatsTimer.Tick += (sender, e) => RefreshPlayerStats();
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

            if (window.IsActive())
                newWindowState = WindowState.Normal;
            else
                newWindowState = WindowState.Minimized;

            if (!gameClient!.IsConnected)
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

        private void RefreshPlayerStats()
        {
            var gameClient = App.GameClient;
            if (gameClient == null || !gameClient.IsConnected)
            {
                if (_experienceCalculator != null)
                    _experienceCalculator = null;
                return;
            }

            _playerStats = gameClient.ReadPlayerStats();

            if (_experienceCalculator == null)
                _experienceCalculator = new ExperienceCalculator(_playerStats.Experience);
            else
                _experienceCalculator?.Tick(_playerStats.Level, _playerStats.Experience);

            //WindowState = gameClient.GetWindowState();
            PingLabel.Content = string.Format("{0} ms", _pingMeter.CurrentPing > 1000 ? ">1000" : _pingMeter.CurrentPing);
            LevelLabel.Content = _playerStats.Level;
            ExperienceLabel.Content = string.Format("{0:0,0}", _playerStats.Experience);
            LevelProgressBar.Minimum = _experienceCalculator!.ExperienceStats.ExperienceForLevel;
            LevelProgressBar.Maximum = _experienceCalculator!.ExperienceStats.ExperienceForNextLevel;
            LevelProgressBar.Value = _playerStats.Experience;
            ExperienceRemainingLabel.Content = string.Format("{0:0,0}", _experienceCalculator!.ExperienceStats.RemainingExperience);
            ExperiencePerHourLabel.Content = _experienceCalculator!.ExperienceStats.ExperiencePerHour > 0 ? string.Format("{0:0,0}", _experienceCalculator.ExperienceStats.ExperiencePerHour) : "-";
            int hours = _experienceCalculator!.ExperienceStats.RemainingTotalMinutes > 0 ? (int)Math.Floor((double)_experienceCalculator!.ExperienceStats.RemainingTotalMinutes / 60) : 0;
            int minutes = _experienceCalculator.ExperienceStats.RemainingTotalMinutes % 60;
            var remaingTime = "";
            if (hours > 0)
                remaingTime = hours + "h ";
            if (minutes > 0)
                remaingTime += minutes + "m";
            LevelTimeRemainingLabel.Content = _experienceCalculator.ExperienceStats.RemainingTotalMinutes > 0 ? remaingTime : "-";
        }

        private void ResetCounterButton_Click(object sender, RoutedEventArgs e)
        {
            _experienceCalculator?.Reset(_playerStats.Experience);
        }

        private void CharacterSearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://tibiantis.online/?page=character");
        }

        private void MapViewerButton_Click(object sender, RoutedEventArgs e)
        {
            var position = App.GameClient!.ReadPlayerPosition();
            OpenUrl($"https://tibiantis.info/library/map#{position.X},{position.Y},{position.Z},8");
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
