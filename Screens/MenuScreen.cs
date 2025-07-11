﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Core;

namespace MonoSnake.Screens
{
    /// <summary>
    /// Base class for screen that containg a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    internal abstract class MenuScreen : GameScreen
    {
        private List<MenuEntry> menuEntries = new();
        private int selectedEntry = 0;
        private string menuTitle;
        // Default color is black. Use new Color(192, 192, 192) for off-white.
        private Color menuTitleColor = Color.White;
        public Texture2D background;
        private Rectangle backgroundRectangle = Rectangle.Empty;

        // The background includes a border somewhat larger than the text itself.
        private const int hPad = 32;
        private const int vPad = 16;

        private MenuEntry longestEntry;

        /// <summary>
        /// Gets or sets the title of the menu screen.
        /// </summary>
        public string Title { get => menuTitle; set => menuTitle = value; }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuScreen"/> class.
        /// </summary>
        /// <param name="menuTitle">The title of the menu screen.</param>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;
            selectedEntry = 0;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads content for the menu screen. This method is called once per game
        /// and is the place to load all content specific to the menu screen.
        /// </summary>
        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("UI/menuPixel");

            base.LoadContent();
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or canceling the menu.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="inputState">Provides the current state of input devices.</param>
        public override void HandleInput(GameTime gameTime, InputState inputState)
        {
            base.HandleInput(gameTime, inputState);

            // Handle mouse input for desktop platforms.
            if (Game1.IsDesktop)
            {
                if (inputState.IsLeftMouseButtonClicked())
                    TextSelectedCheck(inputState.CurrentCursorLocation);
                else if (inputState.IsMiddleMouseButtonClick())
                    OnSelectEntry(selectedEntry, PlayerIndex.One);
                else { TextHoveredCheck(inputState.CurrentCursorLocation); }
            }

            //Move to the previous menu entry.
            if (inputState.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                while (!menuEntries[selectedEntry].Enabled)
                {
                    selectedEntry--;

                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }
            }

            // Move to the next menu entry.
            if (inputState.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                SetNextEnabledMenu();
            }

            // Accept or cancel the menu.
            if (inputState.IsMenuSelect())
                OnSelectEntry(selectedEntry);
            else if (inputState.IsMenuCancel())
                OnCancel();
        }

        /// <summary>
        /// Checks is a mouse click has selected a menu entry.
        /// </summary>
        /// <param name="touchLocation">The location of the mouse click.</param>
        private void TextSelectedCheck(Vector2 touchLocation)
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var textSize = ScreenManager.Font.MeasureString(menuEntries[i].Text);
                var entryBounds = new Rectangle((int)menuEntries[i].Position.X, (int)menuEntries[i].Position.Y - (int)(textSize.Y / 2), (int)textSize.X, (int)textSize.Y);

                if (entryBounds.Contains(touchLocation))
                {
                    selectedEntry = i;
                    OnSelectEntry(selectedEntry);
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if the mouse is hovering over a menu entry.
        /// </summary>
        /// <param name="touchLocation">The location where the mouse is hovering.</param>
        private void TextHoveredCheck(Vector2 mouseLocation)
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var textSize = ScreenManager.Font.MeasureString(menuEntries[i].Text);
                var entryBounds = new Rectangle((int)menuEntries[i].Position.X, (int)menuEntries[i].Position.Y - (int)(textSize.Y / 2), (int)textSize.X, (int)textSize.Y);

                if (entryBounds.Contains(mouseLocation))
                {
                    selectedEntry = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the next enabled menu entry as the selected entry.
        /// </summary>
        private void SetNextEnabledMenu()
        {
            while (!menuEntries[selectedEntry].Enabled)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }
        }

        /// <summary>
        /// Handler for when the user has canceled the menu.
        /// </summary>
        /// <param name="entryIndex">The index of the selected menu entry.</param>
        /// <param name="playerIndex">The index of the player who triggered the selection.</param>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex = PlayerIndex.One)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has canceled the menu.
        /// </summary>
        /// <param name="playerIndex">The index of the player who triggered the cancellation.</param>
        protected virtual void OnCancel(PlayerIndex playerIndex = PlayerIndex.One)
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments containig the player index.</param>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Updates the positions of the menu entries. by default, all menu entries
        /// are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look for interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Start at Y = 175; each X value is generated per entry.
            Vector2 position = new Vector2(0f, ScreenManager.BaseScreenSize.Y * 0.375f); // 3/8 from the top of the screen

            // Update each menu entry's location in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                if (longestEntry == null || menuEntry.Text.Length > longestEntry.Text.Length)
                    longestEntry = menuEntry;

                // Each entry is to be centered horizontally.
                position.X = ScreenManager.BaseScreenSize.X / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                menuEntry.Position = position;

                position.Y += menuEntry.GetHeight(this);
            }
        }

        /// <summary>
        /// Updates the menu screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="otherScreenHasfocus">Whether another screen currently has focus.</param>
        /// <param name="coveredByOtherScreen">Whether this screen is covered by another screen.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasfocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasfocus, coveredByOtherScreen);

            SetNextEnabledMenu();

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // Make sure our entries are in the right place before we draw them.
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ScreenManager.GlobalTransformation);

            //spriteBatch.Draw(background, Vector2.Zero, Color.White);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Get menu title information for centering on screen.
            Vector2 titlePosition = new Vector2(ScreenManager.BaseScreenSize.X / 2, ScreenManager.BaseScreenSize.Y / 5);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = menuTitleColor * TransitionAlpha;
            float titleScale = 1.25f;

            if (this.IsPopup)
            {
                if (backgroundRectangle.IsEmpty)
                {
                    // Center the message text in the BaseScreenSize.
                    // The GlobalTransformation will scale everything for us.
                    Vector2 textSize = ScreenManager.Font.MeasureString(longestEntry.Text);
                    var yOffset = menuEntries[0].Position.Y - titlePosition.Y;
                    var xOffset = titlePosition.X - (ScreenManager.BaseScreenSize.X / 2 - longestEntry.GetWidth(this) / 2);
                    textSize.Y += (textSize.Y * MenuEntries.Count) + yOffset;
                    var messageTextPosition = (ScreenManager.BaseScreenSize - textSize) / 2;

                    backgroundRectangle = new Rectangle((int)titlePosition.X - hPad - (int)xOffset,
                                                    (int)titlePosition.Y - vPad - (int)(yOffset / 4),
                                                    (int)textSize.X + hPad * 2,
                                                    (int)textSize.Y + vPad * 2);
                }

                spriteBatch.Draw(background, backgroundRectangle, Color.White * TransitionAlpha);
            }
            

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {

                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Draw menu title using information collected above, including transition offset for transitions on and off screen.
            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
