﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// memory
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;


namespace Cheat
{
    class Memory
    {

        static private bool _memoryHooked;
        static private bool _isMemory;
        static public bool _bSendPatched;
        static private readonly byte[] _pCallBytes = new byte[10];

        static private IntPtr _hHandle;
        static public IntPtr _pRecoilSync;
        static public IntPtr _pRadio;
        static public IntPtr _pCallCmdSyncData;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, IntPtr dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
  
        static public void Init()
        {
            _hHandle = OpenProcess(ProcessAccessFlags.All, false, Process.GetCurrentProcess().Id);
        }

        static public void Hook()
        {
            if (!_memoryHooked)
            {
                _memoryHooked = true;
                _pCallCmdSyncData = typeof(PlyMovementSync).GetMethod("CallCmdSyncData").MethodHandle.GetFunctionPointer();
                _pRecoilSync = typeof(Recoil).GetMethod("DoRecoil").MethodHandle.GetFunctionPointer();
                _pRadio = typeof(Radio).GetMethod("Start", BindingFlags.NonPublic |
                                                         BindingFlags.Instance).MethodHandle.GetFunctionPointer();

                //_isMemory = true;
                ReadMemory(_pCallCmdSyncData + 0x1d9, _pCallBytes, 10);
                _isMemory = true;

                // fix recoil
                WriteMemory(_pRecoilSync + 0x61, new byte[] { 0x48, 0x39, 0xCF }, 3 * sizeof(byte));

                // enable all radio
                WriteMemory(_pRadio + 0x15F, new byte[] { 0x1 }, sizeof(byte));
            }
        }

        static public void SetSendPacket(bool val)
        {
            if (!_isMemory)
            {
                return;
            }

            if (!val)
            {
                _bSendPatched = true;

                byte[] patch = { 0x90, 0x90, 0x90 };
                WriteMemory(_pCallCmdSyncData + 0x1d9, patch, 3 * sizeof(byte));
            }
            else
            {
                _bSendPatched = false;
                WriteMemory(_pCallCmdSyncData + 0x1d9, _pCallBytes, 3 * sizeof(byte));
            }
        }

        static public long ReadMemory(IntPtr address, byte[] buffer, long size)
        {
            IntPtr read;
            ReadProcessMemory(_hHandle, address, buffer, (IntPtr)size, out read);
            return (long)read;
        }

        static public uint WriteMemory(IntPtr address, byte[] data, uint length)
        {
            UIntPtr bytesWritten;
            WriteProcessMemory(_hHandle, address, data, length, out bytesWritten);
            return bytesWritten.ToUInt32();
        }
        static public uint WriteMemory(IntPtr address, string _data)
        {
            UIntPtr bytesWritten;
            byte[] buff = new byte[_data.Length];
            for (int i = 0; i < _data.Length; i++) buff[i] = (byte)_data[i];

            WriteProcessMemory(_hHandle, address, buff, (uint)buff.Length, out bytesWritten);
            return bytesWritten.ToUInt32();
        }
    }
}
