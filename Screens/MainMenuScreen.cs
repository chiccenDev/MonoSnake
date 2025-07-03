
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Core;
using MonoSnake.Effects;
using MonoSnake.Settings;

namespace MonoSnake.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    internal class MainMenuScreen : MenuScreen
    {
        private ContentManager content;
        private bool readyToPlay;
        private ParticleManager particleManager;
        private SettingsManager<GameSettings> settingsManager;
        private MenuEntry playMenuEntry;
        //private MenuEntry tutorialMenuEntry;
        private MenuEntry settingsMenuEntry;
        private MenuEntry aboutMenuEntry;
        private MenuEntry exitMenuEntry;
        private Texture2D gradientTexture;
        private int tutorialStep = -1;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            playMenuEntry = new MenuEntry("Play");
            //tutorialMenuEntry = new MenuEntry("Tutorial");
            settingsMenuEntry = new MenuEntry("Settings");
            aboutMenuEntry = new MenuEntry("About");
            exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playMenuEntry.Selected += PlayMenuEntrySelected;
            //tutorialMenuEntry.Selected += TutorialMenuEntrySelected;
            settingsMenuEntry.Selected += SettingsMenuEntrySelected;
            aboutMenuEntry.Selected += AboutMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add the entries to the menu.
            MenuEntries.Add(playMenuEntry);
            //MenuEntries.Add(tutorialMenuEntry);
            MenuEntries.Add(settingsMenuEntry);
            MenuEntries.Add(aboutMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void SetLanguageText()
        {
            aboutMenuEntry.Text = "About";
            playMenuEntry.Text = "Play";
            settingsMenuEntry.Text = "Settings";
            //tutorialMenuEntry.Text = Resources.Tutorial;
            exitMenuEntry.Text = "Exit";

            Title = "~ Mono Snake ~";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content for the game.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            particleManager ??= ScreenManager.Game.Services.GetService<ParticleManager>();

            settingsManager ??= ScreenManager.Game.Services.GetService<SettingsManager<GameSettings>>();
            settingsManager.Settings.PropertyChanged += (s, e) =>
            {
                SetLanguageText();
            };

            SetLanguageText();

            gradientTexture = content.Load<Texture2D>("Sprites/gradient");
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// 
        /// This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="otherScreenHasfocus">If another screen has focus.</param>
        /// <param name="coveredByOtherScreen">If currently covered by another screen.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasfocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasfocus, coveredByOtherScreen);

            if (readyToPlay)
            {
                //level.Player.Movement = 1.0f;

                //level.Player.Move(gameTime);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState inputState)
        {
            base.HandleInput(gameTime, inputState);

            // Update the level, passing down the GameTime along with all of our input states
            //level.Update(gameTime, inputState, ScreenManager.Game.Window.CurrentOrientation, readyToPlay);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.Game.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //level.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        private void DrawTutorialSteps(SpriteBatch spriteBatch)
        {
            SpriteFont font = ScreenManager.Font;

            // The background includes a border somewhat larger than the text itself/
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = Rectangle.Empty;
            Vector2 textSize;
            string message = string.Empty;

            switch (tutorialStep)
            {
                case 0:
                    message = "First tutorial message!";
                    textSize = font.MeasureString(message);
                    // MISSING
                    break;
                case 1:
                    message = "Second tutorial message!";
                    textSize = font.MeasureString(message);
                    // MISSING
                    break;
                case 2:
                    message = "Don't die.";
                    textSize = font.MeasureString(message);
                    // MISSING
                    break;
                case 3:
                    message = "Tap to pause";
                    textSize = font.MeasureString(message);
                    // MISSING
                    break;
            }

            Vector2 textPosition = new Vector2(backgroundRectangle.X + hPad, backgroundRectangle.Y + vPad);

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, Color.White);

            // Draw the tutorial text.
            spriteBatch.DrawString(font, message, textPosition, Color.White);

        }

        /// <summary>
        /// Event handler for when the Play menu entry is selected.
        /// </summary>
        void PlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var toastMessageBox = new MessageBoxScreen("Let's GO!!", false, new TimeSpan(0, 0, 1), true);
            toastMessageBox.Accepted += (sender, e) =>
            {
                readyToPlay = true;
            };

            LoadingScreen.Load(ScreenManager, false, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void SettingsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new SettingsScreen());
        }

        /// <summary>
        /// Event handler for when the About emnu entry is selected.
        /// </summary>
        void AboutMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new AboutScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex = PlayerIndex.One)
        {
            string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }

        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
