using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGIle_Robotics
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

        public void EnqueueTask(Task _task)
        {
            taskList.Enqueue(_task);
            TaskEnqueued?.Invoke(this, _task);
        }

        private void CheckScheduling()
        {
            int worker = -1;
            Task nextTask;
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
                            nextTask = taskList.Dequeue();
                            workers[worker] = nextTask;
                            nextTask.Start();
                            nextTask.ContinueWith(t => TaskFinished?.Invoke(this, t));
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
