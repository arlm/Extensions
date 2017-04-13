// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>
    /// Provides a task scheduler that ensures only one task is executing at a time, and that tasks
    /// execute in the order that they were queued.
    /// </summary>
    public sealed class OrderedTaskScheduler : LimitedConcurrencyLevelTaskScheduler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedTaskScheduler"/> class.
        /// </summary>
        public OrderedTaskScheduler()
            : base(1)
        {
        }
    }
}
