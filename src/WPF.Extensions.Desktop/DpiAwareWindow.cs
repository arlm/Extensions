// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
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
    /// <summary>
    /// WPF Window base class that is DPI-aware
    /// </summary>
    public class DpiAwareWindow : Window
    {
        private const double EPSILON = 0.0001;

        private bool isPerMonitorEnabled;
        private HwndSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpiAwareWindow"/> class.
        /// </summary>
        public DpiAwareWindow()
        {
            // Set up the SourceInitialized event handler
            this.SourceInitialized += this.DpiAwareWindow_SourceInitialized;
        }

        /// <summary>
        /// Gets the current DPI value for this window
        /// </summary>
        public Dpi CurrentDpi { get; private set; }

        /// <summary>
        /// Gets the current scale-factor for this window
        /// </summary>
        public Point ScaleFactor { get; private set; }

        /// <summary>
        /// Gets the System DPI value
        /// </summary>
        public Dpi SystemDpi { get; private set; }

        /// <summary>
        /// Gets or sets the current DPI value for WPF infrastructure
        /// </summary>
        protected Point WpfDpi { get; set; }

        /// <summary>
        /// Notifies the window that the DPI changed
        /// </summary>
        public void OnDPIChanged()
        {
            this.ScaleFactor = new Point
            {
                X = this.CurrentDpi.X / this.WpfDpi.X,
                Y = this.CurrentDpi.Y / this.WpfDpi.Y
            };

            this.UpdateLayoutTransform(this.ScaleFactor);
        }

        /// <summary>
        /// Window message handler procedure that processes the DPI related messages
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        /// <param name="msg">Window message code</param>
        /// <param name="wParam">First message parameter</param>
        /// <param name="lParam">Second message parameter</param>
        /// <param name="handled">Notifies if the message was handled or not</param>
        /// <returns>Always returns <see cref="IntPtr.Zero"/> to let WPF to process all the messages</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "System.Windows.Interop.HwndSourceHook needs this pattern")]
        public virtual IntPtr WindowProcedureHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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
                        var monitorInfo = IntPtr.Zero;
                        var rcWorkArea = new PInvoke.RECT();
                        var rcMonitorArea = new PInvoke.RECT();

                        try
                        {
                            monitorInfo = Marshal.AllocHGlobal(Marshal.SizeOf<PInvoke.User32.MONITORINFO>());

                            if (PInvoke.User32.GetMonitorInfo(monitor, monitorInfo))
                            {
                                var temp = Marshal.PtrToStructure<PInvoke.User32.MONITORINFO>(monitorInfo);

                                // Get the Monitor's working area
                                rcWorkArea = temp.rcWork;
                                rcMonitorArea = temp.rcMonitor;
                            }
                        }
                        finally
                        {
                            if (monitorInfo != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(monitorInfo);
                            }
                        }

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
            this.isPerMonitorEnabled = DpiAwareApi.GetProcessDpiAwareness() == PInvoke.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;

            // Is the window in per-monitor DPI mode?
            if (this.isPerMonitorEnabled)
            {
                // It is. Calculate the DPI used by the System.
                this.SystemDpi = DpiAwareApi.SystemDPI;

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
            if (this.isPerMonitorEnabled)
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