using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace TibiantisLauncher.Clients.Memory
{
    internal class ProcessMemory
    {
        private readonly IntPtr _processHandle;
        private readonly long _baseAddress;

        public ProcessMemory(Process process)
        {
            _processHandle = process.Handle;

            _baseAddress = WinApi.GetBaseAddress(_processHandle).ToInt64();
        }

        #region Reading
        public int ReadPlayerId()
        {
            byte[] buffer = new byte[4];
            WinApi.ReadProcessMemory(_processHandle, new IntPtr(0x19A1DC), buffer, 4, out _);

            return BitConverter.ToInt32(buffer, 0);
        }

        public byte ReadByte(long address)
        {
            return ReadBytes(address, 1)[0];
        }

        public byte[] ReadBytes(long address, uint bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            WinApi.ReadProcessMemory(_processHandle, new IntPtr(_baseAddress + address), buffer, bytesToRead, out _);

            return buffer;
        }

        public string ReadString(long address)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (byte i = ReadByte(address); i != 0; i = ReadByte(address))
            {
                stringBuilder.Append((char)i);
                address += 1L;
            }

            return stringBuilder.ToString();
        }

        public int ReadInt(long address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }
        #endregion

        #region Writting
        public bool WriteBytes(long address, byte[] bytes)
        {
            return WinApi.WriteProcessMemory(_processHandle, new IntPtr(_baseAddress + address), bytes, (uint)bytes.Length, out _) != 0;
        }

        public bool WriteString(long address, string value)
        {
            value += '\0';
            byte[] bytes = Encoding.ASCII.GetBytes(value);

            return WriteBytes(address, bytes);
        }
        #endregion
    }
}
