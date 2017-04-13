// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while
    /// running on top of the ThreadPool.
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;

        /// <summary>The list of tasks to be executed.</summary>
        private readonly LinkedList<Task> tasks = new LinkedList<Task>(); // protected by lock(tasks)

        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int maxDegreeOfParallelism;

        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private int delegatesQueuedOrRunning = 0; // protected by lock(tasks)

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedConcurrencyLevelTaskScheduler"/> class
        /// with the specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
            }

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <inheritdoc />
        public sealed override int MaximumConcurrencyLevel => this.maxDegreeOfParallelism;

        /// <inheritdoc />
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (this.tasks)
            {
                this.tasks.AddLast(task);

                if (this.delegatesQueuedOrRunning < this.maxDegreeOfParallelism)
                {
                    ++this.delegatesQueuedOrRunning;
                    this.NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <inheritdoc />
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!currentThreadIsProcessingItems)
            {
                return false;
            }

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
            {
                this.TryDequeue(task);
            }

            // Try to run the task.
            return this.TryExecuteTask(task);
        }

        /// <inheritdoc />
        protected sealed override bool TryDequeue(Task task)
        {
            lock (this.tasks)
            {
                return this.tasks.Remove(task);
            }
        }

        /// <inheritdoc />
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(this.tasks, ref lockTaken);
                if (lockTaken)
                {
                    return this.tasks.ToArray();
                }

                throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(this.tasks);
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(
                param =>
                {
                    // Note that the current thread is now processing work items.
                    // This is necessary to enable inlining of tasks into this thread.
                    currentThreadIsProcessingItems = true;
                    try
                    {
                        // Process all available items in the queue.
                        while (true)
                        {
                            Task item;
                            lock (this.tasks)
                            {
                                // When there are no more items to be processed,
                                // note that we're done processing, and get out.
                                if (this.tasks.Count == 0)
                                {
                                    --this.delegatesQueuedOrRunning;
                                    break;
                                }

                                // Get the next item from the queue
                                item = this.tasks.First.Value;
                                this.tasks.RemoveFirst();
                            }

                            // Execute the task we pulled out of the queue
                            this.TryExecuteTask(item);
                        }
                    }

                    // We're done processing items on the current thread
                    finally
                    {
                        currentThreadIsProcessingItems = false;
                    }
                },
                null);
        }
    }
}
