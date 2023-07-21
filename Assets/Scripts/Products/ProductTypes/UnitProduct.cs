using Grid;
using MapEntities;
using MapEntities.Buildings.Interfaces;
using MapEntities.Units;
using Products.ScriptableObjects;
using UI;
using UI.Menus;
using UnityEngine;
using Utilities;

namespace Products.ProductTypes
{
    public class UnitProduct : Product
    {
        public IProduceUnits producer;

        public override void CreateProduct()
        {
            var unitProperty = productProperty as UnitProductProperty;
            var unitProduct = (Unit) Create(unitProperty, PooledObjectType.Unit);

            // the unit should be created via the corresponding building
            // if returns false, it means that the building does not have enough space
            var producingBuilding = producer;
            if (!producingBuilding.CreateUnit(unitProduct))
            {
                unitProduct.ReturnToPool();
                GridManager.Instance.ResetAllTileColors();
                MenuController.Instance.HideMenu(MenuType.InformationMenu);
            }
        }


        protected override MapEntity AddComponent(GameObject o, ProductProperty productProductProperty)
        {
            var unitType = ((UnitProductProperty) productProductProperty).unitType;
            var unit = ProductFactory.GetUnitTypeFromFactory(unitType);
            o.TryGetComponent(out Unit unitComponent);
            if (unitComponent != null) return unitComponent;
            return o.AddComponent(unit.GetType()) as Unit;
        }
    }
}