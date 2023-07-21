using System.Collections.Generic;
using MapEntities.Buildings;
using NaughtyAttributes;
using UnityEngine;

namespace Products.ScriptableObjects
{
    [CreateAssetMenu]
    public class BuildingProductProperty : ProductProperty
    {
        public BuildingType buildingType;
        public bool producingUnits;
        [ShowIf("producingUnits")] public List<UnitProductProperty> unitProductProperties;
    }
}