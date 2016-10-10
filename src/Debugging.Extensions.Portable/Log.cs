// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Debugging.Extensions
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Debugging helpers
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs the exception and breaks if there is a debugger attached
        /// </summary>
        /// <param name="ex">The exception to be loggged</param>
        public static void Trace(this Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("Error: {0}", ex?.ToString());
                Debugger.Break();
            }
        }
    }
}