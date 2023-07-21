using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridTile : MonoBehaviour
    {
        public Vector2 size;
        public bool isEmpty;
        public bool isBuildingLandTile;
        public bool isBuildingEdgeTile;
        public bool isMapEdgeTile;
        public bool isReserved;

        public GridTile previousNode;
        public float gCost;
        public float hCost;


        private SpriteRenderer _spriteRenderer;
        private Color _startColor;
        public float fCost => gCost + hCost;
        public Vector3Int Coordinate { get; set; }

        public void Init(Vector3Int coordinate, Vector2 size)
        {
            this.size = size;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _startColor = _spriteRenderer.color;
            Coordinate = coordinate;
            isEmpty = true;
        }

        public void SetColor(Color red)
        {
            _spriteRenderer.color = red;
        }

        public void ResetColor()
        {
            _spriteRenderer.color = _startColor;
        }

        public List<GridTile> GetNeighbors()
        {
            var neighbors = new List<GridTile>();
            var directions = new List<Vector3Int>
            {
                Vector3Int.up,
                Vector3Int.down,
                Vector3Int.left,
                Vector3Int.right
            };
            foreach (var direction in directions)
            {
                var neighbor = GridManager.Instance.GetNeighbor(this, direction);
                if (neighbor != null) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is GridTile other) return Coordinate == other.Coordinate;

            return false;
        }
    }
}