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
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Diagnostics;
using EasyHook;

namespace IKVDirectXHook
{
    class Interface : MarshalByRefObject
    {
        public delegate void Messages(string txt);

        public event Messages onMessage;

        public void Text(string txt)
        {
            if (onMessage != null)
            {
                onMessage(txt);
            }
        }
        public void Ping()
        {

        }
    }
}
