using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Core
{
    internal class Sprite
    {
        Texture2D texture;
        public Texture2D Texture { get => texture;  }

        int index;
        public int Index { get => index; set => index = value; }

        int width;
        public int Width {  get => width; }

        int height;
        public int Height { get => height; }
        public Rectangle Source
        {
            get
            {
                return new Rectangle(index * width, 0, width, height);
            }
        }

        public Vector2 Origin
        {
            get { return new Vector2(Width / 2.0f, Height / 2.0f); }
        }

        public Sprite(Texture2D texture, int index = 0, int width = 16, int height = 16)
        {
            this.texture = texture;
            this.index = index;
            this.width = width;
            this.height = height;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            Draw(gameTime, spriteBatch, position, spriteEffects, Color.White);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
        {
            spriteBatch.Draw(Texture, position, Source, color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
