using System;
using System.Diagnostics;
using System.IO;

namespace TibiantisLauncher.Clients
{
    public abstract class Client : IDisposable
    {
        public static string ClientDirectoryFullPath => Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty;
        protected abstract string _clientFullPath { get; }

        protected Process _process { get; init; }

        protected Client()
        {
            _process = new Process();

            var clientDir = ClientDirectoryFullPath;

            if (!clientDir.EndsWith(@"\"))
                clientDir += @"\";

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.WorkingDirectory = @$"{clientDir}";
            _process.StartInfo.FileName = _clientFullPath;
            _process.StartInfo.CreateNoWindow = true;
        }

        public virtual void Start()
        {
            _process.Start();
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
