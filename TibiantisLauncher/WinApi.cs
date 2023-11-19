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
        internal static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        internal static IntPtr GetBaseAddress(IntPtr hProcess)
        {
            SYSTEM_INFO system_info;
            GetSystemInfo(out system_info);
            IntPtr lpMinimumApplicationAddress = system_info.lpMinimumApplicationAddress;
            MEMORY_BASIC_INFORMATION structure = new MEMORY_BASIC_INFORMATION();
            uint dwLength = (uint)Marshal.SizeOf(structure);
            while (lpMinimumApplicationAddress.ToInt64() < system_info.lpMaximumApplicationAddress.ToInt64())
            {
                if (!VirtualQueryEx(hProcess, lpMinimumApplicationAddress, out structure, dwLength))
                {
                    Console.WriteLine("Could not VirtualQueryEx {0} segment at {1}; error {2}", hProcess.ToInt64(), lpMinimumApplicationAddress.ToInt64(), Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
                if ((structure.Type == 0x1000000) && (structure.BaseAddress == structure.AllocationBase) && ((structure.Protect & 0x100) != 0x100))
                {
                    IMAGE_DOS_HEADER image_dos_header = ReadUnmanagedStructure<IMAGE_DOS_HEADER>(hProcess, lpMinimumApplicationAddress);
                    if (image_dos_header.e_magic == 0x5a4d)
                    {
                        IntPtr lpAddr = new IntPtr(lpMinimumApplicationAddress.ToInt64() + (image_dos_header.e_lfanew + 4));
                        if ((ReadUnmanagedStructure<IMAGE_FILE_HEADER>(hProcess, lpAddr).Characteristics & 2) == 2)
                        {
                            return lpMinimumApplicationAddress;
                        }
                    }
                }
                long introduced6 = structure.BaseAddress.ToInt64();
                lpMinimumApplicationAddress = new IntPtr(introduced6 + structure.RegionSize.ToInt64());
            }
            return lpMinimumApplicationAddress;
        }

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
        internal struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;
            public ushort e_cblp;
            public ushort e_cp;
            public ushort e_crlc;
            public ushort e_cparhdr;
            public ushort e_minalloc;
            public ushort e_maxalloc;
            public ushort e_ss;
            public ushort e_sp;
            public ushort e_csum;
            public ushort e_ip;
            public ushort e_cs;
            public ushort e_lfarlc;
            public ushort e_ovno;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] e_res1;
            public ushort e_oemid;
            public ushort e_oeminfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public ushort[] e_res2;
            public int e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
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