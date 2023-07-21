using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using MapEntities.Components;
using Products.ScriptableObjects;
using UI.Utilities;
using UnityEngine;
using Utilities;

namespace MapEntities
{
    public abstract class MapEntity : MonoBehaviour
    {
        public PooledObject pooledObject;

        public List<GridTile> landTiles;
        public List<GridTile> edgeTiles;

        public Vector2 size;

        private BoxCollider2D _boxCollider2D;

        public Health health;
        public Action onDestroyed;
        protected SpriteRendererController spriteRendererController;

        protected void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();

            spriteRendererController = new SpriteRendererController(GetComponent<SpriteRenderer>());

            var healthBar = GetComponentInChildren<SpriteFillBar>();
            health = new Health(healthBar);
            health.onHit += OnHit;
            health.onDied += DestroyEntity;
        }

        private void OnHit()
        {
            StartCoroutine(GetHitAnimation());
        }

        private IEnumerator GetHitAnimation()
        {
            spriteRendererController.SetColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            spriteRendererController.SetColorToDefault();
        }

        public void ReturnToPool()
        {
            pooledObject.ReturnToPool();
        }

        public virtual void Initialize(ProductProperty productProperty)
        {
            health.Initialize(productProperty.health);
            SetSize(productProperty.size);

            spriteRendererController.SetSprite(productProperty.sprite);
        }

        private void SetSize(Vector2 size)
        {
            this.size = size;
            _boxCollider2D.size = size;
        }


        public virtual void DestroyEntity()
        {
            onDestroyed?.Invoke();
            ReturnToPool();
        }


        public GridTile GetClosestTile(GridTile positionTile)
        {
            // return the closest tile from edgeTilesList to the positionTile

            var possibleTiles = new List<GridTile>();
            foreach (var edgeTile in edgeTiles.ToList())
                possibleTiles.AddRange(edgeTile.GetNeighbors().Where(x => x.isEmpty && !x.isReserved));

            if (possibleTiles.Count == 0) return positionTile;

            var closestTile = possibleTiles[0];
            var closestDistance = Vector3.Distance(positionTile.transform.position, closestTile.transform.position);
            foreach (var tile in possibleTiles)
            {
                var distance = Vector3.Distance(positionTile.transform.position, tile.transform.position);
                if (!(distance < closestDistance)) continue;

                closestDistance = distance;
                closestTile = tile;
            }


            return closestTile;
        }
    }
}