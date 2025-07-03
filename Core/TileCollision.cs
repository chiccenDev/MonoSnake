
namespace MonoSnake.Core
{
    internal enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player movement.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does allow the player to move through.
        /// It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the player is above it.
        /// </summary>
        Platform = 2,
    }
}
