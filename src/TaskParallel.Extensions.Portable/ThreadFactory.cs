// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace TaskParallel.Extensions
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class ThreadFactory
    {
        /// <summary>
        /// The thread factory singleton instance
        /// </summary>
        public static readonly ThreadFactory Instance = new ThreadFactory();

        /// <summary>
        /// Event to handle task errors and exceptions
        /// </summary>
        public event EventHandler<UnobservedTaskExceptionEventArgs> OnError;

        /// <summary>
        /// Invokes the <see cref="OnError"/> event
        /// </summary>
        /// <param name="task">The task that had errors</param>
        /// <param name="error">The error information to send to the <see cref="OnError"/> handler</param>
        public void InvokeError(Task task, UnobservedTaskExceptionEventArgs error)
        {
            this.OnError?.Invoke(task, error);
        }

        /// <summary>
        /// Starts a new task
        /// </summary>
        /// <param name="action">The action to be executed on the task</param>
        public void Start(Action action)
        {
            var task = new Task(action);
            this.Start(task);
        }

        /// <summary>
        /// Starts a new task
        /// </summary>
        /// <param name="action">The action to be executed on the task</param>
        /// <param name="options">The task creation options</param>
        public void Start(Action action, TaskCreationOptions options)
        {
            var task = new Task(action, options);
            this.Start(task);
        }

        private void Start(Task task)
        {
            task.ContinueWith(t => 
                {
                    var aggregateException = new AggregateException(t.Exception);
                    var unobservedTaskExceptionEventArgs = new UnobservedTaskExceptionEventArgs(aggregateException);
                    this.InvokeError(t, unobservedTaskExceptionEventArgs);
                },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            task.Start();
        }
    }
}