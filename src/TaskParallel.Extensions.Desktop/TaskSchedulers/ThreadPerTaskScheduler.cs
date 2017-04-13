// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that dedicates a thread per task.</summary>
    public class ThreadPerTaskScheduler : TaskScheduler
    {
        private ThreadPriority priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPerTaskScheduler"/> class
        /// that creates all threads using the given priority.
        /// </summary>
        /// <param name="priority">The priority that the threads will have</param>
        public ThreadPerTaskScheduler(ThreadPriority priority = ThreadPriority.Normal)
        {
            this.priority = priority;
        }

        /// <inheritdocs />
        protected override IEnumerable<Task> GetScheduledTasks() { return Enumerable.Empty<Task>(); }

        /// <inheritdocs />
        protected override void QueueTask(Task task)
        {
            var thread = new Thread(
                () => this.TryExecuteTask(task))
            {
                IsBackground = true,
                Priority = this.priority
            };

            thread.Start();
        }

        /// <inheritdocs />
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }
    }
}
