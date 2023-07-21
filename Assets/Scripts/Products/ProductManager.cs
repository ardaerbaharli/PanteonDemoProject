using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using MapEntities.Buildings;
using MapEntities.Buildings.Interfaces;
using Products.ProductTypes;
using Products.ScriptableObjects;
using UI;
using UI.Menus;
using UnityEngine;
using Utilities;

namespace Products
{
    public class ProductManager : MonoBehaviour
    {
        [SerializeField] private string productPropertiesResourcesPath = "Products";

        [Space] [Header("Layer Masks")] [SerializeField]
        private LayerMask productLayerMask;

        public Building buildingProduct;
        private List<GridTile> _currentTiles;

        private bool _isBuildingProductOnValidPosition;
        private ProductProperty[] _products;

        private Building _selectedBuilding;
        public static ProductManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _products = Resources.LoadAll<ProductProperty>(productPropertiesResourcesPath);
            ProductFactory.Initialize();
        }


        private IEnumerator Start()
        {
            yield return new WaitUntil(() => ObjectPool.Instance.isPoolSet);

            PlayerInputManager.Instance.onLeftButtonDown += CheckProductClick;
            PlayerInputManager.Instance.onDragging += HandleDraggingBuilding;
            PlayerInputManager.Instance.onLeftButtonUp += HandleDroppingBuilding;
            MapEntityInputHandler.Instance.onSelectedBuilding += HandleSelectedBuilding;


            InitializeProductMenu();
        }

        private void HandleSelectedBuilding(Building selectedBuilding)
        {
            _selectedBuilding = selectedBuilding;
            InitializeInformationMenu(selectedBuilding);
        }

        private void InitializeInformationMenu(Building selectedBuilding)
        {
            // Get the products from pool, initialize them and set them active
            // Note: they are already in the product menu scroll view content

            // Clear the information menu before initializing the new products
            var informationMenu =
                (InformationMenu) MenuController.Instance.GetMenu(MenuType.InformationMenu);
            informationMenu.Clear();

            var buildingProperty = selectedBuilding.buildingProperty;
            var buildingName = buildingProperty.buildingType.ToString();
            var buildingSprite = buildingProperty.sprite;

            if (buildingProperty.producingUnits)
            {
                var productsToShow = CreateProductsToShow(selectedBuilding.buildingProperty.unitProductProperties);
                informationMenu.Initialize(buildingName, buildingSprite, productsToShow);
            }
            else
            {
                informationMenu.Initialize(buildingName, buildingSprite);
            }

            MenuController.Instance.ShowMenu(MenuType.InformationMenu);
        }


        private void InitializeProductMenu()
        {
            // Get the products from pool, initialize them and set them active
            // Note: they are already in the product menu scroll view content
            var productProperties = _products.Where(x => x.type == ProductType.Building);
            foreach (var p in productProperties)
            {
                var pooledObject = ObjectPool.Instance.GetPooledObject(PooledObjectType.BuildingProduct);
                pooledObject.gameObject.TryGetComponent(out BuildingProduct product);
                if (product == null) product = pooledObject.gameObject.AddComponent<BuildingProduct>();
                product.pooledObject = pooledObject;
                product.gameObject.SetActive(true);
                var buildingName = ((BuildingProductProperty) p).buildingType.GetDescription();
                product.Initialize(p, buildingName);
            }
        }

        public List<Product> CreateProductsToShow(List<UnitProductProperty> unitProductProperties)
        {
            // Get the products from pool, initialize them and set them active
            // Note: they are already in the information menu scroll view content

            var productsToShow = new List<Product>();
            foreach (var property in unitProductProperties)
            {
                var pooledObject = ObjectPool.Instance.GetPooledObject(PooledObjectType.UnitProduct);
                pooledObject.gameObject.TryGetComponent(out UnitProduct product);
                if (product == null) product = pooledObject.gameObject.AddComponent<UnitProduct>();
                product.pooledObject = pooledObject;
                product.gameObject.SetActive(true);

                // make it the last child so it will be always on same order
                product.transform.SetAsLastSibling();
                var unitName = $"Level {property.level} {property.unitType.GetDescription()}";

                product.Initialize(property, unitName);
                productsToShow.Add(product);
            }

            return productsToShow;
        }


        public void CheckProductClick(Vector3 clickPosition)
        {
            if (!MenuController.Instance.IsMenuShown(MenuType.ProductionMenu) &&
                !MenuController.Instance.IsMenuShown(MenuType.InformationMenu)) return;

            HandleProductClicks(clickPosition);
        }

        private void HandleProductClicks(Vector3 position)
        {
            // send a ray at the mouse position on productLayerMask
            var productHit = Physics2D.Raycast(position, Vector2.zero, 0, productLayerMask);
            if (productHit.collider == null) return;
            var product = productHit.collider.TryGetComponent<Product>(out var p) ? p : null;

            // Create real version of the product and move that
            CreateProduct(product);
        }

        private void CreateProduct(Product product)
        {
            if (product.type == ProductType.Unit)
            {
                var unitProduct = (UnitProduct) product;
                unitProduct.producer = (IProduceUnits) _selectedBuilding;
            }

            product.CreateProduct();
        }


        private void HandleDraggingBuilding(Vector3 position)
        {
            buildingProduct.transform.localPosition = position;
            _isBuildingProductOnValidPosition =
                GridManager.Instance.Hover(position, buildingProduct.size, out _currentTiles);
        }

        private void HandleDroppingBuilding()
        {
            if (buildingProduct == null) return;

            PlayerInputManager.Instance.isDragging = false;
            if (_isBuildingProductOnValidPosition)
            {
                if (buildingProduct == null) return;
                buildingProduct.Build(_currentTiles);
                _isBuildingProductOnValidPosition = false;
                buildingProduct = null;
                _currentTiles.Clear();
            }
            else
            {
                buildingProduct.ReturnToPool();
                GridManager.Instance.ResetAllTileColors();
            }
        }
    }
}