using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Effects;
using MonoSnake.Screens;
using MonoSnake.Settings;

namespace MonoSnake
{
    public class Game1 : Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;

        // Manages the game's screen transitions and screens.
        private ScreenManager screenManager;

        // Manages game settings, such as preferences and configurations
        private SettingsManager<GameSettings> settingsManager;

        // Texture for rendering particles
        private Texture2D particleTexture;

        // Manages particle effects in the game
        private ParticleManager particleManager;

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Public accessor for name of game for specific needs such as storage path.
        /// </summary>
        public readonly static string Name = "Game";

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings,
        /// initializes services like settings, and sets up the screen manager for
        /// screen transitions.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphics);

            // Determine the appropriate settings storage based on the platform.
            ISettingsStorage storage;

            if (IsDesktop)
            {
                storage = new SettingsStorage();
                graphics.IsFullScreen = false;
                IsMouseVisible = true;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            // Initialize settings manager.
            settingsManager = new SettingsManager<GameSettings>(storage);
            Services.AddService(typeof(SettingsManager<GameSettings>), settingsManager);

            Content.RootDirectory = "Content";

            // Configure screen orientations.
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            // Initialize the screen manager.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
        }

        protected override void Initialize()
        {
            base.Initialize();

            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // Load a texture for particles and initialize the particle manager.
            particleTexture = Content.Load<Texture2D>("Sprites/blank");
            particleManager = new ParticleManager(particleTexture, new Vector2(400, 200));

            // Share the particle manager as a service.
            Services.AddService(typeof(ParticleManager), particleManager);
        }
    }
}
