// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinForms.Extensions
{
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Windows.Forms related helper methods
    /// </summary>
    public static class WinFormsExtensions
    {
        /// <summary>
        /// Starts full-screen mode
        /// </summary>
        /// <param name="form">Form to enter full-screen mode</param>
        public static void EnterFullScreenMode(this Form form)
        {
            if (form == null)
            {
                throw new System.ArgumentNullException(nameof(form));
            }

            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;
            form.Bounds = Screen.PrimaryScreen.Bounds;
        }

        /// <summary>
        /// Runs the action on the form or component thread on a per-needed basis.
        /// </summary>
        /// <param name="obj">The form or component instance</param>
        /// <param name="action">The action to be performed</param>
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }

            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action?.Invoke();
            }
        }

        /// <summary>
        /// Checks if the screen is on landscape orientation
        /// </summary>
        /// <param name="form">Form to test</param>
        /// <returns>True if the screen is on landscape orientation, false otherwise</returns>
        public static bool IsScreenInLandscapeOrientation(this Form form)
        {
            if (form == null)
            {
                throw new System.ArgumentNullException(nameof(form));
            }

            int theScreenRectHeight = Screen.PrimaryScreen.Bounds.Height;
            int theScreenRectWidth = Screen.PrimaryScreen.Bounds.Width;

            // Compare height and width of screen and act accordingly.
            return theScreenRectHeight < theScreenRectWidth;
        }

        /// <summary>
        /// Checks if the screen is on portrait orientation
        /// </summary>
        /// <param name="form">Form to test</param>
        /// <returns>True if the screen is on portrait orientation, false otherwise</returns>
        public static bool IsScreenInPortraitOrientation(this Form form)
        {
            if (form == null)
            {
                throw new System.ArgumentNullException(nameof(form));
            }

            int theScreenRectHeight = Screen.PrimaryScreen.Bounds.Height;
            int theScreenRectWidth = Screen.PrimaryScreen.Bounds.Width;

            // Compare height and width of screen and act accordingly.
            return theScreenRectHeight > theScreenRectWidth;
        }

        /// <summary>
        /// Leaves full-screen mode
        /// </summary>
        /// <param name="form">Form to leave full-screen mode</param>
        public static void LeaveFullScreenMode(this Form form)
        {
            if (form == null)
            {
                throw new System.ArgumentNullException(nameof(form));
            }

            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.WindowState = FormWindowState.Normal;
        }
    }
}