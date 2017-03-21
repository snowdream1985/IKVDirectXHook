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
