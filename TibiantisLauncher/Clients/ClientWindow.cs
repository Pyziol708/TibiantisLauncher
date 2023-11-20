using System;
using System.Drawing;
using System.Windows;

namespace TibiantisLauncher.Clients
{
    internal class ClientWindow
    {
        private readonly IntPtr _windowHandle;
        
        public ClientWindow(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            Activate();
        }

        public void Activate()
        {
            WinApi.SetForegroundWindow(_windowHandle);
        }

        public bool IsActive()
        {
            if (_windowHandle == IntPtr.Zero)
                return false;

            return WinApi.GetForegroundWindow() == _windowHandle;
        }

        public Rectangle? GetRect()
        {
            if (_windowHandle == IntPtr.Zero)
                return null;

            var rect = new WinApi.Rect();
            WinApi.GetWindowRect(_windowHandle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            return bounds;
        }
    }
}
