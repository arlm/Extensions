// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All
// rights reserved. Licensed under the MIT license. See LICENSE.txt file in the project root for full
// license information.

using System.ComponentModel;
using System.Windows.Forms;

namespace WinForms.Extensions
{
    public static class WinFormsExtensions
    {
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
    }
}