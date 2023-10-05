using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher.Clients
{
    public class GameClient : Client
    {
        public const string FileName = "Tibiantis.exe";
        private const string ProcessName = "Tibiantis";
        private const long CfgPathMemoryAddress = 1418427;
        public const int CfgPathMaxLength = 23;
        public const string ClientVersion = "1.0";
        public static string ClientFullPath => Path.Combine(ClientDirectoryFullPath, FileName);

        private IntPtr _processHandle { get; set; }
        //private IntPtr _windowHandle { get; set; } = IntPtr.Zero;
        private ProcessMemory? _memory { get; set; }
        private Profile _profile { get; init; }

        protected override string _clientFullPath => ClientFullPath;

        public GameClient(Profile profile) : base()
        {
            _profile = profile;
        }

        public override void Start()
        {
            GameClientValidator.ValidateCfgPath(_profile.CfgPath);

            base.Start();
            _processHandle = WinApi.OpenProcess(0x1f0fff, 0, (uint)_process.Id);//TODO: verify that process.Handle has different access rights
            _memory = new ProcessMemory(_processHandle);

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

        //public void FindWindow()
        //{
        //    IntPtr windowHandle = IntPtr.Zero;

        //    for (int i = 0; i < 50; i++)
        //    {
        //        Thread.Sleep(100);
        //        windowHandle = WinApi.FindWindow("Tibiantis c", "Tibiantis       ");

        //        if (windowHandle != IntPtr.Zero)
        //        {
        //            _windowHandle = windowHandle;
        //            return;
        //        }
        //    }

        //    throw new ApplicationException("Failed to attach to Tibiantis client window.");
        //}

        private void WriteCfgPath()
        {
            if (_memory is null)
                throw new NullReferenceException(nameof(_memory));

            _memory.WriteString(CfgPathMemoryAddress, _profile.CfgPath);
        }
    }
}