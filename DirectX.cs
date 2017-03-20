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
using System.Threading;
using System.Runtime.InteropServices;
using EasyHook;
using SharpDX;
using SharpDX.Direct3D9;


namespace IKVDirectXHook
{
    public class Main : EasyHook.IEntryPoint
    {
        public Device deviceGlobal;
        const int D3D9_DEVICE_METHOD_COUNT = 119;

        LocalHook Direct3DDevice_EndSceneHook = null;
        LocalHook Direct3DDevice_IndexPrimitiveHook = null;
        List<IntPtr> id3dDeviceFunctionAddresses = new List<IntPtr>();

        IntPtr greenSharedHandle = IntPtr.Zero;
        IntPtr pTxSharedHandle = IntPtr.Zero;

        SharpDX.Rectangle d3dlr;

        Interface Interface;
        List<BaseTexture> baseTextures = new List<BaseTexture>();
        List<DXLogger> stride = new List<DXLogger>();

        DXLogger strideLog;
        BaseTexture baseTexture;

        Line line = null;
        Vector2[] vLine = new SharpDX.Vector2[5];
        ColorBGRA col;

        Boolean Startlog = true;
        Boolean found = false;

        Int32 iStride = 0;
        Int32 iBaseTex = 0;
        Int32 baseVertexIndexFound = 0;
        Int32 minVertexIndexFound = 0;
        Int32 numVerticesFound = 0;
        Int32 startIndexFound = 0;
        Int32 primCountFound = 0;
        Int32 bTex = 0;

        Int32 frames = 0;
        Int32 tickCount = 0;
        float frameRate = 0;

