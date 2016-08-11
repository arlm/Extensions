// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinForms.Extensions
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public static class WinFormsExtensions
    {
        public static void EnterFullScreenMode(this Form form)
        {
            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;
            form.Bounds = Screen.PrimaryScreen.Bounds;
        }

        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }

        public static bool IsScreenInLandscapeOrientation(this Form form)
        {
            int theScreenRectHeight = Screen.PrimaryScreen.Bounds.Height;
            int theScreenRectWidth = Screen.PrimaryScreen.Bounds.Width;

            // Compare height and width of screen and act accordingly.
            return theScreenRectHeight < theScreenRectWidth;
        }

        public static bool IsScreenInPortraitOrientation(this Form form)
        {
            int theScreenRectHeight = Screen.PrimaryScreen.Bounds.Height;
            int theScreenRectWidth = Screen.PrimaryScreen.Bounds.Width;

            // Compare height and width of screen and act accordingly.
            return theScreenRectHeight > theScreenRectWidth;
        }

        public static void LeaveFullScreenMode(this Form form)
        {
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.WindowState = FormWindowState.Normal;
        }
    }
}