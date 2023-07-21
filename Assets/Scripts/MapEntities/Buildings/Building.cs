using System.Collections.Generic;
using System.Linq;
using Grid;
using Products.ScriptableObjects;

namespace MapEntities.Buildings
{
    public abstract class Building : MapEntity
    {
        public BuildingProductProperty buildingProperty;

        public override void Initialize(ProductProperty productProperty)
        {
            base.Initialize(productProperty);
            buildingProperty = (BuildingProductProperty) productProperty;
        }


        public virtual void Build(List<GridTile> currentTiles)
        {
            landTiles = currentTiles.ToList();
            edgeTiles = landTiles.Where(tile => tile.isBuildingEdgeTile).ToList();

            var leftBottomTile = landTiles[0];
            var rightTopTile = landTiles[^1];
            var center = (leftBottomTile.transform.position + rightTopTile.transform.position) / 2;
            transform.position = center;
            landTiles.ForEach(tile =>
            {
                tile.isBuildingLandTile = true;
                tile.isEmpty = false;
                tile.ResetColor();
            });
        }

        public override void DestroyEntity()
        {
            landTiles.ForEach(tile =>
            {
                tile.isBuildingLandTile = false;
                tile.isEmpty = true;
                tile.ResetColor();
            });

            edgeTiles.ForEach(tile =>
            {
                tile.isBuildingEdgeTile = false;
                tile.isEmpty = true;
                tile.ResetColor();
            });

            base.DestroyEntity();
        }
    }
}