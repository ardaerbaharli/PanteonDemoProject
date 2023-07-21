using UnityEngine;

namespace MapEntities.Components
{
    public class SpriteRendererController
    {
        private readonly Color _defaultColor;
        private readonly SpriteRenderer _spriteRenderer;

        public SpriteRendererController(SpriteRenderer renderer)
        {
            _spriteRenderer = renderer;
            _defaultColor = _spriteRenderer.color;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        public void SetColorToDefault()
        {
            _spriteRenderer.color = _defaultColor;
        }

        public void SetSprite(Sprite getSprite)
        {
            _spriteRenderer.sprite = getSprite;
        }
    }
}