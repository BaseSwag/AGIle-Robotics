using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGIle_Robotics.Extension
{
    public class WorkPool
    {
        public int MaxThreads
        {
            get => maxThreads;
            set
            {
                if(value != maxThreads)
                {
                    maxThreads = value;
                    lock (workers)
                    {
                        Array.Resize(ref workers, maxThreads);
                    }
                }
            }
        }
        private int maxThreads = 4;

        private Queue<Task> taskList = new Queue<Task>();
        private Task[] workers;

        public int Workload
        {
            get
            {
                lock (workers)
                {
                    int count = 0;
                    for (int i = 0; i < workers.Length; i++)
                        if (workers[i] != null && workers[i].Status == TaskStatus.Running)
                            count++;
                    return count;
                }
            }
        }

        private event EventHandler<Task> TaskEnqueued;
        private event EventHandler<Task> TaskFinished;

        public WorkPool(int _maxThreads)
        {
            workers = new Task[maxThreads];
            MaxThreads = _maxThreads;

            TaskEnqueued += WorkPool_TaskEnqueued;
            TaskFinished += WorkPool_TaskFinished;
        }

        public Task Enqueue(Action action)
        {
            var task = new Task(action);
            Enqueue(task);
            return task;
        }
        public Task<T> Enqueue<T>(Func<T> action)
        {
            var task = new Task<T>(() => action());
            Enqueue(task);
            return task;
        }
        public void Enqueue(Task _task)
        {
            _task.ContinueWith(t => TaskFinished?.Invoke(this, t));
            lock (taskList)
            {
                taskList.Enqueue(_task);
            }
            TaskEnqueued?.Invoke(this, _task);
        }
        public void Enqueue(Task[] _tasks)
        {
            for(int i = 0; i < _tasks.Length; i++)
            {
                Task _task = _tasks[i];
                _task.ContinueWith(t => TaskFinished?.Invoke(this, t));
                lock (taskList)
                {
                    taskList.Enqueue(_task);
                }
                TaskEnqueued?.Invoke(this, _task);
            }
        }

        public Task ForToTask(int fromInclusive, int toExclusive, Action body) => Task.WhenAll(For(fromInclusive, toExclusive, body));
        public Task ForToTask(int fromInclusive, int toExclusive, Action<int> body) => Task.WhenAll(For(fromInclusive, toExclusive, body));
        public Task[] For(int fromInclusive, int toExclusive, Action body)
        {
            int count = toExclusive - fromInclusive;
            Task[] tasks = new Task[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                var t = Enqueue(body);
                tasks[counter++] = t;
            }
            return tasks;
        }
        public Task[] For(int fromInclusive, int toExclusive, Action<int> body)
        {
            int count = toExclusive - fromInclusive;
            Task[] tasks = new Task[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                int i2 = i;
                var t = Enqueue(() => body(i2));
                tasks[counter++] = t;
            }
            return tasks;
        }
        public Task<T>[] For<T>(int fromInclusive, int toExclusive, Func<T> body)
        {
            int count = toExclusive - fromInclusive;
            Task<T>[] tasks = new Task<T>[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                var t = Enqueue(() => body());
                tasks[counter++] = t;
            }
            return tasks;
        }
        public Task<T>[] For<T>(int fromInclusive, int toExclusive, Func<int, T> body)
        {
            int count = toExclusive - fromInclusive;
            Task<T>[] tasks = new Task<T>[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                int i2 = i;
                var t = Enqueue(() => body(i2));
                tasks[counter++] = t;
            }
            return tasks;
        }

        private void CheckScheduling()
        {
            int worker = -1;
            Task nextTask = Task.CompletedTask;
            lock (workers)
            {
                for (int i = 0; i < workers.Length; i++)
                {
                    if (workers[i] == null || workers[i].IsCompleted || workers[i].IsCanceled || workers[i].IsFaulted)
                    {
                        worker = i;
                        break;
                    }
                }

                if (worker > -1)
                {
                    lock (taskList)
                    {
                        if (taskList.Count > 0)
                        {
                            try
                            {
                                nextTask = taskList.Dequeue();
                                workers[worker] = nextTask;
                                nextTask.Start();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
        }

        private void WorkPool_TaskEnqueued(object sender, Task e)
        {
            CheckScheduling();
        }

        private void WorkPool_TaskFinished(object sender, Task e)
        {
            //Console.WriteLine($"Finished: {e.Id}");
            CheckScheduling();
        }
    }
}
