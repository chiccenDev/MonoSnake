using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.Core;

namespace MonoSnake.Screens
{
    /// <summary>
    /// The ScreenManager is a component responsbile for managing multiple <see cref="GameScreen"/> instances.
    /// It maintains a stack of screens, invoked their Update and Draw methods, and automatically routes input
    /// to the top-most active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        // List of active screens and screens pending update.
        private readonly List<GameScreen> screens = new();
        private readonly List<GameScreen> screensToUpdate = new();

        private bool cursorDebug;
        public bool CursorDebug { get => cursorDebug; set => cursorDebug = value; }

        // Manages player input.
        private readonly InputState inputState = new();

        // Shared resources for drawing and content management.
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Texture2D blankTexture;
        private Texture2D blankSquare;

        private bool isInitialized;
        private bool traceEnabled;

        private int backbufferWidth;
        /// <summary>Gets or sets the current backbuffer width.</summary>
        public int BackbufferWidth { get => backbufferWidth; set => backbufferWidth = value; }

        private int backbufferHeight;
        /// <summary>Gets or sets the current backbuffer height.</summary>
        public int BackbufferHeight { get => backbufferHeight; set => backbufferHeight = value; }

        private Vector2 baseScreenSize = new Vector2(800, 480);
        /// <summary>Gets or sets the base screen size for scaling calculations.</summary>
        public Vector2 BaseScreenSize { get => baseScreenSize; set => baseScreenSize = value; }

        private Matrix globalTransformation;
        /// <summary>Gets or sets the global transformation matrix for scaling and positioning.</summary>
        public Matrix GlobalTransformation { get => globalTransformation; set => globalTransformation = value; }

        /// <summary>
        /// Provides access to a shared SpriteBatch instance for drawing operations.
        /// </summary>
        public SpriteBatch SpriteBatch => spriteBatch;

        /// <summary>
        /// Provides access to a shared SpriteFont instance for text rendering.
        /// </summary>
        public SpriteFont Font => font;

