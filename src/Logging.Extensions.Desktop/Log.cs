// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Logging.Extensions.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ExceptionHandling.Extensions;

   public static class Log
    {
        public static readonly bool IsRunningInUnitTest;

        private const string LOG_FORMAT = "{0} [{1}@{2}:{3}]: {4}";

        private static readonly HashSet<string> UnitTestAttributes = new HashSet<string>
        {
            "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute",
            "NUnit.Framework.TestFixtureAttribute",
        };

        static Log()
        {
            IsRunningInUnitTest = false;

            foreach (var f in new StackTrace().GetFrames())
            {
                if (f.GetMethod().DeclaringType.GetCustomAttributes(false).Any(x => UnitTestAttributes.Contains(x.GetType().FullName)))
                {
                    IsRunningInUnitTest = true;
                    break;
                }
            }
        }

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
        public static void Caller(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var stacktrace = new StackTrace(true);

            if (stacktrace.FrameCount >= 3)
            {
                var frame = stacktrace.GetFrame(2);
                sourceFilePath = frame.GetFileName();
                memberName = frame.GetMethod().Name;
                sourceLineNumber = frame.GetFileLineNumber();
            }

            Info(message, sourceLineNumber, sourceFilePath, memberName);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Debug.ToString(), memberName, fileName, sourceLineNumber, message);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Debug, memberName, string.Format(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message));
            }
        }

        public static void Error(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Error.ToString(), memberName, fileName, sourceLineNumber, message);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Error, memberName, string.Format(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message));
            }
        }

        public static void Error(Exception ex, [CallerMemberName] string memberName = "")
        {
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Error.ToString(), memberName, ex.Flatten(), string.Empty, string.Empty);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Error, memberName, ex.Flatten());
            }
        }

        [Conditional("DEBUG")]
        public static void Info(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Information.ToString(), memberName, fileName, sourceLineNumber, message);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Information, memberName, string.Format(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message));
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Notification(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Notification.ToString(), memberName, fileName, sourceLineNumber, message);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Information, memberName, string.Format(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message));
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Trace(this Exception ex)
        {
            if (Debugger.IsAttached)
            {
                if (Debugger.IsLogging())
                {
                    Debugger.Log(0, "Error", ex?.ToString());
                }

                Debugger.Break();
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        public static void Warning(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (IsRunningInUnitTest || !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(LOG_FORMAT, DebugLevel.Warning.ToString(), memberName, fileName, sourceLineNumber, message);
            }
            else
            {
                Debugger.Log((int)DebugLevel.Warning, memberName, string.Format(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), string.Empty, fileName, sourceLineNumber, message));
            }
        }
    }
}