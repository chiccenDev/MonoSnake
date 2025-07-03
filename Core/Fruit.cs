using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Core
{
    internal class Fruit
    {
        private Sprite sprite;
        public Sprite Sprite { get => sprite; }

        private Vector2 position;
        Vector2 Position { get => position; }

        public Rectangle SourceRect
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    sprite.Width,
                    sprite.Height
                    );
            }
        }

        public Fruit(Sprite sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.position = position;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);
        }
    }
}
