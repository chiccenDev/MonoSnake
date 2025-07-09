using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoSnake.Core;
using MonoSnake.Effects;

namespace MonoSnake.Screens
{
    /// <summary>
    /// This screen implements the actual game logic and manages the gameplay experience.
    /// It controls level loading, player interaction, game state updates, and render
    /// for the active gameplay session.
    /// </summary>
    partial class GameplayScreen : GameScreen
    {

        ContentManager content;

        float pauseAlpha;

        private SpriteBatch spriteBatch;

        private World world;

        private ParticleManager particleManager;

        private const int textEdgeSpacing = 10;

        public GameplayScreen()
        {
            TransitionOffTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            spriteBatch = ScreenManager.SpriteBatch;

            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(content.Load<Song>("Sounds/Music"));

            particleManager ??= ScreenManager.Game.Services.GetService<ParticleManager>();

            LoadNextLevel();

            ScreenManager.Game.ResetElapsedTime();
        }

        private void LoadNextLevel()
        {
            world = new World(ScreenManager, null, 0);
            world.ParticleManager = particleManager;
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasfocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasfocus, coveredByOtherScreen);

            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            world.Paused = !IsActive;

            if (IsActive)
            {
                if (world.ParticleManager.Finished)
                {
                    if (!world.Player.IsAlive)
                        Debug.WriteLine("GameplayScreen: Player is dead.");
                }
            }
        }

        public override void HandleInput(GameTime gameTime, InputState inputState)
        {
            ArgumentNullException.ThrowIfNull(inputState);

            base.HandleInput(gameTime, inputState);

            if (inputState.IsPauseGame())
                ScreenManager.AddScreen(new PauseScreen());
            else
            {
                world.Update(gameTime, inputState, ScreenManager.Game.Window.CurrentOrientation);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            world.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
