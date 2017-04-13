// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that runs tasks on the current thread.</summary>
    public sealed class CurrentThreadTaskScheduler : TaskScheduler
    {
        /// <inheritdoc />
        public override int MaximumConcurrencyLevel => 1;

        /// <inheritdoc />
        protected override void QueueTask(Task task)
        {
            this.TryExecuteTask(task);
        }

        /// <inheritdoc />
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return this.TryExecuteTask(task);
        }

        /// <inheritdoc />
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }
    }
}