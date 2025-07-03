using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;

namespace MonoSnake.Screens
{
    /// <summary>
    /// Represents the "About" screen, providing information about the game and its technology.
    /// This screen displays credits and links to the MonoGame website as well as my own relevant socials.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="MenuScreen"/>, inheriting its menu management capabilities.
    /// </remarks>
    internal class AboutScreen : MenuScreen
    {
        private MenuEntry monoGameWebsiteMenuEntry;
        private MenuEntry createdByMenuEntry;
        private MenuEntry githubMenuEntry;

        public AboutScreen()
            : base("About")
        {
            // Create the static label entry. Disabled as it's a label.
            createdByMenuEntry = new MenuEntry("Created by chiccen using MonoGame", false);
            // Create the clickable link entries.
            monoGameWebsiteMenuEntry = new MenuEntry("MonoGameSite");
            githubMenuEntry = new MenuEntry("chiccenDev on Github");

            // Create the "Back" button entry.
            MenuEntry back = new MenuEntry("Back");

            // Attach event handlers for menu entry selections.
            monoGameWebsiteMenuEntry.Selected += MonoGameWebsiteMenuSelected;
            githubMenuEntry.Selected += GithubWebsiteMenuSelected;
            back.Selected += OnCancel;

            // Add the menu entries to the screen.
            MenuEntries.Add(createdByMenuEntry);
            MenuEntries.Add(monoGameWebsiteMenuEntry);
            MenuEntries.Add(githubMenuEntry);
            MenuEntries.Add(back);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.Game.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

        }

        /// <summary>
        /// handles the selection event for the MonoGame website menu entry.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        private void MonoGameWebsiteMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            LaunchDefaultBrowser("https://monogame.net/");
        }

        /// <summary>
        /// handles the selection event for the MonoGame website menu entry.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        private void GithubWebsiteMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            LaunchDefaultBrowser("https://github.com/chiccendev/");
        }

        private static void LaunchDefaultBrowser(string url)
        {
            // UseShellExecute is crucial for launching the default browser.
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
