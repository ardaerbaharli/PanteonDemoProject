using System;
using UI.Utilities;

namespace MapEntities.Components
{
    public class Health
    {
        private readonly SpriteFillBar _healthBar;
        private float _health;

        private MapEntity _mapEntity;
        private float _maxHealth;
        public Action onDied;
        public Action onHit;

        public Health(SpriteFillBar healthBar)
        {
            _healthBar = healthBar;
        }

        public void Initialize(float productPropertyHealth)
        {
            _maxHealth = productPropertyHealth;
            _health = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (_health <= 0) return;

            _health -= damage;
            onHit?.Invoke();
            _healthBar.SetFillBar(_health / _maxHealth);
            if (_health <= 0) Die();
        }

        private void Die()
        {
            onDied?.Invoke();
        }
    }
}