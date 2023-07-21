using System;
using Grid;
using MapEntities.Buildings;
using MapEntities.Units.Components;
using Products.ScriptableObjects;
using UnityEngine;

namespace MapEntities.Units
{
    public abstract class Unit : MapEntity
    {
        public UnitProductProperty unitProperty;
        public GridTile positionTile;

        public bool isAlive;
        public bool isOnSpawn;

        public Movement movement;
        public bool reachedTarget;
        public bool canMove;

        protected bool isActing;
        protected bool movingToAttack;
        public Action onLeftSpawn;


        private new void Awake()
        {
            base.Awake();
            movement = GetComponent<Movement>();
            movement.onReached += OnReachedTile;
        }

        private void OnEnable()
        {
            isAlive = true;
            reachedTarget = true;
        }

        public override void Initialize(ProductProperty productProperty)
        {
            base.Initialize(productProperty);
            unitProperty = (UnitProductProperty) productProperty;
            movement.Initialize(unitProperty.speed);
        }

        public abstract void Act(Building b);

        public abstract void Act(Unit u);
        protected abstract void AbortAct();

        public void Select()
        {
            spriteRendererController.SetColor(Color.green);
        }

        public void Deselect()
        {
            spriteRendererController.SetColorToDefault();
        }

        protected bool IsMe(Unit unit)
        {
            return unit == this;
        }

        public override void DestroyEntity()
        {
            isAlive = false;
            if (!positionTile.isBuildingEdgeTile && !positionTile.isBuildingLandTile)
            {
                positionTile.isEmpty = true;
                positionTile.isReserved = false;
            }

            positionTile.ResetColor();

            movement.onReached -= OnReachedTile;
            movement.ResetEverything();
            base.DestroyEntity();
        }


        protected void Move(Building selectedBuilding)
        {
            var targetTile = selectedBuilding.GetClosestTile(positionTile);
            Move(targetTile);
        }

        public void Move(GridTile targetTile)
        {
            if (isActing) AbortAct();

            reachedTarget = false;

            if (targetTile.isReserved || !targetTile.isEmpty)
            {
                return;
            }

            if (isOnSpawn)
            {
                onLeftSpawn?.Invoke();
                isOnSpawn = false;
            }

            movement.StartMoving(positionTile, targetTile);
        }

        private void OnReachedTile(GridTile tile)
        {
            if (tile == positionTile)
            {
                canMove = false;
                return;
            }
            
            canMove = true;
            
            if (!positionTile.isBuildingLandTile && !tile.isBuildingLandTile)
            {
                positionTile.isEmpty = true;
                positionTile.isReserved = false;
            }

            positionTile = tile;
            positionTile.isEmpty = false;
            positionTile.ResetColor();
            
            reachedTarget = true;
            movingToAttack = false;
        }
    }
}