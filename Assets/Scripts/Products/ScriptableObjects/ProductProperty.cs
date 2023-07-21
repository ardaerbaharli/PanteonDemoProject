using UnityEngine;

namespace Products.ScriptableObjects
{
    public class ProductProperty : ScriptableObject
    {
        public ProductType type;
        public Vector2 size;
        public Sprite sprite;
        public float health;
    }
}