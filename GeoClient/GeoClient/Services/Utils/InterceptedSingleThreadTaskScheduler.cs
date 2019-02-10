using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoClient.Services.Utils
{
    public delegate AfterTaskExecutionHook BeforeTaskExecutionHook();

    public delegate void AfterTaskExecutionHook();

    public class InterceptedSingleThreadTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _taskCollection;

        public static BeforeTaskExecutionHook AroundTaskExecution = delegate
        {
            Console.WriteLine("Not delegate for intercepted task scheduler registered.");
            return () => { };
        };

        public InterceptedSingleThreadTaskScheduler()
        {
            _taskCollection = new BlockingCollection<Task>();
            StartMainThread();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _taskCollection.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            _taskCollection.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _taskCollection.CompleteAdding();
            _taskCollection.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void StartMainThread()
        {
            var mainThread = new Thread(ExecuteNextTask);
            if (!mainThread.IsAlive)
            {
                mainThread.Start();
            }
            else
            {
                Console.WriteLine("Main thread is already running..?");
            }
        }

        private void ExecuteNextTask()
        {
            foreach (var task in _taskCollection.GetConsumingEnumerable())
            {
                var afterTaskExecutionHook = AroundTaskExecution();
                TryExecuteTask(task);
                afterTaskExecutionHook();
            }
        }
    }
}