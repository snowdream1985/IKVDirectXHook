/*
 *
 * Copyright (C) 2017 Alimşah YILDIRIM <alimsahy@gmail.com>
 *
 * This file part of IKVDirectXHook
 *
 * IKVDirectXHook is free software: you can redistribute it and/or modify
 * it under free terms of the GNU General Public Licence as published by
 * the Free Software Foundation, either version 3 of the licence, or
 * any later version
 *
 * IKVDirectXHook is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
*/

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