        static SharpDX.Direct3D9.Texture redTexture = null;
        static byte[] red =
        {
            0x42, 0x4D, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00
        };

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int Direct3D9Device_EndSceneDelegate(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int Direct3D9Device_IndexPrimitiveDelegate(IntPtr device, SharpDX.Direct3D9.PrimitiveType primitiveType, int baseVertexIndex, int minimumVertexIndex, int vertexCount, int startIndex, int primitiveCount);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        public Main(RemoteHooking.IContext InContext, String InChannelName)
        {
            Interface = RemoteHooking.IpcConnectClient<Interface>(InChannelName);
            Interface.Ping();
        }

        public void Run(RemoteHooking.IContext InContext, String InChannelName)
        {
            Thread key = new Thread(new ParameterizedThreadStart(onKeyState));
            key.IsBackground = true;
            key.Start();
            Hook();
        }

        int DrawIndexedPrimitiveHook(IntPtr devicePtr, PrimitiveType primitiveType, int baseVertexIndex, int minimumVertexIndex, int numVertices, int startIndex, int primCount)
        {
            Device device = (Device)devicePtr;

            VertexBuffer Stream_Data;
            int Offset = 0;
            int Stride = 0;
            device.GetStreamSource(0, out Stream_Data, out Offset, out Stride);
            Stream_Data.Dispose();
            if (Startlog)
            {
                if (Stride == iStride)
                {
                    baseTexture = device.GetTexture(0);
                    found = false;
                    for (int i = 0; i < baseTextures.Count; i++)
                    {
                        if (baseTextures[i].NativePointer == baseTexture.NativePointer)
                        {
                            found = true;
                        }
                    }

                    if (found == false)
                    {
                        baseTextures.Add(baseTexture);
                    }

                    if (baseTextures[iBaseTex].NativePointer == baseTexture.NativePointer)
                    {
                        bTex = baseTexture.LevelCount;
                        baseVertexIndexFound = baseVertexIndex;
                        minVertexIndexFound = minimumVertexIndex;
                        numVerticesFound = numVertices;
                        startIndexFound = startIndex;
                        primCountFound = primCount;
                        device.SetRenderState(SharpDX.Direct3D9.RenderState.FillMode, SharpDX.Direct3D9.FillMode.Solid);

                        if (redTexture == null)
                        {
                            redTexture = SharpDX.Direct3D9.Texture.FromMemory(device, red);
                        }

                        device.SetTexture(0, redTexture);
                        device.SetRenderState(SharpDX.Direct3D9.RenderState.ZEnable, false);
                        device.DrawIndexedPrimitive(primitiveType, baseVertexIndex, minimumVertexIndex, numVertices, startIndex, primCount);
                        device.SetRenderState(SharpDX.Direct3D9.RenderState.ZEnable, true);
                        found = false;

                        foreach (DXLogger item in stride)
                        {
                            if (item.Base == baseVertexIndex && item.Min == minimumVertexIndex && item.Num == numVertices && item.Start == startIndex && item.Prim == primCount)
                            {
                                found = true; break;
                            }
                        }

                        if (found == false)
                        {
                            strideLog.Base = baseVertexIndex;
                            strideLog.Min = minimumVertexIndex;
                            strideLog.Num = numVertices;
                            strideLog.Start = startIndex;
                            strideLog.Prim = primCount;
                            stride.Add(strideLog);
                        }
                    }
                }
            }
            device.DrawIndexedPrimitive(primitiveType, baseVertexIndex, minimumVertexIndex, numVertices, startIndex, primCount);
            return Result.Ok.Code;
        }

        int EndSceneHook(IntPtr devicePtr)
        {
            Frame();
            Device device = (Device)devicePtr;
            if (Startlog)
            {
                Text(device, "Istanbul Kıyamet Vakti DirectX Hook (" + GetFPS().ToString("0") + " fps) ", 10, 10);
                Text(device, "Programming: Alimşah Yıldırım", 10, 40);
                strideLogger(device);
            }
            device.EndScene();
            return Result.Ok.Code;
        }

        private void strideLogger(Device device)
        {
            DrawLine(device);
            Text(device, "Texture: " + baseTextures.Count.ToString()
            + "\nStride; " + iStride.ToString()
            + "\n Base Tex Num: " + (iBaseTex + 1).ToString()
            + "\nLog Enable: " + Startlog.ToString()
            + "\n\nF1: Stride++\nF2: Stride--\nF3: BaseTexNum++\nF4: BaseTexNum--\nF5: Log On/Off\n"
            + string.Format("{0:N0} {0:N0} {0:N0} {0:N0} {0:N0} {0:N0}",
            baseVertexIndexFound,
            minVertexIndexFound,
            numVerticesFound,
            startIndexFound,
            primCountFound,
            bTex), device.Viewport.Width - 170, device.Viewport.Height - 230);
        }

        void DrawLine(Device device)
        {
            try
            {
                if (line == null)
                {
                    line = new Line(device);
                    vLine[0] = new Vector2();
                    vLine[1] = new Vector2();
                    col = new ColorBGRA(0f, 100f, 0f, 1f);
                }

                vLine[0].X = device.Viewport.Width - 180;
                vLine[0].Y = device.Viewport.Height - 240;
                vLine[1].X = device.Viewport.Width - 180;
                vLine[1].Y = device.Viewport.Height - 20;

                vLine[2].X = device.Viewport.Width - 20;
                vLine[2].Y = device.Viewport.Height - 20;

                vLine[3].X = device.Viewport.Width - 20;
                vLine[3].Y = device.Viewport.Height - 240;

                vLine[4].X = device.Viewport.Width - 180;
                vLine[4].Y = device.Viewport.Height - 240;

                line.Width = 3;
                line.Draw(vLine, col);
            }
            catch (Exception ex)
            {
                Interface.Text("Info: DrawLine() " + ex.StackTrace);
            }

        }

        public void Text(Device device, string hook, int x, int y)
        {
            try
            {
                using (var font = new SharpDX.Direct3D9.Font(device, new FontDescription()
                {
                    Height = 18,
                    FaceName = "Arial",
                    Italic = false,
                    Width = 0,
                    MipLevels = 1,
                    CharacterSet = FontCharacterSet.Default,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural,
                    PitchAndFamily = FontPitchAndFamily.Default | FontPitchAndFamily.DontCare,
                    Weight = FontWeight.Bold
                }))
                {
                    {
                        font.DrawText(null, hook, x, y, new SharpDX.ColorBGRA(255, 0, 0, (byte)Math.Round((Math.Abs(6.0f) * 255f))));
                    }
                }
            }
            catch (Exception ex)
            {
                Interface.Text("Info: Text() " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void Hook()
        {
            id3dDeviceFunctionAddresses = new List<IntPtr>();

            using (Direct3D d3d = new Direct3D())
            {
                using (var renderForm = new System.Windows.Forms.Form())
                {
                    using (deviceGlobal = new Device(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1, DeviceWindowHandle = renderForm.Handle }))
                    {
                        id3dDeviceFunctionAddresses.AddRange(GetVTblAddresses(deviceGlobal.NativePointer, D3D9_DEVICE_METHOD_COUNT));
                    }
                }
            }
            Direct3DDevice_EndSceneHook = LocalHook.Create(id3dDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.EndScene], new Direct3D9Device_EndSceneDelegate(EndSceneHook), this);
            Direct3DDevice_EndSceneHook.ThreadACL.SetExclusiveACL(new Int32[1]);

            Direct3DDevice_IndexPrimitiveHook = LocalHook.Create(id3dDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.DrawIndexedPrimitive], new Direct3D9Device_IndexPrimitiveDelegate(DrawIndexedPrimitiveHook), this);

            Direct3DDevice_IndexPrimitiveHook.ThreadACL.SetExclusiveACL(new Int32[1]);
            Interface.Text("DEBUG: Hooking game.." + EasyHook.RemoteHooking.GetCurrentProcessId().ToString());
        }

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int numberOfMethods)
        {
            List<IntPtr> vtblAddresses = new List<IntPtr>();
            IntPtr vTable = Marshal.ReadIntPtr(pointer);

            for (int i = 0; i < numberOfMethods; i++)
            {
                vtblAddresses.Add(Marshal.ReadIntPtr(vTable, i * IntPtr.Size));
            }
            return vtblAddresses.ToArray();
        }
       
        public void Frame()
        {
            frames++;
            if (Math.Abs(Environment.TickCount - tickCount) > 1000)
            {
                frameRate = (float)frames * 1000 / Math.Abs(Environment.TickCount - tickCount);
                tickCount = Environment.TickCount;
                frames = 0;
            }
        }

        public float GetFPS()
        {
            return frameRate;
        }

        private void onKeyState(object device)
        {
            while (true)
            {
                if (GetAsyncKeyState(System.Windows.Forms.Keys.Insert) != 0)
                {
                    stride.Clear();
                    Startlog = !Startlog;
                    Console.Beep(500, 600);
                }

                if (GetAsyncKeyState(System.Windows.Forms.Keys.F1) != 0)
                {
                    iStride++; baseTextures.Clear(); iBaseTex = 0;
                    Thread.Sleep(200);
                }

                if (GetAsyncKeyState(System.Windows.Forms.Keys.R) != 0)
                {
                    if (iStride > 0)
                    {
                        iStride--; baseTextures.Clear(); iBaseTex = 0;
                        Thread.Sleep(200);
                    }
                }

                if (GetAsyncKeyState(System.Windows.Forms.Keys.F3) != 0)
                {
                    if (iBaseTex < baseTextures.Count - 1)
                    {
                        iBaseTex++;
                        Thread.Sleep(200);
                    }
                }

                if (GetAsyncKeyState(System.Windows.Forms.Keys.F4) != 0)
                {
                    if (iBaseTex > 0)
                    {
                        iBaseTex--;
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }

}
