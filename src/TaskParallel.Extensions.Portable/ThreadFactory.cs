// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace TaskParallel.Extensions
{
    using System;
    using System.Threading.Tasks;

    public class ThreadFactory
    {
        public static readonly ThreadFactory Instance = new ThreadFactory();

        public event EventHandler<UnobservedTaskExceptionEventArgs> Error;

        public void InvokeError(Task task, UnobservedTaskExceptionEventArgs error)
        {
            this.Error?.Invoke(task, error);
        }

        public void Start(Action action)
        {
            var task = new Task(action);
            this.Start(task);
        }

        public void Start(Action action, TaskCreationOptions options)
        {
            var task = new Task(action, options);
            this.Start(task);
        }

        private void Start(Task task)
        {
            task.ContinueWith(
                t => {
                    var aggregateException = new AggregateException(t.Exception);
                    var unobservedTaskExceptionEventArgs = new UnobservedTaskExceptionEventArgs(aggregateException);
                    this.InvokeError(t, unobservedTaskExceptionEventArgs);
                    },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            task.Start();
        }
    }
}