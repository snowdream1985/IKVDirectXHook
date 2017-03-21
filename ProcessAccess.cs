using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IKVDirectXHook
{
    internal enum ProcessAccess : Int32
    {
        Terminate = 0x1,
        CreateThread = 0x2,
        VMOperation = 0x8,
        VMRead = 0x10,
        VMWrite = 0x20,
        DuplicateHandle = 0x40,
        SetInformation = 0x200,
        QueryInformation = 0x400,
        Synchronize = 0x100000
    }
}
