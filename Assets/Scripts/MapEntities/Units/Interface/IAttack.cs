using MapEntities.Buildings;

namespace MapEntities.Units.Interface
{
    public interface IAttack
    {
        public float Damage { get; set; }
        public float AttackInterval { get; set; }
        public bool IsAttacking { get; set; }
        public bool AbortAttack { get; set; }
        public void Attack(Building b);
        public void Attack(Unit u);
    }
}