using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PlanetbaseMultiplayer.Model.Collections
{
    // Since the game does not run on .NET Framework 4+, we have to reinvent the wheel
    // Also the mono version Planetbase is using does not have the methods required to lock and unlock objects. Great!
    public class ConcurrentQueue<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private bool locked;

        private void WaitForUnlock()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            new Thread(() =>
            {
                while (locked) { Thread.Sleep(1); }
                mre.Set();
            }).Start();
            mre.WaitOne();
        }

        public void Lock(bool waitForUnlock = true)
        {
            if (locked && waitForUnlock)
                WaitForUnlock();
            locked = true;
        }

        public void Unlock()
        {
            if (locked)
                locked = false;
        }

        public ConcurrentQueue()
        {
            _queue = new Queue<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            Lock();
            foreach (var item in _queue)
            {
                yield return item;
            }
            Unlock();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            Lock();
            ((ICollection)_queue).CopyTo(array, index);
            Unlock();
        }

        public int Count
        {
            get
            {
                // Assumed to be atomic, so locking is unnecessary
                return _queue.Count;
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public void Enqueue(T item)
        {
            Lock();
            _queue.Enqueue(item);
            Unlock();
        }

        public T Dequeue()
        {
            Lock();
            T obj = _queue.Dequeue();
            Unlock();
            return obj;
        }

        public T Peek()
        {
            Lock();
            T obj = _queue.Peek();
            Unlock();
            return obj;
        }

        public void Clear()
        {
            Lock();
            _queue.Clear();
            Unlock();
        }
    }
}
