using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Core
{
    internal struct Tile
    {
        /// <summary>
        /// The texture that represents the tile's visual appearance.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The source rectangle for the tile.
        /// </summary>
        public Rectangle SourceRect = new Rectangle(0, 0, 16, 16);

        /// <summary>
        /// The type of collision behavior this tile exhibits.
        /// </summary>
        public TileCollision Collision;

        /// <summary>
        /// The standars width of a tile, measured in pixels.
        /// </summary>
        public const int Width = 16;

        /// <summary>
        /// The standard height of a tile, measures in pixels.
        /// </summary>
        public const int Height = 16;

        /// <summary>
        /// The size of a tile as a <see cref="Vector2"/> for convenience.
        /// </summary>
        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> struct.
        /// </summary>
        /// <param name="texture">The texture representing the tile's appearance. <see cref="Texture2D"/></param>
        /// <param name="collision">The collision type that defines the tile's behavior. <see cref="TileCollision"/></param>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
