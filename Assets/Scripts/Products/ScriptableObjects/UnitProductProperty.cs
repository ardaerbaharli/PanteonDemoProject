using System;
using MapEntities.Units;
using UnityEngine;

namespace Products.ScriptableObjects
{
    [CreateAssetMenu]
    [Serializable]
    public class UnitProductProperty : ProductProperty
    {
        public UnitType unitType;
        public float damage;
        public float attackInterval;
        public string level;
        public float speed;
    }
}