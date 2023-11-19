using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace TibiantisLauncher
{
    internal static class WinApi
    {

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        internal static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesRead);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        internal static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        private static T? ReadUnmanagedStructure<T>(IntPtr hProcess, IntPtr lpAddr)
        {
            byte[] lpBuffer = new byte[Marshal.SizeOf(typeof(T))];
            ReadProcessMemory(hProcess, lpAddr, lpBuffer, new UIntPtr((uint)lpBuffer.Length), IntPtr.Zero);
            GCHandle handle = GCHandle.Alloc(lpBuffer, GCHandleType.Pinned);
            T? local = (T?)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T?));
            handle.Free();
            return local;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement windowPlacement);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            private ushort reserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public uint length;
            public uint flags;
            public ShowStates showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public enum ShowStates : uint
        {
            SW_FORCEMINIMIZE = 11,
            SW_HIDE = 0,
            SW_MAXIMIZE = 3,
            SW_MINIMIZE = 6,
            SW_RESTORE = 9,
            SW_SHOW = 5,
            SW_SHOWDEFAULT = 10,
            SW_SHOWMAXIMIZED = 3,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOWNORMAL = 1
        }
    }
}