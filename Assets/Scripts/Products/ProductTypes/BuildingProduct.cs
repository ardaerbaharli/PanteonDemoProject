using MapEntities;
using MapEntities.Buildings;
using Products.ScriptableObjects;
using UI;
using UI.Menus;
using UnityEngine;
using Utilities;

namespace Products.ProductTypes
{
    public class BuildingProduct : Product
    {
        public override void CreateProduct()
        {
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);

            // create and start dragging the product
            var buildingProperty = productProperty as BuildingProductProperty;
            var buildingProduct = (Building) Create(buildingProperty, PooledObjectType.Building);
            ProductManager.Instance.buildingProduct = buildingProduct;
            PlayerInputManager.Instance.isDragging = true;
        }

        protected override MapEntity AddComponent(GameObject o, ProductProperty productProductProperty)
        {
            var buildingType = ((BuildingProductProperty) productProductProperty).buildingType;
            var building = ProductFactory.GetBuildingTypeFromFactory(buildingType);
            o.TryGetComponent(out Building buildingComponent);
            if (buildingComponent != null) return buildingComponent;
            return o.AddComponent(building.GetType()) as Building;
        }
    }
}