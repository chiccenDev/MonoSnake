using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoSnake.Screens
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggerd the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    internal class PlayerIndexEventArgs : EventArgs
    {
        PlayerIndex playerIndex;
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }
}
