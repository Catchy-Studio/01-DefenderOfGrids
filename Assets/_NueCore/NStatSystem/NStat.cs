using System;

namespace _NueCore.NStatSystem
{
    [Serializable]
    public class NStat
    {
        public string key;
        public float value;
        public int statCategory;
        public NStatCategory GetStatCategory()
        {
            return (NStatCategory)statCategory;
        }
        public static NStat operator +(NStat a, NStat b)
        {
            if (a.key != b.key)
            {
                throw new InvalidOperationException("Cannot add NStats with different keys");
            }

            return new NStat
            {
                key = a.key,
                value = a.value + b.value,
                statCategory = a.statCategory
            };
        }
    }
}