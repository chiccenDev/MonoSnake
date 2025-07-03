using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoSnake.Core
{
    /// <summary>
    /// Controls playback of an Animation
    /// </summary>
    internal struct AnimationPlayer
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation
        {
            get { return animation;  }
        }
        Animation animation;

        public int FrameIndex { get { return frameIndex; } }
        int frameIndex;

        private float time;

        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        /// <summary>
        /// Begins or continues the playback of an animation.
        /// </summary>
        /// <param name="animation"></param>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            Draw(gameTime, spriteBatch, position, spriteEffects, Color.White);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process the elapsed time to advance the animation
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Advance the frame if enough time ahs passed
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Loop the animation or clamp to the final frame
                if (Animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                }
            }

            // Determine the portion of the texture representing the current frame
            Rectangle source = new Rectangle(
                frameIndex * Animation.Texture.Height,  // Horizontal offset per frame
                0,                                      // Top edge of the frame
                Animation.FrameWidth,                   // Frame width
                Animation.FrameHeight                   // Frame height
            );

            // Draw the current frame at the specified position
            spriteBatch.Draw(Animation.Texture, position, source, color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
