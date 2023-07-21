using System;
using System.Collections.Generic;
using Grid;
using Pathfinding;
using UnityEngine;

namespace MapEntities.Units.Components
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Transform movePoint;
        private GridTile _currentTile;

        private bool _isMoving;
        private float _moveSpeed;
        private List<GridTile> _path;
        public Action<GridTile> onReached;
        public Action startedMoving;


        private void FixedUpdate()
        {
            if (!_isMoving) return;
            Move();
        }


        public void Initialize(float speed)
        {
            _moveSpeed = speed;
            movePoint.SetParent(null);
        }

        private void Move()
        {
            // move towards the move point, if we are close enough to the move point, move to the next tile
            if (Vector3.Distance(transform.position, movePoint.position) <= .001f)
                switch (_path.Count)
                {
                    case > 0:
                        _currentTile = _path[0];
                        _path.RemoveAt(0);
                        SetMovementVector();
                        break;
                    case 0:
                        _isMoving = false;
                        onReached?.Invoke(_currentTile);
                        break;
                }

            transform.position =
                Vector3.MoveTowards(transform.position, movePoint.position, _moveSpeed * Time.fixedDeltaTime);
        }

        private void SetMovementVector()
        {
            if (_path == null) return;
            if (_path.Count <= 0) return;

            movePoint.position = _path[0].transform.position;
        }

        public void ResetEverything()
        {
            movePoint.SetParent(transform);
        }

        private GridTile _targetTile, _previousTargetTile;

        public void StartMoving(GridTile startTile, GridTile targetTile)
        {
            if (_isMoving) startTile = _currentTile;
            _isMoving = false;

            if (_targetTile != null)
            {
                _previousTargetTile = _targetTile;
                _previousTargetTile.ResetColor();
            }

            _targetTile = targetTile;

            _currentTile = startTile;
            _targetTile.SetColor(Color.yellow);
            _targetTile.isReserved = true;

            startedMoving?.Invoke();
            Pathfinder.Instance.CalculatePath(startTile, _targetTile, OnPathFound);
        }

        private void OnPathFound(List<GridTile> path)
        {
            if (path.Count <= 0)
            {
                _targetTile.ResetColor();
                onReached?.Invoke(_currentTile);
                return;
            }

            _path = path;
            _isMoving = true;
            movePoint.position = _path[0].transform.position;
        }
    }
}