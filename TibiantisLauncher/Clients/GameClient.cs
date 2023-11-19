using System;
using System.Diagnostics;
using System.IO;
using TibiantisLauncher.Clients.Memory;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher.Clients
{
    internal struct PlayerStats
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int PositionZ { get; set; }
    }

    internal class GameClient : Client
    {
        public const string FileName = "Tibiantis.exe";
        private const string ProcessName = "Tibiantis";

        public const int CfgPathMaxLength = 23;
        public const string ClientVersion = "1.0";

        protected override string _clientFullPath => ClientFullPath;
        public bool IsConnected { get => _memory?.ReadInt(MemoryAddresses.ConnectionStatus) > 8; }
        public static string ClientFullPath => Path.Combine(ClientDirectoryFullPath, FileName);
        private Profile? _profile { get; init; }

        public GameClient(Profile profile) : base()
        {
            _profile = profile;
        }

        public GameClient(Process process)
        {
            _process = process;
            _memory = new ProcessMemory(_process);
            _window = new ClientWindow(_process.MainWindowHandle);
            _process.EnableRaisingEvents = true;
            _process.Exited += OnExit;
        }

        public override void Start()
        {
            GameClientValidator.ValidateCfgPath(_profile?.CfgPath);

            base.Start();
            _memory = new ProcessMemory(_process);

            WriteCfgPath();
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

        public void WriteCfgPath()
        {
            if (_memory is null)
                throw new NullReferenceException(nameof(_memory));

            if (_profile is null)
                throw new NullReferenceException(nameof(_profile));

            _memory.WriteString(MemoryAddresses.CfgPath, _profile.CfgPath);
        }

        public PlayerStats ReadPlayerStats()
        {
            if (_memory is null)
                throw new NullReferenceException(nameof(_memory));

            var stats = new PlayerStats
            {
                Experience = _memory.ReadInt(MemoryAddresses.Experience),
                Level = _memory.ReadInt(MemoryAddresses.Level),
                PositionX = _memory.ReadInt(MemoryAddresses.PositionX),
                PositionY = _memory.ReadInt(MemoryAddresses.PositionY),
                PositionZ = _memory.ReadInt(MemoryAddresses.PositionZ)
            };

            return stats;
        }
    }
}