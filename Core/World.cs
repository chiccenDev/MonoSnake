using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Effects;
using MonoSnake.Screens;

namespace MonoSnake.Core
{
    /// <summary>
    /// A uniform grid of tiles with collections of objects.
    /// The level owns the player and controls the game's conditions
    /// </summary>
    internal class World : IDisposable
    {
        private Fruit fruit;
        public Dictionary<Vector2, Tail> tails;

        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        Player player;
        /// <summary>
        /// Gets the player instance in the level.
        /// </summary>
        public Player Player
        {
            get { return player; }
        }

        // Key locations
        private Vector2 start;

        //private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
        private Random random = new Random(Guid.NewGuid().GetHashCode()); // More "true" random
        //private Random random = new Random(354668); // Arbitrary but constant seed. Uncomment for for debugging

        private ScreenManager screenManager;

        ContentManager content;
        /// <summary>
        /// Gets the content manager for the level.
        /// </summary>
        public ContentManager Content
        {
            get { return content; }
        }

        //private SoundEffect soundEffect;
        private SpriteFont hudFont;

        /// <summary>
        /// Gets the width of the level measured in tiles.
        /// </summary>
        public int Width => 50; //tiles.GetLength(0);

        /// <summary>
        /// Gets the height of the level measured in tiles.
        /// </summary>
        public int Height => 30; // tiles.GetLength(1);

        private ParticleManager particleManager;
        //private bool particlesExploding;

        public ParticleManager ParticleManager { get => particleManager; set => particleManager = value; }

        //private bool saved;
        private bool readyToPlay;

        private float cameraPosition;

        public bool Paused { get; internal set; }

        public const int NUMBER_OF_LAYERS = 3;

        public World(ScreenManager screenManager, string levelPath, int levelIndex)
        {
            this.screenManager = screenManager;

            content = new ContentManager(this.screenManager.Game.Services, "Content");


            start = new Vector2(
                (Width / 2) * Tile.Width,
                (Height - 2) * Tile.Height
            );

            player = new Player(this, start);

            tails = new();

            // Load font
            hudFont = content.Load<SpriteFont>("Fonts/Hud");

            SpawnFruit();
        }

        public void Dispose()
        {
            Content.Unload();
        }

        public bool GetFruit(Vector2 tile)
        {
            return fruit.SourceRect.Intersects(player.SourceRect);
        }

        public void SpawnFruit()
        {
            Vector2 location = Vector2.Zero;
            while (location == Vector2.Zero || tails.ContainsKey(location))
            {
                location = new Vector2(
                    (random.Next(Width) + 1f) * Tile.Width,
                    (random.Next(Height) + 1f) * Tile.Height
                    );
            }
            fruit = new Fruit(new Sprite(content.Load<Texture2D>("Sprites/fruit")), location);
        }

        public void Update(GameTime gameTime, InputState inputState, DisplayOrientation displayOrientation, bool readyToPlay = true)
        {
            if (gameTime == null)
                throw new ArgumentNullException(nameof(gameTime));
            if (inputState == null)
                throw new ArgumentNullException(nameof(inputState));

            this.readyToPlay = readyToPlay;
            particleManager.Update(gameTime);

            Player.Update(gameTime, inputState, displayOrientation);

            // Parallax scroll if necessary
            UpdateCamera(screenManager.BaseScreenSize);
        }

        public void StartNewLife()
        {
            Player.Reset(start);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Create a camera transformation matrix to simulate parallax scrolling.
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, 0.0f, 0.0f);

            // Get the global transformation scale for consistent rendering across resolutions.
            float transformScale = screenManager.GlobalTransformation.M11;

            // Draw main game elements (tiles, points, player, enemies, etc) with camera transformation.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraTransform * screenManager.GlobalTransformation);

            float cameraRight = cameraPosition + screenManager.BaseScreenSize.X;

            // Draw the player.
            Player.Draw(gameTime, spriteBatch);

            fruit?.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // Draw the HUD (time, score, etc).
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, screenManager.GlobalTransformation);

            particleManager.Draw(spriteBatch);
            DrawHud(spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Draws the HUD overlay. Currently used for debugging.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawHud(SpriteBatch spriteBatch)
        {
            string snakePos = $"Snake Position: {player.Position}";
            string tailCount = $"Tails: {tails.Count}";
            spriteBatch.DrawString(hudFont, snakePos, new Vector2(16, 16), Color.White);
            spriteBatch.DrawString(hudFont, tailCount, new Vector2(16, hudFont.MeasureString(snakePos).Y + 16), Color.White);
            spriteBatch.DrawString(hudFont, $"Tail Tiles: {tails.Keys.ToString()}", new Vector2(16, hudFont.MeasureString(snakePos).Y + hudFont.MeasureString(tailCount).Y + 16), Color.White);
        }

        private void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
        {
            // Draw the shadow slightly offset from the main text.
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            // Draw the main text.
            spriteBatch.DrawString(font, value, position, color);
        }

        const float ViewMargin = 0.35f;
        private void UpdateCamera(Vector2 viewport)
        {
            if (!readyToPlay || Player == null)
                return;

            // Calcualte the edges of the screen based on the view margin.
            float marginWidth = viewport.X * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.X - marginWidth;

            // Calculate how far to scroll the camera when the player approaches the screen edges.
            float cameraMovement = 0.0f;
            if (Player.Position.X < marginLeft)
                cameraMovement = Player.Position.X - marginLeft;
            else if (Player.Position.X > marginRight)
                cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, ensuring it stays within the level bounds.
            float maxCameraPosition = Tile.Width * Width - viewport.X;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }
    }
}
