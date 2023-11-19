using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace TibiantisLauncher
{
    internal class PingMeter
    {
        private readonly string _host = "51.89.155.163";
        private int _currentPing = int.MaxValue;
        public int CurrentPing => _currentPing;

        public PingMeter()
        {
            Task.Run(Start);
        }

        private async Task Start()
        {
            while (true)
                await CheckPing();
        }

        public async Task CheckPing()
        {
            Ping ping = new Ping();
            PingReply pingReply = await ping.SendPingAsync(_host);

            if (pingReply.Status == IPStatus.Success)
            {
                _currentPing = (int)pingReply.RoundtripTime;
                return;
            }

            _currentPing = int.MaxValue;
        }

        public async Task CancelCheckPing()
        {
            await Task.Delay(1000);
            _currentPing = int.MaxValue;
        }
    }
}
