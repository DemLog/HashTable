using System;
using System.Collections;
using System.Collections.Generic;

namespace HashTable.Dictionary
{
    public class LinkedList<T> : IEnumerable<T>
    {
        private Node<T> _head;
        private Node<T> _tail;

        public LinkedList()
        {
            _head = null;
            _tail = null;
            Count = 0;
        }

        public int Count { get; private set; }

        public void Add(T data)
        {
            Node<T> node = new Node<T>(data);

            if (_head == null)
                _head = node;
            else
                _tail.Next = node;
            _tail = node;

            Count++;
        }

        public bool Remove(T data)
        {
            Node<T> current = _head;
            Node<T> previous = null;

            while (current != null)
            {
                if (current.Data.Equals(data))
                {
                    if (previous != null)
                    {
                        previous.Next = current.Next;
                        if (current.Next == null)
                            _tail = previous;
                    }
                    else
                    {
                        _head = _head.Next;
                        if (_head == null)
                            _tail = null;
                    }

                    Count--;
                    return true;
                }

                previous = current;
                current = current.Next;
            }

            return false;
        }

        public bool IsEmpty => Count == 0;

        public void Clear()
        {
            _head = null;
            _tail = null;
            Count = 0;
        }

        public bool Contains(T data)
        {
            Node<T> current = _head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                    return true;
                current = current.Next;
            }

            return false;
        }

        public void AppendFirst(T data)
        {
            Node<T> node = new Node<T>(data);
            node.Next = _head;
            _head = node;
            if (Count == 0)
                _tail = _head;
            Count++;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            Node<T> current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }
}