using UnityEngine;

namespace __Project.Systems.DamageNumberSystem
{
    public struct NumberInfo
    {
        public NumberTypes numberType;
        public Vector3 position;
        public float damage;
        public string spamID;
        public Transform followTransform;
        public bool isCritical;
    }
}