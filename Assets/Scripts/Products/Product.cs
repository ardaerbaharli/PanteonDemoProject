using MapEntities;
using Products.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Products
{
    public abstract class Product : MonoBehaviour
    {
        public ProductProperty productProperty;
        public PooledObject pooledObject;
        public ProductType type;
        private Image _image;
        private TextMeshProUGUI _nameText;


        private void Awake()
        {
            _image = GetComponent<Image>();
            _nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ReturnToPool()
        {
            pooledObject.ReturnToPool();
        }

        public void Initialize(ProductProperty property, string name)
        {
            productProperty = property;
            _image.sprite = property.sprite;
            type = property.type;
            _nameText.text = name;
        }

        public abstract void CreateProduct();

        protected MapEntity Create(ProductProperty productProductProperty, PooledObjectType pooledObjectType)
        {
            var mapEntityPooledObject = ObjectPool.Instance.GetPooledObject(pooledObjectType);
            var mapEntity = AddComponent(mapEntityPooledObject.gameObject, productProductProperty);
            mapEntity.pooledObject = mapEntityPooledObject;
            mapEntity.gameObject.SetActive(true);
            mapEntity.Initialize(productProductProperty);

            return mapEntity;
        }

        protected abstract MapEntity AddComponent(GameObject o, ProductProperty productProductProperty);
    }
}