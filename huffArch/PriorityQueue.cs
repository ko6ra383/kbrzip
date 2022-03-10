using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huffArch {
    class PriorityQueue<T> {
        public int Size { get; private set; }
        SortedDictionary<int, Queue<T>> que;
        public PriorityQueue() {
            Size = 0;
            que = new SortedDictionary<int, Queue<T>>();
        }
        public void Enqueue(int freq, T q) {
            if (!que.ContainsKey(freq)) {
                que.Add(freq, new Queue<T>());
            }
            que[freq].Enqueue(q);
            Size++;
        }
        public T Dequeue() {
            Size--;
            return (que.Where(x => x.Value.Count > 0).Select(x => x.Value.Dequeue()).First());
        }
    }
}
