// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace TaskParallel.Extensions
{
    using System.Threading.Tasks;
    using Debugging.Extensions;

    /// <summary>
    /// Parallel task helpers
    /// </summary>
    public static class TaskParallelExtensions
    {
        /// <summary>
        /// Runs tasks sequencially, one task after the other has finished.
        /// </summary>
        /// <param name="parent">The task to wait for execution</param>
        /// <param name="next">The task to be run sequencially</param>
        /// <returns></returns>
        public static Task Then(this Task parent, Task next)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException(nameof(parent));
            }

            var tcs = new TaskCompletionSource<object>();

            parent.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    task.Exception.Trace();

                    tcs.SetException(task.Exception.InnerException);
                }
                else
                {
                    next.ContinueWith(nt =>
                    {
                        if (nt.IsFaulted)
                        {
                            nt.Exception.Trace();
                            tcs.SetException(nt.Exception.InnerException);
                        }
                        else
                        {
                            tcs.SetResult(null);
                        }
                    });

                    next.Wait();
                }
            });

            return tcs.Task;
        }
    }
}