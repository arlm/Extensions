// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Windows.Core.Extensions
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Process helpers
    /// </summary>
    public static class Processes
    {
        /// <summary>
        /// Checks if the OS is an Unix platform
        /// </summary>
        public static bool IsRunningOnUnix
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;

                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Gets the process parent
        /// </summary>
        /// <param name="process">Process get the parent from</param>
        /// <returns>The process parent or null if it is a top-level process</returns>
        public static Process Parent(this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }

        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                using (var processId = new PerformanceCounter("Process", "ID Process", processIndexdName))
                {
                    if ((int)processId.NextValue() == pid)
                    {
                        return processIndexdName;
                    }
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            Process result;

            using (var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName))
            {
                var processId = (int)parentId.NextValue();

                result = processId == 0 ? null : Process.GetProcessById(processId);
            }

            return result;
        }
    }
}