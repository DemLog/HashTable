using System;
using System.Collections.Generic;
using System.Linq;
using HashTable.HashFunctions;

namespace HashTable
{
    public sealed class HashTableChained<TKey, TValue>
    {
        private readonly int _size;
        private readonly Dictionary.LinkedList<KeyValuePair<TKey, TValue?>>[] _items;
        private readonly Func<object, int, int> _hashFuncType;
        public int Count { get; private set; }

        public HashTableChained(int size, HashFuncType hashFuncType)
        {
            if (!(size > 0)) throw new AggregateException(nameof(size));
            _size = size;
            _hashFuncType = HashFunc.GetHashFunc(hashFuncType);
            _items = new Dictionary.LinkedList<KeyValuePair<TKey, TValue?>>[size];
        }

        public HashTableChained(int size)
        {
            if (!(size > 0)) throw new AggregateException(nameof(size));
            _size = size;
            _hashFuncType = HashFunc.GetHashFunc(HashFuncType.Div);
            _items = new Dictionary.LinkedList<KeyValuePair<TKey, TValue?>>[size];
        }

        public HashTableChained()
        {
            _size = 1000;
            _hashFuncType = HashFunc.GetHashFunc(HashFuncType.Div);
            _items = new Dictionary.LinkedList<KeyValuePair<TKey, TValue?>>[_size];
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var item = new KeyValuePair<TKey, TValue?>(key, value);
            Insert(item);
        }

        public void SetValue(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);

            var foundItem = false;
            foreach (KeyValuePair<TKey, TValue?> item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    foundItem = true;
                    if (item.Value == null || !item.Value.Equals(value))
                    {
                        linkedList.Remove(item);
                        linkedList.Add(new KeyValuePair<TKey, TValue?>(key, value));
                    }

                    break;
                }
            }

            if (!foundItem) throw new ArgumentOutOfRangeException(nameof(key));
        }

        public bool Remove(TKey key)
        {
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);
            var itemFound = false;
            var foundItem = default(KeyValuePair<TKey, TValue?>);
            foreach (KeyValuePair<TKey, TValue?> item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    itemFound = true;
                    foundItem = item;
                }
            }

            if (itemFound)
            {
                linkedList.Remove(foundItem);
                Count--;
            }

            return itemFound;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);

            var foundItem = false;
            foreach (KeyValuePair<TKey, TValue?> item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    foundItem = true;
                    break;
                }
            }

            return foundItem;
        }

        public TValue? GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);
            foreach (KeyValuePair<TKey, TValue?> item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key)) return item.Value;
            }

            return default;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                for (var i = 0; i < _items.Length; i++)
                {
                    _items[i] = null;
                }
            }
        }

        private void Insert(KeyValuePair<TKey, TValue?> item)
        {
            var position = GetListPosition(item.Key);
            var linkedList = GetLinkedList(position);

            foreach (KeyValuePair<TKey, TValue?> pair in linkedList)
            {
                if (pair.Key != null && pair.Key.Equals(item.Key))
                    throw new ArgumentException("Элемент по указанному ключу уже существует.");
            }

            linkedList.Add(item);
            Count++;
        }

        private int GetListPosition(TKey key)
        {
            return _hashFuncType.Invoke(key, _size);
        }

        private Dictionary.LinkedList<KeyValuePair<TKey, TValue?>> GetLinkedList(int position)
        {
            var linkedList = _items[position];
            if (linkedList is null)
            {
                linkedList = new Dictionary.LinkedList<KeyValuePair<TKey, TValue?>>();
                _items[position] = linkedList;
            }

            return linkedList;
        }

        public double FillFactor => (double) Count / _size;
        public int MaxLengthChain => _items.Max(x => x?.Count ?? 0);
        public int MinLengthChain => _items.Min(x => x?.Count ?? 0);
    }
}