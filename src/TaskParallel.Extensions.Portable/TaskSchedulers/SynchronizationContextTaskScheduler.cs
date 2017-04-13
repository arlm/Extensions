// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that targets a specific SynchronizationContext.</summary>
    public sealed class SynchronizationContextTaskScheduler : TaskScheduler
    {
        /// <summary>The queue of tasks to execute, maintained for debugging purposes.</summary>
        private readonly ConcurrentQueue<Task> tasks;

        /// <summary>The target context under which to execute the queued tasks.</summary>
        private readonly SynchronizationContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextTaskScheduler"/> class.
        /// </summary>
        public SynchronizationContextTaskScheduler()
            : this(SynchronizationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextTaskScheduler"/> class
        /// with the specified SynchronizationContext.
        /// </summary>
        /// <param name="context">The SynchronizationContext under which to execute tasks.</param>
        public SynchronizationContextTaskScheduler(SynchronizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;
            this.tasks = new ConcurrentQueue<Task>();
        }

        /// <inheritdoc />
        public override int MaximumConcurrencyLevel => 1;

        /// <inheritdoc />
        protected override void QueueTask(Task task)
        {
            this.tasks.Enqueue(task);

            this.context.Post(
                delegate
                {
                    Task nextTask;

                    if (this.tasks.TryDequeue(out nextTask))
                    {
                        this.TryExecuteTask(nextTask);
                    }
                }, 
                null);
        }

        /// <inheritdoc />
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return this.context == SynchronizationContext.Current && this.TryExecuteTask(task);
        }

        /// <inheritdoc />
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return this.tasks.ToArray();
        }
    }
}