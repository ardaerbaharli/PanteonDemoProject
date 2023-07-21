using System.Collections.Generic;
using System.Linq;
using Grid;
using MapEntities.Buildings.Interfaces;
using MapEntities.Units;
using UnityEngine;
using Utilities;

namespace MapEntities.Buildings.BuildingTypes
{
    public class Barracks : Building, IProduceUnits
    {
        private List<int> _availableSlotIndexes, _unavailableSlotIndexes;
        private int _maxIndex;
        private List<PooledObject> _unitSlots;
        private List<GridTile> _unitTiles;

        public bool CreateUnit(Unit unit)
        {
            if (_availableSlotIndexes.Count == 0) return false;

            var unitSlotIndex = _availableSlotIndexes[0];

            var unitTile = _unitTiles[unitSlotIndex];
            var unitTileTransform = unitTile.transform;

            unit.transform.position = unitTileTransform.position;
            unit.positionTile = unitTile;
            unit.isOnSpawn = true;

            _unavailableSlotIndexes.Add(unitSlotIndex);
            _availableSlotIndexes.RemoveAt(0);

            unit.onLeftSpawn += () => _availableSlotIndexes.Add(unitSlotIndex);
            unit.onDestroyed += () =>
            {
                if (unit.isOnSpawn)
                    _availableSlotIndexes.Add(unitSlotIndex);
            };
            return true;
        }


        public override void Build(List<GridTile> currentTiles)
        {
            base.Build(currentTiles);
            _availableSlotIndexes = new List<int>();
            _unavailableSlotIndexes = new List<int>();
            _unitSlots = new List<PooledObject>();
            _unitTiles = new List<GridTile>();

            _maxIndex = edgeTiles.Count;
            _availableSlotIndexes = Enumerable.Range(0, _maxIndex).ToList();

            // create unit slots around the building
            foreach (var outerTile in edgeTiles)
            {
                var unitSlot = ObjectPool.Instance.GetPooledObject(PooledObjectType.UnitSlot);
                unitSlot.transform.position =
                    outerTile.transform.position -
                    new Vector3(0, outerTile.size.x * (1 - unitSlot.transform.localScale.x), 0);
                unitSlot.transform.SetParent(transform);
                unitSlot.gameObject.SetActive(true);
                _unitSlots.Add(unitSlot);
                _unitTiles.Add(outerTile);
            }

            print("Barracks built");
        }

        public override void DestroyEntity()
        {
            _unitSlots.ForEach(slot => slot.ReturnToPool());
            base.DestroyEntity();

            _unavailableSlotIndexes.ForEach(i => _unitTiles[i].isEmpty = false);
        }
    }
}