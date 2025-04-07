﻿using PrimalEditor.DllWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace PrimalEditor.Utilities
{
    class RenderSurfaceHost : HwndHost
    {
        private readonly int VK_LBUTTON = 0x01;
        private readonly int _width = 800;
        private readonly int _height = 600;
        private IntPtr _renderWindowHandle = IntPtr.Zero;
        DelayEventTimer _resizeTimer;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        public int SurfaceID { get; private set; } = ID.INVALID_ID;

        private void Resize(object sender, DelayEventsTimerArgs e)
        {
            e.RepeatEvent = GetAsyncKeyState(VK_LBUTTON) < 0;
            if (!e.RepeatEvent)
            {
                EngineAPI.ResizeRenderSurface(SurfaceID);
                Logger.Log(MessageType.Info, "Resized");
            }

        }

        public RenderSurfaceHost(double width, double height)
        {
            _width = (int)width;
            _height = (int)height;
            _resizeTimer = new DelayEventTimer(TimeSpan.FromMilliseconds(250.0));
            _resizeTimer.Triggered += Resize;
            SizeChanged += (s, e) => _resizeTimer.Trigger();
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            SurfaceID = EngineAPI.CreateRenderSurface(hwndParent.Handle, _width, _height);
            Debug.Assert(ID.IsValid(SurfaceID));
            _renderWindowHandle = EngineAPI.GetWindowHandle(SurfaceID);
            Debug.Assert(_renderWindowHandle != IntPtr.Zero);

            return new HandleRef(this, _renderWindowHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            EngineAPI.RemoveRenderSurface(SurfaceID);
            SurfaceID = ID.INVALID_ID;
            _renderWindowHandle = IntPtr.Zero;
        }
    }
}