        /// <summary>
        /// Enabled or disabled screen tracing for debugging purposes.
        /// When enabled, the manager prints a list of active screens during updates.
        /// </summary>
        public bool TraceEnabled { get => traceEnabled; set => traceEnabled = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenManager"/> class.
        /// </summary>
        /// <param name="game">The associated Game instance.</param>
        public ScreenManager(Game game) : base(game)
        {
            TraceEnabled = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
            cursorDebug = false;
        }

        /// <summary>
        /// Loads graphical content for the ScreenManager and all active screens.
        /// </summary>
        protected override void LoadContent()
        {
            {
                ContentManager content = Game.Content;
                spriteBatch = new SpriteBatch(GraphicsDevice);
                font = content.Load<SpriteFont>("Fonts/Hud");
                blankTexture = content.Load<Texture2D>("Sprites/blank");
                blankSquare = content.Load<Texture2D>("UI/blankSquare");


                foreach (GameScreen screen in screens)
                {
                    screen.LoadContent();
                }
            }
        }

        /// <summary>
        /// Unload graphical content for all screens.
        /// </summary>
        protected override void UnloadContent()
        {
            {
                foreach (GameScreen screen in screens)
                {
                    screen.UnloadContent();
                }
            }
        }

        /// <summary>
        /// Updates the active screens and processes input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            inputState.Update(gameTime, GraphicsDevice.Viewport);
            screensToUpdate.Clear();
            screensToUpdate.AddRange(screens);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreens = false;

            while (screensToUpdate.Count > 0)
            {
                GameScreen screen = screensToUpdate[^1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreens);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, inputState);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                        coveredByOtherScreens = true;
                }
            }

            if (traceEnabled)
                TraceScreens();
        }

        /// <summary>
        /// Prints active screen names to the debug console for diagnostic purposes.
        /// </summary>
        private void TraceScreens()
        {
            var screenNames = screens.Select(screen => screen.GetType().Name).ToList();
            Debug.WriteLine(string.Join(", ", screenNames));
        }

        /// <summary>
        /// Draws the active screens.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            foreach (var screen in screens)
            {
                /*
                if (screen.ScreenState != ScreenState.Hidden) 
                    screen.Draw(gameTime);
                */
                screen.Draw(gameTime);
            }
            
            if (cursorDebug)
            {
                Vector2 mouse = inputState.CurrentCursorLocation; // pre-adjusted to screen-space using matrix
                Vector2 mouseLocation = new(mouse.X - (mouse.X % 16), mouse.Y - (mouse.Y % 16));
                Rectangle rect = new((int)mouse.X, (int)mouse.Y, 16, 16);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, globalTransformation);
                spriteBatch.Draw(blankSquare, rect, null, Color.Red);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Release resources used by the <see cref="ScreenManager"/> object.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only the unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing) spriteBatch?.Dispose(); // Dispose of managed resources
            }
            finally
            {
                // Call the base class's Dispose method to ensure proper cleanup.
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Adds a new screen to the ScreenManager.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized) screen.LoadContent();

            screens.Add(screen);
        }

        /// <summary>
        /// removes a screen from the ScreenManager.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized) screen.UnloadContent();

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Returns an array of all active screens managed by the ScreenManager.
        /// </summary>
        /// <returns>
        /// An array containing all current GameScreen instances. This array is a copy
        /// of the internal list to esnure screens are only added for removed using
        /// <see cref="AddScreen(GameScreen)"/>
        /// <see cref="RemoveScreen(GameScreen)"/>
        /// </returns>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        /// <summary>
        /// Draws a translucent black fullscreen sprite. Thisis used for fading
        /// screens in and out, or for darkening the background ebhind popups.
        /// </summary>
        /// <param name="alpha">The opacity level of the fade (0 = fully transparent, 1 = fully opaque).</param>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, GlobalTransformation);

            spriteBatch.Draw(blankTexture,
                new Rectangle(0, 0, viewport.Width, viewport.Height),
                Color.Black * alpha);

            spriteBatch.End();
        }

        /// <summary>
        /// Scales the game presentation area to match the screen's aspect ratio.
        /// </summary>
        public void ScalePresentationArea()
        {
            // Validate parameters before calculation
            if (GraphicsDevice == null || baseScreenSize.X <= 0 || baseScreenSize.Y <= 0)
            {
                throw new InvalidOperationException("Invalid gaphics configuration");
            }

            // Fetch screen dimensions
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Prevent division by zero
            if (backbufferHeight == 0 || baseScreenSize.Y == 0) return;

            // Calculate aspect ratios
            float baseAspectRatio = baseScreenSize.X / baseScreenSize.Y;
            float screenAspectRatio = backbufferWidth / (float)backbufferHeight;

            // Determine uniform scaling factor
            float scalingFactor;
            float horizontalOffset = 0;
            float verticalOfffset = 0;

            if (screenAspectRatio > baseAspectRatio)
            {
                // Wiser screen: scale by height
                scalingFactor = backbufferHeight / baseScreenSize.Y;

                // Center things horizontally
                horizontalOffset = (backbufferWidth - baseScreenSize.X * scalingFactor) / 2;
            }
            else
            {

                // Taller screen: scale by width
                scalingFactor = backbufferWidth / baseScreenSize.X;

                // Center things vertically
                verticalOfffset = (backbufferHeight - baseScreenSize.Y * scalingFactor) / 2;
            }

            // Update the transformation matrix
            globalTransformation = Matrix.CreateScale(scalingFactor) *
                                   Matrix.CreateTranslation(horizontalOffset, verticalOfffset, 0);

            // Update the inputTransformation with the Inverted globalTransformation
            inputState.UpdateInputTransformation(Matrix.Invert(globalTransformation));

            // Debug info
            Debug.WriteLine($"Screen Size - Width[{backbufferWidth}] Height[{backbufferHeight}] ScalingFactor[{scalingFactor}]");
        }
    }
}
