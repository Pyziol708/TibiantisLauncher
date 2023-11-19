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
        }

        public void Activate()
        {
            WinApi.SetForegroundWindow(_windowHandle);
        }

        public WindowState GetState()
        {
            if (_windowHandle == IntPtr.Zero)
                return WindowState.Minimized;

            WinApi.WindowPlacement wp = default;
            wp.length = (byte)System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinApi.WindowPlacement));
            if (!WinApi.GetWindowPlacement(_windowHandle, out wp))
            {
                return WindowState.Minimized;
            }
            switch (wp.showCmd)
            {
                case WinApi.ShowStates.SW_SHOWNORMAL:
                case WinApi.ShowStates.SW_SHOWMAXIMIZED:
                    if (WinApi.GetForegroundWindow() == _windowHandle)
                    {
                        return WindowState.Normal;
                    }
                    return WindowState.Minimized;
                default:
                    return WindowState.Minimized;
            }
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
