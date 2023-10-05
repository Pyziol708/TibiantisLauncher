using System;
using System.Text;

namespace TibiantisLauncher.Clients
{
    public class ProcessMemory
    {
        private readonly IntPtr _processHandle;
        private readonly long _baseAddress;

        public ProcessMemory(IntPtr processHandle)
        {
            _processHandle = processHandle;
            _baseAddress = WinApi.GetBaseAddress(_processHandle).ToInt64();
        }

        #region Reading
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
