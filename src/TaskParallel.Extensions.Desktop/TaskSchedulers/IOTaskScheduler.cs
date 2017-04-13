// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that targets the I/O ThreadPool.</summary>
    public sealed class IOTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>Represents a task queued to the I/O pool.</summary>
        private unsafe class WorkItem
        {
            internal IOTaskScheduler _scheduler;
            internal NativeOverlapped* _pNOlap;
            internal Task _task;

            internal void Callback(uint errorCode, uint numBytes, NativeOverlapped* pNOlap)
            {
                // Execute the task
                _scheduler.TryExecuteTask(_task);

                // Put this item back into the pool for someone else to use
                var pool = _scheduler.availableWorkItems;

                if (pool != null)
                {
                    pool.PutObject(this);
                }
                else
                {
                    Overlapped.Free(pNOlap);
                }
            }
        }

        // A pool of available WorkItem instances that can be used to schedule tasks
        private ObjectCollection<WorkItem> availableWorkItems;

        /// <summary>Initializes a new instance of the IOTaskScheduler class.</summary>
        public unsafe IOTaskScheduler()
        {
            // Configure the object pool of work items
            availableWorkItems = new ObjectCollection<WorkItem>(
                () =>
                {
                    var wi = new WorkItem { _scheduler = this };
                    wi._pNOlap = new Overlapped().UnsafePack(wi.Callback, null);
                    return wi;
                },
                new ConcurrentStack<WorkItem>());
        }

        /// <inheritdoc />
        protected override unsafe void QueueTask(Task task)
        {
            var pool = availableWorkItems;

            if (pool == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var wi = pool.GetObject();
            wi._task = task;
            ThreadPool.UnsafeQueueNativeOverlapped(wi._pNOlap);
        }

        /// <inheritdoc />
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        /// <summary>Disposes of resources used by the scheduler.</summary>
        public unsafe void Dispose()
        {
            var pool = availableWorkItems;
            availableWorkItems = null;
            var workItems = pool.ToArrayAndClear();

            foreach (WorkItem wi in workItems)
            {
                Overlapped.Free(wi._pNOlap);
            }

            // NOTE: A window exists where some number of NativeOverlapped ptrs could
            // be leaked, if the call to Dispose races with work items completing.
        }

        /// <inheritdoc />
        protected override IEnumerable<Task> GetScheduledTasks() { return Enumerable.Empty<Task>(); }
    }
}
