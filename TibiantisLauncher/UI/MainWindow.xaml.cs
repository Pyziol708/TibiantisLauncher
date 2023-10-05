using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TibiantisLauncher.Clients;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.UI;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameClient? _client;
        private CamPlayer? _camPlayer;
        private readonly ProfileManager _profileManager;
        public ObservableCollection<Profile> Profiles => _profileManager.Profiles;

        public MainWindow()
        {
            InitializeComponent();
            _profileManager = ProfileManager.Instance;

            string? version = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(3);

            VersionLabel.Content = $"v{version}";
            WindowDragHandle.MouseLeftButtonDown += (sender, e) => DragMove();

            _profileManager.LoadProfiles();

            DataContext = this;
        }

        #region Callbacks
        private void StartClientButton_Click(object sender, RoutedEventArgs e)
        {
            StartGameClient();
        }

        private void StartCamPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            StartCamPlayer();
        }

        private void MinimalizeIconButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddProfileWindow();
        }

        private void AddProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowAddProfileWindow();
        }

        private void RenameProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowRenameProfileWindow();
        }

        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRemoveProfileWindow();
        }

        private void RemoveProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowRemoveProfileWindow();
        }

        private void ProfileListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            StartGameClient();
        }

        private void ProfileListBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ProfileListBox.SelectedIndex != -1)
            {
                if (e.Key == System.Windows.Input.Key.F2)
                    ShowRenameProfileWindow();

                if (e.Key == System.Windows.Input.Key.Delete)
                    ShowRemoveProfileWindow();
            }
        }
        #endregion

        private void StartGameClient()
        {
            try
            {
                GameClientValidator.ValidateClientExistence();
                GameClientValidator.ValidateClientVersion();
                GameClientValidator.ValidateClientNotRunning();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show (ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _client = new GameClient((Profile)ProfileListBox.SelectedItem);
            _client.Start();
            Thread.Sleep(1000);
            Close();
        }

        private void StartCamPlayer()
        {
            try
            {
                GameClientValidator.ValidateCamPlayerExistence();
                GameClientValidator.ValidateClientVersion();
                GameClientValidator.ValidateClientNotRunning();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _camPlayer = new CamPlayer();
            _camPlayer.Start();
            Thread.Sleep(1000);
            Close();
        }

        private void ShowAddProfileWindow()
        {
            var window = new ProfileWindow() { Owner = this };
            var result = window.ShowDialog();

            if (result == true)
                _profileManager.AddProfile(window.ProfileName!);
        }

        private void ShowRenameProfileWindow()
        {
            var selectedProfile = (Profile)ProfileListBox.SelectedItem;
            if (selectedProfile is null)
                return;

            var window = new ProfileWindow() { ProfileName = selectedProfile.Name, Owner = this, Title = $"Rename Profile {selectedProfile.Name}" };
            var result = window.ShowDialog();

            if (result == true)
                _profileManager.RenameProfile(selectedProfile, window.ProfileName);
        }

        private void ShowRemoveProfileWindow()
        {
            var selectedProfile = (Profile)ProfileListBox.SelectedItem;
            if (selectedProfile is null)
                return;

            var window = new YesNoWindow() { Message = $"Are you sure you want to delete {selectedProfile.Name} profile?", Owner = this };
            var result = window.ShowDialog();

            if (result == true)
                _profileManager.RemoveProfile((Profile)ProfileListBox.SelectedItem);
        }
    }
}
