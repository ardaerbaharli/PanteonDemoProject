using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        public static Pathfinder Instance;
        private List<GridTile> _closedSet;
        private List<GridTile> _openSet;

        private void Awake()
        {
            Instance = this;

            _openSet = new List<GridTile>();
            _closedSet = new List<GridTile>();
        }

        public void CalculatePath(GridTile startTile, GridTile targetTile, Action<List<GridTile>> onPathFound)
        {
            // Calculate the path from the start tile to the target tile using the A* algorithm
            _openSet = new List<GridTile>();
            _closedSet = new List<GridTile>();
            _openSet.Add(startTile);

            var path = new List<GridTile>();

            while (_openSet.Count > 0)
            {
                var currentNode = _openSet[0];
                for (var i = 1; i < _openSet.Count; i++)
                    if (_openSet[i].fCost < currentNode.fCost ||
                        (Math.Abs(_openSet[i].fCost - currentNode.fCost) < 0.001f &&
                         _openSet[i].hCost < currentNode.hCost))
                        currentNode = _openSet[i];

                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                if (currentNode == targetTile)
                {
                    path = RetracePath(startTile, targetTile);
                    break;
                }

                foreach (var neighbour in currentNode.GetNeighbors())
                {
                    if (!neighbour.isEmpty || _closedSet.Contains(neighbour)) continue;

                    var newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (!(newMovementCostToNeighbour < neighbour.gCost) && _openSet.Contains(neighbour)) continue;

                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetTile);
                    neighbour.previousNode = currentNode;

                    if (!_openSet.Contains(neighbour)) _openSet.Add(neighbour);
                }
            }

            if (path.Count == 0) targetTile.isReserved = false;
            onPathFound?.Invoke(path);
        }

        private float GetDistance(GridTile currentNode, GridTile neighbour)
        {
            var currentTilePosition = currentNode.transform.position;
            var targetTilePosition = neighbour.transform.position;
            var distance = Vector3.Distance(currentTilePosition, targetTilePosition);
            return distance;
        }

        private List<GridTile> RetracePath(GridTile startNode, GridTile targetNode)
        {
            var path = new List<GridTile>();
            var currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.previousNode;
            }

            path.Reverse();
            return path;
        }
    }
}