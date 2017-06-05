using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RSSforTwitterCore2.ServiceInterface.CustomTypes
{
    public class FixedSizedQueue<T> : Queue<T>
    {
        public int Limit { get; }

        public FixedSizedQueue(int limitSize)
        {
            Limit = limitSize;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            while (Count > Limit) base.Dequeue();
        }
    }
}
