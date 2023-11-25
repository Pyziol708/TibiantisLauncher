using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher.Clients
{
    internal class GameClient : Client
    {
        public const string FileName = "Tibiantis.exe";
        private const string ProcessName = "Tibiantis";

        public const int CfgPathMaxLength = 23;
        public const string ClientVersion = "1.0";
        private readonly ImageProcessor _imageProcessor = new ImageProcessor();

        protected override string _clientFullPath => ClientFullPath;
        public static string ClientFullPath => Path.Combine(ClientDirectoryFullPath, FileName);
        public static string ConfigFullPath => Path.Combine(ClientDirectoryFullPath, "game", "Tibiantis.cfg");
        public static string ConfigBackupFullPath => Path.Combine(ClientDirectoryFullPath, "game", "Tibiantis.cfg.bak");
        private Profile? _profile { get; init; }

        public GameClient(Profile profile) : base()
        {
            _profile = profile;
        }

        private void EnsureConfigBackup()
        {
            if (File.Exists(ConfigBackupFullPath))
                return;

            if (File.Exists(ConfigFullPath))
                File.Copy(ConfigFullPath, ConfigBackupFullPath);
        }

        public void SetConfig()
        {
            if (_profile == null)
                throw new NullReferenceException(nameof(_profile));

            var sourcePath = Path.Combine(ClientDirectoryFullPath, _profile.CfgPath);
            if (File.Exists(sourcePath))
                File.Copy(sourcePath, ConfigFullPath, true);
        }

        public void UnsetConfig()
        {
            if (_profile == null)
                throw new NullReferenceException(nameof(_profile));

            var sourcePath = ConfigFullPath;
            if (File.Exists(sourcePath))
                File.Copy(sourcePath, Path.Combine(ClientDirectoryFullPath, _profile.CfgPath), true);
        }

        public override void Start()
        {
            EnsureConfigBackup();
            SetConfig();
            GameClientValidator.ValidateCfgPath(_profile?.CfgPath);

            base.Start();
            //_memory = new ProcessMemory(_process);

            //WriteCfgPath();

            int tries = 0;
            while (_process.MainWindowHandle == IntPtr.Zero)
            {
                Task.Delay(1000).Wait();
                tries++;

                if (tries >= 15)
                    throw new Exception("Client window not found");
            }

            _window = new ClientWindow(_process.MainWindowHandle);
        }

        public static Process? FindClientProcess()
        {
            Process? process = null;
            Process[] processList = Process.GetProcessesByName(ProcessName);

            if (processList.Length > 0)
            {
                process = processList[0];
            }

            return process;
        }

        public async Task<int?> GetPlayerExperience()
        {
            var rightPanelBitmap = Window?.CaptureRightPanel();
            if (rightPanelBitmap == null)
                return null;

            var experience = await _imageProcessor.ExtractExperiencePointsAsync(rightPanelBitmap);

            return experience;
        }
    }
}