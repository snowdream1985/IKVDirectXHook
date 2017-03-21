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
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IKVDirectXHook
{
    class Memory
    {
        private int processId;

        private int processHandle;

        public Memory(int process)
        {
            processId = process;
            Process.EnterDebugMode();

            processHandle = Kernel32.OpenProcess(ProcessAccess.VMRead | ProcessAccess.QueryInformation, false, processId);
            if (processHandle == 0)
            {
                throw new Win32Exception("Unable to open process ID: " + processId);
            }
        }

        ~Memory()
        {
            Kernel32.CloseHandle(processHandle);
        }
    }
}
