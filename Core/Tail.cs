using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoSnake.Core
{
    internal class Tail // TODO: add documentation to all this stuff.
    {
        private Sprite sprite;
        public Sprite Sprite {  get =>  sprite; set => sprite = value; }
        Tail previousTail;
        Tail nextTail;
        bool ready = false;

        Rectangle localBounds;
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        private World world;
        public World World { get => world; }

        private bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
        }

        private Color color;
        /// <summary>
        /// Gets or sets the player's color for drawing.
        /// </summary>
        public Color Color { get => color; set => color = value; }

        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Vector2 previousPosition;
        public Vector2 PreviousPosition { get => previousPosition; }

        public Tail(World world, Tail previousTail, Vector2 position, Color color)
        {
            this.world = world;
            this.previousTail = previousTail;
            this.position = position;
            LoadContent();
            world.tails.TryAdd(position, this);
        }

        public void LoadContent()
        {
            // Load texture
            sprite = new Sprite(World.Content.Load<Texture2D>("Sprites/blank"));

            // Create collision bounds - smaller than the sprite for better gameplay feel
            localBounds = new Rectangle(0, 0, sprite.Width, sprite.Height);
            ready = true;
        }

        public void SpawnTail()
        {
            if (nextTail == null)
                nextTail = new Tail(world, this, previousPosition, color);
            else
                nextTail.SpawnTail();
        }

        public void Update()
        {
            isAlive = previousTail?.isAlive ?? world.Player.IsAlive;
            if (isAlive)
            {
                if (previousTail != null)
                    Update(previousTail.PreviousPosition);
                else
                {
                    Update(world.Player.PreviousPosition);
                }
            }

        }

        public void Update(Vector2 position)
        {
            previousPosition = this.position;
            this.position = position;
            world.tails.TryAdd(position, this);

            nextTail?.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!ready) return;

            sprite.Draw(gameTime, spriteBatch, position, SpriteEffects.None);
            if (nextTail != null)
                nextTail.Draw(gameTime, spriteBatch);
        }
    }
}
