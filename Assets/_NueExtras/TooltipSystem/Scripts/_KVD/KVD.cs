﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NueGames.NTooltip
{
    [Serializable]
    public class KVD<K, V> : IEnumerable<KV<K, V>>
    {
        [SerializeField] private List<KV<K, V>> keyValuePairs = new List<KV<K, V>>();
        private System.Random _random = new System.Random();
        

        public V this[K key]
        {
            get
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (kvp.Key.Equals(key))
                        return kvp.Value;
                }

                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
            set
            {
                for (int i = 0; i < keyValuePairs.Count; i++)
                {
                    if (keyValuePairs[i].Key.Equals(key))
                    {
                        keyValuePairs[i].Value = value;
                        return;
                    }
                }

                keyValuePairs.Add(new KV<K, V>(key, value));
            }
        }

        public void Add(K key, V value)
        {
            if (ContainsKey(key))
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
            keyValuePairs.Add(new KV<K, V>(key, value));
        }

        public void AddRange(IEnumerable<KV<K, V>> keyValuePairsToAdd)
        {
            foreach (var kvp in keyValuePairsToAdd)
            {
                Add(kvp.Key, kvp.Value);
            }
        }
        
        public KV<K, V> LastOrDefault()
        {
            return keyValuePairs.LastOrDefault();
        }

        public bool Remove(K key)
        {
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                if (keyValuePairs[i].Key.Equals(key))
                {
                    keyValuePairs.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(K key)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Key.Equals(key))
                    return true;
            }

            return false;
        }

        public bool TryGetValue(K key, out V value)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Key.Equals(key))
                {
                    value = kvp.Value;
                    return true;
                }
            }

            value = default(V);
            return false;
        }

        public void Clear()
        {
            keyValuePairs.Clear();
        }

        public int Count => keyValuePairs.Count;
        
        public KV<K, V> GetRandom()
        {
            if (keyValuePairs.Count == 0)
                throw new InvalidOperationException("Kvp is empty.");

            int randomIndex = _random.Next(0, keyValuePairs.Count);
            return keyValuePairs[randomIndex];
        }

        public IEnumerator<KV<K, V>> GetEnumerator()
        {
            return keyValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}