using System;
using UnityEngine;

namespace NueGames.NTooltip
{
    [Serializable]
    public class KV<K, V>
    {
        [SerializeField] private K key;
        [SerializeField] private V value;

        public K Key
        {
            get { return key; }
            set { key = value; }
        }

        public V Value
        {
            get => value;
            set => this.value = value;
        }

        public KV(K key, V value)
        {
            this.key = key;
            this.value = value;
        }
    }
}