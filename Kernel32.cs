using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace IKVDirectXHook
{
    static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 OpenProcess(ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] Boolean bInheritHandle, Int32 dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean CloseHandle(Int32 hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadProcessMemory(Int32 hProcess, UInt32 lpBaseAddress, out Byte buffer, Int32 dwSize, out Int32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadProcessMemory(Int32 hProcess, UInt32 lpBaseAddress, out Int32 buffer, Int32 dwSize, out Int32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadProcessMemory(Int32 hProcess, UInt32 lpBaseAddress, out float buffer, Int32 dwSize, out Int32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 GetCurrentProcess();
    }
}
