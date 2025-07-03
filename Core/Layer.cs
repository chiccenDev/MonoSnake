using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Core
{
    /// <summary>
    /// Represents a parallax scrolling layer in the game, typically used for background elements.
    /// </summary>
    internal class Layer
    {
        private Texture2D[] textures;
        private float scrollSpeed;

        public Layer(Texture2D[] textures, float scrollSpeed)
        {
            this.textures = textures;
            this.scrollSpeed = scrollSpeed;
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch, float cameraPosition)
        {
            // Assume each segment is the same width.
            int segmentWidth = textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition * scrollSpeed;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            spriteBatch.Draw(textures[leftSegment % textures.Length], new Vector2(x, 0.0f), Color.White);
            spriteBatch.Draw(textures[rightSegment % textures.Length], new Vector2(x + segmentWidth, 0.0f), Color.White);
        }
    }
}
