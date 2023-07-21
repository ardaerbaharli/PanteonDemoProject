using System;
using System.Collections.Generic;
using System.Linq;
using MapEntities.Buildings;
using MapEntities.Units;

namespace Products
{
    public static class ProductFactory
    {
        private static Dictionary<Type, BuildingType> _buildingFactoryDictionary;

        private static Dictionary<Type, UnitType> _unitFactoryDictionary;

        public static void Initialize()
        {
            SetUnitFactoryDictionary();
            SetBuildingFactoryDictionary();
        }

        private static void SetUnitFactoryDictionary()
        {
            var unitTypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>();
            var unitFactoryTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(Unit).IsAssignableFrom(p) && !p.IsInterface).ToList();

            _unitFactoryDictionary = new Dictionary<Type, UnitType>();
            foreach (var unitType in unitTypes)
            {
                var unitFactoryType = unitFactoryTypes.FirstOrDefault(t => t.Name == unitType.ToString());
                if (unitFactoryType != null) _unitFactoryDictionary.Add(unitFactoryType, unitType);
            }
        }

        private static void SetBuildingFactoryDictionary()
        {
            var buildingTypes = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>();
            var buildingFactoryTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(Building).IsAssignableFrom(p) && !p.IsInterface).ToList();

            _buildingFactoryDictionary = new Dictionary<Type, BuildingType>();
            foreach (var buildingType in buildingTypes)
            {
                var buildingFactoryType = buildingFactoryTypes.FirstOrDefault(t => t.Name == buildingType.ToString());
                if (buildingFactoryType != null) _buildingFactoryDictionary.Add(buildingFactoryType, buildingType);
            }
        }

        public static Unit GetUnitTypeFromFactory(UnitType type)
        {
            var unitFactoryType = _unitFactoryDictionary.FirstOrDefault(x => x.Value == type).Key;
            return (Unit) Activator.CreateInstance(unitFactoryType);
        }

        public static Building GetBuildingTypeFromFactory(BuildingType type)
        {
            var buildingFactoryType = _buildingFactoryDictionary.FirstOrDefault(x => x.Value == type).Key;
            return (Building) Activator.CreateInstance(buildingFactoryType);
        }
    }
}