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
