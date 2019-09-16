using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class FixedSizedQueue<T> : IEnumerable {
    private Queue<T> queue = new Queue<T>();
    
    public int Limit { get; set; }
    public void Enqueue(T obj)
    {
        queue.Enqueue(obj);
        while (queue.Count > Limit) {
            queue.Dequeue();
        } 
    }

    public IEnumerator GetEnumerator() {
        throw new System.NotImplementedException();
    }
}