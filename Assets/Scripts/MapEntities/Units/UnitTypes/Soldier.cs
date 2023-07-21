using System.Collections;
using MapEntities.Buildings;
using MapEntities.Units.Interface;
using Products.ScriptableObjects;
using UnityEngine;

namespace MapEntities.Units.UnitTypes
{
    public class Soldier : Unit, IAttack
    {
        public float Damage { get; set; }
        public float AttackInterval { get; set; }

        public bool IsAttacking
        {
            get => isActing;
            set => isActing = value;
        }

        public bool AbortAttack { get; set; }

        public void Attack(Unit unit)
        {
            if (!isAlive) return;
            if (!unit.isAlive) return;
            if (IsMe(unit)) return;

            StartCoroutine(AttackCoroutine(unit));
        }

        public void Attack(Building building)
        {
            if (!isAlive) return;
            StartCoroutine(AttackCoroutine(building));
        }

        public override void Initialize(ProductProperty productProperty)
        {
            base.Initialize(productProperty);
            Damage = unitProperty.damage;
            AttackInterval = unitProperty.attackInterval;
        }


        public override void Act(Building b)
        {
            Attack(b);
        }

        public override void Act(Unit u)
        {
            Attack(u);
        }

        protected override void AbortAct()
        {
            AbortAttack = true;
        }

        private IEnumerator AttackCoroutine(Unit targetUnit)
        {
            targetUnit.onDestroyed = null;
            targetUnit.movement.startedMoving = null;

            if (IsAttacking || movingToAct)
            {
                AbortAct();
                yield return new WaitUntil(() => !IsAttacking);
                yield return new WaitUntil(() => reachedTarget);
            }

            yield return new WaitUntil(() => targetUnit.reachedTarget);

            targetUnit.onDestroyed += AbortAct;
            targetUnit.movement.startedMoving += () =>
            {
                if (!isAlive) return;
                if (!targetUnit.isAlive) return;
                StartCoroutine(AttackCoroutine(targetUnit));
            };

            var targetUnitTile = targetUnit.positionTile;
            var targetUnitNeighbors = targetUnitTile.GetNeighbors();
            if (!targetUnitNeighbors.Contains(positionTile))
            {
                var targetTile = targetUnit.positionTile.GetNeighbors().Find(x => x.isEmpty && !x.isReserved);
                if (targetTile == null) yield break;
                movingToAct = true;
                Move(targetTile);
                if (!canMove)
                {
                    canMove = true;
                    reachedTarget = true;
                    yield break;
                }
                yield return new WaitUntil(() => reachedTarget);
            }


            IsAttacking = true;

            while (IsAttacking)
            {
                if (AbortAttack)
                {
                    AbortAttack = false;
                    break;
                }

                targetUnit.health.TakeDamage(Damage);
                yield return new WaitForSeconds(AttackInterval);
            }

            IsAttacking = false;
        }

        private IEnumerator AttackCoroutine(Building building)
        {
            if (IsAttacking || movingToAct)
            {
                AbortAct();
                yield return new WaitUntil(() => !IsAttacking);
            }

            reachedTarget = false;
            movingToAct = true;
            Move(building);
            if (!canMove)
            {
                canMove = true;
                reachedTarget = true;
                yield break;
            }
            yield return new WaitUntil(() => reachedTarget);

            building.onDestroyed += AbortAct;
            IsAttacking = true;

            while (IsAttacking)
            {
                if (AbortAttack)
                {
                    AbortAttack = false;
                    break;
                }

                building.health.TakeDamage(Damage);
                yield return new WaitForSeconds(AttackInterval);
            }

            IsAttacking = false;
        }
    }
}