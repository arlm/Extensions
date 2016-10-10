// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Logging.Extensions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using ExceptionHandling.Extensions;

    /// <summary>
    /// Logging helpers
    /// </summary>
    public static class Log
    {
        private const string LOG_FORMAT = "{0} [{1}@{2}:{3}]: {4}";

        private enum DebugLevel
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

        /// <summary>
        /// Logs a debug message with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="message">Message to be logged with the caller reference</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Debug(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), memberName, fileName, sourceLineNumber, message);
            }
        }

        /// <summary>
        /// Logs an error message with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="message">Message to be logged with the caller reference</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Error(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), memberName, fileName, sourceLineNumber, message);
            }
        }

        /// <summary>
        /// Logs an exception with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="ex">The exception to be logged</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Error(Exception ex, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Error on {0}@{1}:{2}: {3}", memberName, sourceFilePath, sourceLineNumber, ex.Flatten());
            }
        }

        /// <summary>
        /// Logs an information message with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="message">Message to be logged with the caller reference</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Info(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), memberName, fileName, sourceLineNumber, message);
            }
        }

        /// <summary>
        /// Logs a notification message with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="message">Message to be logged with the caller reference</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Notification(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), memberName, fileName, sourceLineNumber, message);
            }
        }

        /// <summary>
        /// Logs the exception and breaks if there is a debugger attached
        /// </summary>
        /// <param name="ex">The exception to be loggged</param>
        public static void Trace(this Exception ex)
        {
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", ex?.ToString());
                Debugger.Break();
            }
        }

        /// <summary>
        /// Logs a warning message with information from the caller of the method that in which it is contained
        /// </summary>
        /// <param name="message">Message to be logged with the caller reference</param>
        /// <param name="sourceLineNumber">The line number of the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="sourceFilePath">The source file containing the method call. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <param name="memberName">The member name calling the method. This parameter is automatically set by the runtime, which will be overwritten by the value you set here.</param>
        /// <remarks>Note that lambdas, closures and anonymous methods/delegates have class and method names that are automatically generated</remarks>

        public static void Warning(string message, [CallerLineNumber] int sourceLineNumber = -1, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        {
            var fileName = Path.GetFileName(sourceFilePath);
            if (Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(LOG_FORMAT, DateTime.Now.ToString("HH:mm:ss.ffff"), memberName, fileName, sourceLineNumber, message);
            }
        }
    }
}