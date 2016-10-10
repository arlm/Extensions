// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace TaskParallel.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a pump that supports running asynchronous methods on the current thread.
    /// </summary>
    public static class AsyncPump
    {
        /// <summary>
        /// Runs the specified asynchronous function.
        /// </summary>
        /// <param name="func">The asynchronous function to execute.</param>
        public static void Run(Func<Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var prevCtx = SynchronizationContext.Current;

            try
            {
                // Invoke the function and alert the context to when it completes
                Task t = null;

                // Establish the new context
                using (var syncCtx = new SingleThreadSynchronizationContext())
                {
                    SynchronizationContext.SetSynchronizationContext(syncCtx);
                    t = func();

                    if (t == null)
                    {
                        throw new InvalidOperationException("No task provided.");
                    }

                    t.ContinueWith(
                        delegate
                        {
                            syncCtx.Complete();
                        },
                        TaskScheduler.Default);

                    // Pump continuations and propagate any exceptions
                    syncCtx.RunOnCurrentThread();
                }

                t?.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        /// Provides a SynchronizationContext that's single-threaded.
        /// </summary>
        private sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
        {
            /// <summary>
            /// The queue of work items.
            /// </summary>
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> queue =
                    new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            // <summary>
            // The processing thread.
            // </summary>
            // private readonly Thread m_thread = Thread.CurrentThread;
            // TODO: Make it work as portable as possible

            /// <summary>
            /// Notifies the context that no more work will arrive.
            /// </summary>
            public void Complete()
            {
                this.queue.CompleteAdding();
            }

            public void Dispose()
            {
                this.queue.Dispose();
            }

            /// <summary>
            /// Dispatches an asynchronous message to the synchronization context.
            /// </summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                if (d == null)
                {
                    throw new ArgumentNullException(nameof(d));
                }

                this.queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            /// <summary>
            /// Runs an loop to process all queued work items.
            /// </summary>
            public void RunOnCurrentThread()
            {
                foreach (var workItem in this.queue.GetConsumingEnumerable())
                {
                    workItem.Key(workItem.Value);
                }
            }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="d">The delegate to send</param>
            /// <param name="state">Object state</param>
            [Obsolete("Not supported", true)]
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        }
    }
}