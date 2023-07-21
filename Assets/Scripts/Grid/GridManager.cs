using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;
        [SerializeField] private Vector2Int size;
        [SerializeField] private Vector2 gap;
        [SerializeField] private AnimationCurve cameraMovementCurve;

        [SerializeField] private GridTile tilePrefab;
        [SerializeField] private Vector3 cellSize;
        [SerializeField] private GridLayout.CellSwizzle gridSwizzle;

        private Camera _cam;

        private Vector3 _cameraPositionTarget;
        private float _cameraSizeTarget;
        private float _cameraSizeVel;

        private Vector2 _currentGap;
        private Vector2 _gapVel;
        private bool _generated;
        private UnityEngine.Grid _grid;
        private Vector3 _moveVel;
        private Dictionary<GridTile, Vector3Int> _tilePositions;

        private void Awake()
        {
            Instance = this;

            _grid = GetComponent<UnityEngine.Grid>();
            _tilePositions = new Dictionary<GridTile, Vector3Int>();
            _cam = Camera.main;
            _currentGap = gap;

            Generate();
            StartCoroutine(SetCameraCoroutine());
        }

        private IEnumerator SetCameraCoroutine()
        {
            // move the camera to the grid 
            yield return new WaitUntil(() => _generated);

            var camPosition = _cam.transform.position;
            var camTargetPosition = _cameraPositionTarget;
            var camSize = _cam.orthographicSize;
            var camTargetSize = _cameraSizeTarget;
            var totalTime = 1f;
            var time = 0f;

            // move it with curve
            while (time < totalTime)
            {
                time += Time.deltaTime;
                var t = time / totalTime;
                var curveValue = cameraMovementCurve.Evaluate(t);
                _cam.transform.position = Vector3.Lerp(camPosition, camTargetPosition, curveValue);
                _cam.orthographicSize = Mathf.Lerp(camSize, camTargetSize, curveValue);
                yield return null;
            }
        }

        private void Generate()
        {
            // clear all children
            foreach (Transform child in transform) Destroy(child.gameObject);

            // set grid properties
            _grid.cellLayout = GridLayout.CellLayout.Rectangle;
            _grid.cellSize = cellSize;
            if (_grid.cellLayout != GridLayout.CellLayout.Hexagon) _grid.cellGap = _currentGap;
            _grid.cellSwizzle = gridSwizzle;

            // calculate coordinates
            var coordinates = new List<Vector3Int>();
            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
                coordinates.Add(new Vector3Int(x, y));

            var bounds = new Bounds();
            var rand = new Random();

            // create tiles
            foreach (var coordinate in coordinates.OrderBy(t => rand.Next()).Take(coordinates.Count))
            {
                var prefab = tilePrefab;
                var position = _grid.GetCellCenterWorld(coordinate);
                var spawned = Instantiate(prefab, position, Quaternion.identity, transform);
                spawned.Init(coordinate, cellSize);
                bounds.Encapsulate(position);
                _tilePositions.Add(spawned, coordinate);
            }

            SetEdges();
            SetCamera(bounds);

            _generated = true;
        }

        private void SetEdges()
        {
            // set the edges of the grid as edge tile so that the player can't spawn building there
            // reason: the player can't spawn building on the edge of the grid because soldiers need to be able to walk around the building and if the building is on the edge of the grid, soldiers can't walk around it 

            var edges = new List<GridTile>();
            foreach (var tile in _tilePositions.Keys)
            {
                var coordinate = _tilePositions[tile];
                var neighbors = new List<Vector3Int>
                {
                    coordinate + Vector3Int.up,
                    coordinate + Vector3Int.down,
                    coordinate + Vector3Int.left,
                    coordinate + Vector3Int.right
                };

                foreach (var neighbor in neighbors)
                    if (_tilePositions.ContainsValue(neighbor) == false)
                    {
                        var edge = GetTile(coordinate);
                        if (edge != null) edges.Add(edge);
                    }
            }

            edges.ForEach(x => x.isMapEdgeTile = true);
        }

        private void SetCamera(Bounds bounds)
        {
            bounds.Expand(2);

            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;

            _cameraPositionTarget = bounds.center + Vector3.back;
            _cameraSizeTarget = Mathf.Max(horizontal, vertical) * 0.5f;
        }

        private GridTile GetTile(Vector3Int coordinate)
        {
            return _tilePositions.FirstOrDefault(t => t.Value == coordinate).Key;
        }


        public bool Hover(Vector3 position, Vector2 buildingSize, out List<GridTile> unknown)
        {
            // while hovering, check if it is in a valid position 
            // if not valid, change the color of the tiles to red
            // if valid, change tiles to green

            var bottomLeft = position - new Vector3(buildingSize.x / 2, buildingSize.y / 2);
            var bottomLeftCell = _grid.WorldToCell(bottomLeft);

            ResetAllTileColors();
            var isValid = true;
            var tiles = new List<GridTile>();
            for (var i = 0; i < buildingSize.x; i++)
            for (var j = 0; j < buildingSize.y; j++)
            {
                var tile = GetTile(bottomLeftCell + new Vector3Int(i, j, 0));

                if (tile == null)
                {
                    isValid = false;
                }
                else
                {
                    tile.isBuildingEdgeTile = i == 0 || j == 0 || i == buildingSize.x - 1 || j == buildingSize.y - 1;
                    tiles.Add(tile);
                    if (!tile.isEmpty || tile.isMapEdgeTile || tile.isReserved) isValid = false;
                }
            }

            foreach (var tile in tiles) tile.SetColor(isValid ? Color.green : Color.red);

            unknown = isValid ? tiles : null;
            return isValid;
        }


        public void ResetAllTileColors()
        {
            _tilePositions.Keys.ToList().ForEach(t => t.ResetColor());
        }

        public GridTile GetNeighbor(GridTile tile, Vector3Int direction)
        {
            var coordinate = _tilePositions[tile];
            var neighborCoordinate = coordinate + direction * (int) (cellSize.x + gap.x);
            return GetTile(neighborCoordinate);
        }
    }
}