
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Effects;
using MonoSnake.Settings;
using System.Resources;

namespace MonoSnake.Screens
{
    /// <summary>
    /// The settings screen is brought up over the top of the main emny
    /// screen, and gives the user a chance to configure the game
    /// in various, hopefully useful, ways.
    /// </summary>
    internal class SettingsScreen : MenuScreen
    {
        private MenuEntry fullscreenMenuEntry;
        private MenuEntry traceScreensMenuEntry;
        private MenuEntry particleEffectMenuEntry;
        private MenuEntry backMenuEntry;
        private MenuEntry cursorDebugMenuEntry;

        private GraphicsDeviceManager gdm;

        private static ParticleEffectType currentParticleEffect;
        /// <summary>
        /// Gets the current selected particle effect type.
        /// </summary>
        public static ParticleEffectType CurrentParticleEffect { get => currentParticleEffect; }

        private SettingsManager<GameSettings> settingsManager;
        private ParticleManager particleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsScreen"/> class.
        /// </summary>
        public SettingsScreen()
            : base("Settings")
        {
            // MISSING

            // Create our menu entries.
            fullscreenMenuEntry = new MenuEntry(string.Empty);
            particleEffectMenuEntry = new MenuEntry(string.Empty);
            traceScreensMenuEntry = new MenuEntry(string.Empty);
            cursorDebugMenuEntry = new MenuEntry(string.Empty);
            backMenuEntry = new MenuEntry(string.Empty);

            // Hook up menu event handlers.
            fullscreenMenuEntry.Selected += FullScreenMenuEntrySelected;
            cursorDebugMenuEntry.Selected += CursorDebugMenuEntrySelected;
            traceScreensMenuEntry.Selected += TraceScreensMenuEntrySelected;
            particleEffectMenuEntry.Selected += ParticleEffectMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entires to the menu.
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(cursorDebugMenuEntry);
            MenuEntries.Add(traceScreensMenuEntry);
            MenuEntries.Add(particleEffectMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

        /// <summary>
        /// Loads content for the settings screen, including lazy loading services and setting initial values.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            // Lazy load some things
            gdm ??= ScreenManager.Game.Services.GetService<GraphicsDeviceManager>();

            settingsManager ??= ScreenManager.Game.Services.GetService<SettingsManager<GameSettings>>();

            settingsManager.Settings.PropertyChanged += (s, e) =>
            {
                SetLanguageText();

                settingsManager.Save();
            };

            currentParticleEffect = settingsManager.Settings.ParticleEffect;
            gdm.IsFullScreen = settingsManager.Settings.FullScreen;

            SetLanguageText();

            particleManager ??= ScreenManager.Game.Services.GetService<ParticleManager>();
        }

        /// <summary>
        /// Updates the settings screen, including particle effects.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="otherScreenHasfocus">Indicates whether another screen has focus.</param>
        /// <param name="coveredByOtherScreen">Indicates whether the screen is covered by another screen.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasfocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasfocus, coveredByOtherScreen);

            particleManager.Update(gameTime);
        }

        /// <summary>
        /// Draws the settings screen, including particle effects.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.Game.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ScreenManager.GlobalTransformation);
            //spriteBatch.Draw(ScreenManager.Game.Content.Load<Texture2D>("Backgrounds/Layer0_2"), Vector2.Zero, Color.White);
            //spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ScreenManager.GlobalTransformation);

            particleManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Filles in the latest values for the options screen menu text.
        /// </summary>
        private void SetLanguageText()
        {
            fullscreenMenuEntry.Text = $"Display Mode: {(gdm.IsFullScreen ? "Fullscreen" : "Windowed")}";

            traceScreensMenuEntry.Text = $"Trace Screens: {ScreenManager.TraceEnabled}";

            cursorDebugMenuEntry.Text = $"Draw Cursor Rectangles: {ScreenManager.CursorDebug}";

            particleEffectMenuEntry.Text = "Particle Effect: " + currentParticleEffect;

            backMenuEntry.Text = "Back";

            Title = "Settings";
        }
        /// <summary>
        /// Event handler for when the Fullscreen menu entry is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            gdm.ToggleFullScreen();

            settingsManager.Settings.FullScreen = gdm.IsFullScreen;
        }
        private void TraceScreensMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.TraceEnabled = !ScreenManager.TraceEnabled;
            traceScreensMenuEntry.Text = $"Trace Screens: {ScreenManager.TraceEnabled}";
        }
        private void CursorDebugMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.CursorDebug = !ScreenManager.CursorDebug;
            cursorDebugMenuEntry.Text = $"Draw Cursor Rectangles: {ScreenManager.CursorDebug}";
        }
        
        /// <summary>
        /// Event handler for when the Particle menu entry is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParticleEffectMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentParticleEffect++;

            if (currentParticleEffect > ParticleEffectType.Sparkles)
                currentParticleEffect = 0;

            settingsManager.Settings.ParticleEffect = currentParticleEffect;

            particleManager.Emit(100, currentParticleEffect); // Emit 100 particles
        }
    }
}
