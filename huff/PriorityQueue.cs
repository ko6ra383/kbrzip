using System.Collections.Generic;

namespace huff {
    public class PriorityQueue<T> {
        int size;
        SortedDictionary<int, Queue<T>> storage;
        public PriorityQueue() {
            storage = new SortedDictionary<int, Queue<T>>();
            size = 0;
        }
        public int Size() => size;
        public void Enqueue(int priority, T item) { // добавление в очередь
            if (!storage.ContainsKey(priority)) 
                storage.Add(priority, new Queue<T>());
            storage[priority].Enqueue(item);
            size++;
            
        }

        public T Dequeue() { // берем элемент
            if(size==0) throw new System.Exception("Queue is empty");
            size--;
            foreach(Queue<T>q in storage.Values) { // уже идут в порядке приоритета
                if (q.Count > 0) return q.Dequeue();
            }
            throw new System.Exception("Queue error");
        }
    }
}