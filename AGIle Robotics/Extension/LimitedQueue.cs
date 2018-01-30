using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Extension
{
    public class LimitedQueue<T> : ConcurrentQueue<T>
    {
        public int Limit { get => limit; private set => limit = value; }
        private int limit;

        protected object lockObject = new object();

        public LimitedQueue(int limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (lockObject)
            {
                while (base.Count > Limit)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }

    }
}
