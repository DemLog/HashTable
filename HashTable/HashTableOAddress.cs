using System;
using System.Collections.Generic;
using System.Linq;
using HashTable.HashFunctions;

namespace HashTable
{
    public sealed class HashTableOAddress<TKey, TValue>
    {
        private readonly int _size;
        private readonly KeyValuePair<TKey, TValue?>[] _items;
        private readonly bool[] _removed;
        private readonly HashProbingType _hashProbingType;
        private readonly Func<object, int, int> _hashFuncType;
        private readonly Func<object, int, int> _hashFuncTypeDouble;
        public int Count { get; private set; }

        private static readonly Func<Func<object, int, int>, object, int, int, int> LinearHashing =
            (f, key, sizeHashTable, index) => (f(key, sizeHashTable) + index) % sizeHashTable;

        private static readonly Func<Func<object, int, int>, object, int, int, int> QuadraticHashing =
            (f, key, sizeHashTable, index) => (f(key, sizeHashTable) + (int) Math.Pow(index, 2)) % sizeHashTable;

        private static readonly Func<Func<object, int, int>, Func<object, int, int>, object, int, int, int>
            DoubleHashing =
                (f1, f2, key, sizeHashTable, index) =>
                    (f1(key, sizeHashTable) + index * f2(key, sizeHashTable)) % sizeHashTable;

        public HashTableOAddress(int size, HashProbingType hashProbingType, params HashFuncType[] hashFuncTypes)
        {
            if (!(size > 0)) throw new AggregateException(nameof(size));
            _size = size;
            _hashProbingType = hashProbingType;
            _hashFuncType = HashFunc.GetHashFunc(hashFuncTypes[0]);
            _hashFuncTypeDouble = HashFunc.GetHashFunc(HashFuncType.Multi);
            if (hashFuncTypes.Length > 1)
                _hashFuncTypeDouble = HashFunc.GetHashFunc(hashFuncTypes[1]);
            _items = new KeyValuePair<TKey, TValue?>[size];
            _removed = new bool[_size];
        }

        public HashTableOAddress(int size, HashProbingType hashProbingType)
        {
            if (!(size > 0)) throw new AggregateException(nameof(size));
            _size = size;
            _hashProbingType = hashProbingType;
            _hashFuncType = HashFunc.GetHashFunc(HashFuncType.Div);
            _hashFuncTypeDouble = HashFunc.GetHashFunc(HashFuncType.Multi);
            _items = new KeyValuePair<TKey, TValue?>[size];
            _removed = new bool[_size];
        }

        public HashTableOAddress()
        {
            _size = 1000;
            _hashProbingType = HashProbingType.Linear;
            _hashFuncType = HashFunc.GetHashFunc(HashFuncType.Div);
            _hashFuncTypeDouble = HashFunc.GetHashFunc(HashFuncType.Multi);
            _items = new KeyValuePair<TKey, TValue?>[_size];
            _removed = new bool[_size];
        }

        public int MaxClusterLength
        {
            get
            {
                var max = 0;
                var current = 0;
                foreach (var item in _items)
                {
                    if (!item.Equals(default(KeyValuePair<TKey, TValue>)))
                    {
                        current++;
                    }
                    else
                    {
                        max = Math.Max(max, current);
                        current = 0;
                    }
                }

                return Math.Max(max, current);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (!CheckOpenSpace()) throw new ArgumentOutOfRangeException("Хеш-таблица переполнена.");

            if (!CheckUniqueKey(key)) throw new ArgumentException("Элемент по указанному ключу уже существует.");

            Insert(key, value);
        }

        private void Insert(TKey key, TValue value)
        {
            var index = 0;
            var hashCode = GetHash(key, index);

            while (!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            _items[hashCode] = new KeyValuePair<TKey, TValue?>(key, value);
            _removed[hashCode] = false;
            Count++;
        }

        public TValue? GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var index = 0;
            var hashCode = GetHash(key, index);

            while ((!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) || _removed[hashCode]) &&
                   !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            return _items[hashCode].Value;
        }

        public bool Remove(TKey key)
        {
            var index = 0;
            var hashCode = GetHash(key, index);

            while ((!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) || _removed[hashCode]) &&
                   !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            if (_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)))
            {
                return false;
            }
            else
            {
                _items[hashCode] = default;
                _removed[hashCode] = true;
                Count--;
                return true;
            }
        }

        private bool CheckOpenSpace()
        {
            var isOpen = false;
            for (var i = 0; i < _size; i++)
            {
                if (_items[i].Equals(default(KeyValuePair<TKey, TValue?>))) isOpen = true;
            }

            return isOpen;
        }

        private bool CheckUniqueKey(TKey key) => !_items.Any(x => x.Key != null && x.Key.Equals(key));
        // {
        //     foreach (var item in _items)
        //     {
        //         if (item.Key != null && item.Key.Equals(key)) return false;
        //     }
        //
        //     return true;
        // }

        private int GetHash(TKey key, int index)
        {
            return _hashProbingType switch
            {
                HashProbingType.Linear =>
                    LinearHashing(_hashFuncType, key, _size, index),
                HashProbingType.Quadratic => QuadraticHashing(_hashFuncType, key, _size,
                    index),
                HashProbingType.Double => DoubleHashing(_hashFuncType,
                    _hashFuncTypeDouble, key, _size, index),
                _ => LinearHashing(_hashFuncType, key, _size, index)
            };
        }
    }
}