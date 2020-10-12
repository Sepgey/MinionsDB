using System.Collections.Generic;
using System.Collections;
using System;

namespace LinkedList
{
    public class Program
    {
        static void Main(string[] args)
        {
            DoublyLinkedList<Minion> minion = new DoublyLinkedList<Minion>();
{
            minion.Add(new Minion(1, "Kevin", 14, 3));
            minion.Add(new Minion(2, "Bob", 23, 2));
            minion.Add(new Minion(3, "Stuart", 21, 1));
}
            minion.AddFirst(new Minion(4, "Mark", 20, 4));
            foreach (var item in minion)
            {
                Console.WriteLine(item);
            }

            minion.Remove(2);

            foreach (var t in minion.BackEnumerator())
            {
                Console.WriteLine(t);
            }
        }
    }
    public class Minion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int TownId { get; set; }

        public Minion(int id, string name, int age, int townId)
        {
            Id = id;
            Name = name;
            Age = age;
            TownId = townId;
        }

        public override string ToString()
        {
            return
            $"Id: {Id}, Name: {Name}, Age: {Age}, TownId: {TownId}";
        }
    }

    public class DoublyNode<T>
    {
        public DoublyNode(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
        public DoublyNode<T> Previous { get; set; }
        public DoublyNode<T> Next { get; set; }

        public override string ToString()
        {
            return $"Data: {Data}";
        }
    }

    public class DoublyLinkedList<T> : IEnumerable<T>
    {
        DoublyNode<T> head;
        DoublyNode<T> tail;
        int count;

        public void Add(T data)
        {
            DoublyNode<T> node = new DoublyNode<T>(data);

            if (head == null)
                head = node;
            else
            {
                tail.Next = node;
                node.Previous = tail;
            }
            tail = node;
            count++;
        }

        public void AddFirst(T data)
        {
            DoublyNode<T> node = new DoublyNode<T>(data);
            DoublyNode<T> temp = head;
            node.Next = temp;
            head = node;
            if (count == 0)
                tail = head;
            else
                temp.Previous = node;
            count++;
        }

        private DoublyNode<T> GetNode(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException($"Index {index} is invalid; Only {Count} elements exist");

            DoublyNode<T> current = head;
            int t = 0;
            while (t < index)
            {
                current = current.Next;
                t++;
            }


            return current;
        }

        public T Get(int index)
        {
            return GetNode(index).Data;
        }

        public void Remove(int index)
        {
            var node = GetNode(index);

            if (head == node)
            {
                if (head.Next != null)
                {
                    head.Next.Previous = null;
                    head = head.Next;
                }
                else
                {
                    head = null;
                }
            }
            else if (tail == node)
            {
                if (tail.Previous != null)
                {
                    tail.Previous.Next = null;
                    tail = tail.Previous;
                }
                else
                {
                    tail = null;
                }
            }
            else
            {
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;
            }

            count--;
        }

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public bool Contains(T data)
        {
            DoublyNode<T> current = head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                    return true;
                current = current.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            DoublyNode<T> current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        public void Set(T data, int index)
        {
            GetNode(index).Data = data;
        }

        public IEnumerable<T> BackEnumerator() // перебор элементов списка с конца
        {
            DoublyNode<T> current = tail;
            while (current != null)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }
    }
}
