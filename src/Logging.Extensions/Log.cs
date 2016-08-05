// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Logging.Extensions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using ExceptionHandling.Extensions;

    public static class Log
    {
        private const string LOG_FORMAT = "{0} [{1}@{2}:{3}]: {4}";

        private enum DebugLevel : int
        {
            Emergency = 0,
            Alerts = 1,
            Critical = 2,
            Error = 3,
            Warning = 4,
            Notification = 5,
            Information = 6,
            Debug = 7
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message);
            }
        }

        public static void Error(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message);
            }
        }

        public static void Error(Exception ex, [CallerMemberName] string memberName = "")
        {
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", ex.Flatten());
            }
        }

        [Conditional("DEBUG")]
        public static void Info(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Notification(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Trace(this Exception ex)
        {
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", ex?.ToString());
                Debugger.Break();
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Warning(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message);
            }
        }
    }
}