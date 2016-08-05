// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All
// rights reserved. Licensed under the MIT license. See LICENSE.txt file in the project root for full
// license information.

using System;
using System.Diagnostics;

namespace Debugging.Extensions
{
    public static class Log
    {
        [Conditional("DEBUG")]
        [Conditional("TRACE")]
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