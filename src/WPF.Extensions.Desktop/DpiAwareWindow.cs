﻿// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WinApi.HighDpi;

using Point = System.Drawing.Point;

namespace WPF.Extensions.Desktop
{
    public class DpiAwareWindow : Window
    {
        private const double EPSILON = 0.0001;

        private bool IsPerMonitorEnabled;
        private HwndSource source;
        private Dpi systemDpi;

        public DpiAwareWindow()
        {
            // Set up the SourceInitialized event handler
            this.SourceInitialized += this.DpiAwareWindow_SourceInitialized;
        }

        public Dpi CurrentDpi { get; private set; }

        public Point ScaleFactor { get; private set; }

        protected Point WpfDpi { get; set; }

        public void OnDPIChanged()
        {
            this.ScaleFactor = new Point
            {
                X = this.CurrentDpi.X / this.WpfDpi.X,
                Y = this.CurrentDpi.Y / this.WpfDpi.Y
            };

            this.UpdateLayoutTransform(this.ScaleFactor);
        }

        public virtual unsafe IntPtr WindowProcedureHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Determine which Monitor is displaying the Window
            var monitor = PInvoke.User32.MonitorFromWindow(hwnd, PInvoke.User32.MonitorOptions.MONITOR_DEFAULTTONEAREST);

            // Switch on the message.
            switch ((PInvoke.User32.WindowMessage)msg)
            {
                case PInvoke.User32.WindowMessage.WM_DPICHANGED:
                    // Marshal the value in the lParam into a Rect.
                    var newDisplayRect = (PInvoke.RECT)Marshal.PtrToStructure(lParam, typeof(PInvoke.RECT));

                    // Set the Window's position & size.
                    var upperLeft = new Vector(newDisplayRect.left, newDisplayRect.top);
                    var ul = this.source.CompositionTarget.TransformFromDevice.Transform(upperLeft);

                    var size = new Vector(newDisplayRect.right = newDisplayRect.left, newDisplayRect.bottom - newDisplayRect.top);
                    var hw = this.source.CompositionTarget.TransformFromDevice.Transform(size);

                    this.Left = ul.X;
                    this.Top = ul.Y;
                    this.Width = hw.X;
                    this.Height = hw.Y;

                    // Remember the current DPI settings.
                    var oldDpi = this.CurrentDpi;

                    // Get the new DPI settings from wParam
                    this.CurrentDpi = new Dpi
                    {
                        X = wParam.ToInt32() >> 16,
                        Y = wParam.ToInt32() & 0x0000FFFF
                    };

                    if (oldDpi.X != this.CurrentDpi.X || oldDpi.Y != this.CurrentDpi.Y)
                    {
                        this.OnDPIChanged();
                    }

                    handled = true;
                    return IntPtr.Zero;

                case PInvoke.User32.WindowMessage.WM_GETMINMAXINFO:
                    // lParam has a pointer to the MINMAXINFO structure. Marshal it into managed memory.
                    var mmi = (PInvoke.User32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(PInvoke.User32.MINMAXINFO));
                    if (monitor != IntPtr.Zero)
                    {
                        var monitorInfo = new PInvoke.User32.MONITORINFO();
                        PInvoke.User32.GetMonitorInfo(monitor, new IntPtr(&monitorInfo));

                        // Get the Monitor's working area
                        PInvoke.RECT rcWorkArea = monitorInfo.rcWork;
                        PInvoke.RECT rcMonitorArea = monitorInfo.rcMonitor;

                        // Adjust the maximized size and position to fit the work area of the current monitor
                        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                    }

                    // Copy our changes to the mmi object back to the original
                    Marshal.StructureToPtr(mmi, lParam, true);
                    handled = false;
                    return IntPtr.Zero;

                default:
                    // Let the WPF code handle all other messages. Return 0.
                    return IntPtr.Zero;
            }
        }

        private void DpiAwareWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.source = (HwndSource)PresentationSource.FromVisual(this);
            this.source.AddHook(this.WindowProcedureHook);

            // Determine if this application is Per Monitor DPI Aware.
            this.IsPerMonitorEnabled = DpiAwareApi.GetProcessDpiAwareness() == PInvoke.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;

            // Is the window in per-monitor DPI mode?
            if (this.IsPerMonitorEnabled)
            {
                // It is. Calculate the DPI used by the System.
                this.systemDpi = DpiAwareApi.SystemDPI;

                // Calculate the DPI used by WPF.
                this.WpfDpi = new Point
                {
                    X = (int)(96 * this.source.CompositionTarget.TransformToDevice.M11),
                    Y = (int)(96 * this.source.CompositionTarget.TransformToDevice.M22)
                };

                // Get the Current DPI of the monitor of the window.
                this.CurrentDpi = DpiAwareApi.GetDpiForWindow(this.source.Handle);

                // Calculate the scale factor used to modify window size, graphics and text.
                this.ScaleFactor = new Point
                {
                    X = this.CurrentDpi.X / this.WpfDpi.X,
                    Y = this.CurrentDpi.Y / this.WpfDpi.Y
                };

                // Update Width and Height based on the on the current DPI of the monitor
                this.Width = this.Width * this.ScaleFactor.X;
                this.Height = this.Height * this.ScaleFactor.Y;

                // Update graphics and text based on the current DPI of the monitor.
                this.UpdateLayoutTransform(this.ScaleFactor);
            }
        }

        private void UpdateLayoutTransform(Point scaleFactor)
        {
            if (this.IsPerMonitorEnabled)
            {

                if (Math.Abs(this.ScaleFactor.X - 1.0) > EPSILON || Math.Abs(this.ScaleFactor.Y - 1.0) > EPSILON)
                {
                    this.LayoutTransform = new ScaleTransform(scaleFactor.X, scaleFactor.Y);
                }
                else
                {
                    this.LayoutTransform = null;
                }
            }
        }
    }
}