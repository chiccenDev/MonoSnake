
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoSnake.Core
{
    internal class Player
    {
        // Determines if the sprite is flipped horizontally based on movement direction
        private SpriteEffects flip = SpriteEffects.None;

        // Manages the sprite for the Snake head and tail
        private Sprite sprite;
        public Sprite Sprite { get => sprite; }

        public Rectangle SourceRect
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    sprite.Width,
                    sprite.Height
                    );
            }
        }

        // Sound Effects
        private SoundEffect killedSound;

        private FaceDirection direction = FaceDirection.Up;

        /// <summary>
        /// Player moves only when timer hits 0, so timer is essentially the player's "speed".
        /// Input, etc. is still handled in real time, so the player can change their mind as
        /// to which direction they would like to travel as long as the corrective input is
        /// made before the timer runs out.
        /// 
        /// In short: the lower the timer, the faster the player moves (updates).
        /// </summary>
        private TimeSpan timer;

        private World world;
        public World World
        {
            get { return world; }
        }

        private Tail tail;
        public Tail Tail { get => tail; }

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

        Vector2 previousPosition;
        public Vector2 PreviousPosition { get => previousPosition; }

        Vector2 nextPosition = Vector2.Zero;
        public Vector2 NextPosition {  get => nextPosition; }


        /// <summary>
        /// Horizontal movement input value. -1.0 for left, 1.0 for right, 0.0 for no movement.
        /// </summary>
        private float movement;
        public float Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        /// <summary>
        /// Vertical movement input value. -1.0 for up, 1.0 for down, 0.0 for no movement.
        /// </summary>
        private float movement_vertical;
        public float Movement_Vertical { get { return movement_vertical; } set { movement_vertical = value; } }

        // Current player mode (e.g., Playing, Scripted Movement)
        public GameState Mode { get; internal set; }

        public Player(World world, Vector2 position)
        {
            this.world = world;

            this.color = Color.White;

            LoadContent();

            Reset(position);
        }

        private void ResetTimer() => timer = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// Loads all player-related content: sprites, animations, and sound effects.
        /// </summary>
        public void LoadContent()
        {
            // Load texture
            sprite = new Sprite(World.Content.Load<Texture2D>("Sprites/snake"));

            // Load sounds.
            killedSound = World.Content.Load<SoundEffect>("Sounds/PlayerKilled");
        }

        /// <summary>
        /// Resets the player to life at the specified position.
        /// Called at the beginning of a level and when respawning after death.
        /// </summary>
        /// <param name="position">The position to respawn at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            isAlive = true;
            ResetTimer();
        }

        /// <summary>
        /// Updates the player's state based on input, physics, and animations.
        /// Main update method called each frame.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        /// <param name="inputState">Current state of all input devices.</param>
        /// <param name="displayOrientation">Orientation of the display for accelerometer adjustments.</param>
        public void Update(GameTime gameTime, InputState inputState, DisplayOrientation displayOrientation)
        {
            if (Mode != GameState.Playing && inputState.CurrentKeyboardState.GetPressedKeyCount() > 0)
                Mode = GameState.Playing;

            // Only process input if the player is in playing mode
            if (Mode == GameState.Playing && isAlive)
                HandleInput(inputState, displayOrientation);
                nextPosition = new Vector2(position.X + (16 * movement), position.Y + (16 * movement_vertical));
            // Update Movement Timer and move if the timer is up.
            timer -= gameTime.ElapsedGameTime;

            if (timer > TimeSpan.Zero)
                return;

            tail?.Update();
            ResetTimer();

            Move(gameTime);
        }

        /// <summary>
        /// Updates player's position, applies physics, and updates animations.
        /// Called each frame after input is processed.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        public void Move(GameTime gameTime)
        {
            if (!isAlive) return;
            if (CheckTail(nextPosition))
            {
                isAlive = false;
                Debug.WriteLine("You Died!");
            }

            if (movement != 0f) direction = (movement > 0f) ? FaceDirection.Right : FaceDirection.Left;
            if (movement_vertical != 0f) direction = (movement_vertical > 0f) ? FaceDirection.Down : FaceDirection.Up;

            previousPosition = position;
            position = nextPosition;
            world.tails.Clear();
            tail?.Update();
            CheckFruit();

            Debug.WriteLine($"Snake: {position}");
            Debug.WriteLine($"Tail: {tail?.Position}");
        }

        /// <summary>
        /// Processes player input from keyboard and mouse.
        /// Sets movement direction and jump state based on input.
        /// </summary>
        /// <param name="inputState">Current state of all input devices.</param>
        /// <param name="displayOrientation">Orientation fo the display for accelerometer adjustments.</param>
        private void HandleInput(InputState inputState, DisplayOrientation displayOrientation)
        {
            // Get list of all currently pressed keys.
            Keys[] pressedKeys = inputState.CurrentKeyboardState.GetPressedKeys();

            // Process keyboard input for movement.
            if ((pressedKeys.Contains(Keys.A)) && direction != FaceDirection.Right)
            {
                movement = -1.0f;
                movement_vertical = 0f;
            }
            if ((pressedKeys.Contains(Keys.D)) && direction != FaceDirection.Left)
            {
                movement = 1.0f;
                movement_vertical = 0f;
            }
            if ((pressedKeys.Contains(Keys.W)) && direction != FaceDirection.Down)
            {
                movement_vertical = -1.0f;
                movement = 0f;
            }
            if ((pressedKeys.Contains(Keys.S)) && direction != FaceDirection.Up)
            {
                movement_vertical = 1.0f;
                movement = 0f;
            }

            if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed)
                HandleClickInput(inputState.CurrentCursorLocation);
        }

        /// <summary>
        /// Processes mouse input. Currently, only used for debugging purposes.
        /// </summary>
        /// <param name="clickPosition"></param>
        private void HandleClickInput(Vector2 clickPosition)
        {
            Debug.WriteLine($"Mouse: {clickPosition}");
            //SpawnTail();
        }

        private void SpawnTail()
        {
            if (tail is not null)
                tail.SpawnTail();
            else
            {
                if (previousPosition == position) previousPosition = Tile.Size;
                this.tail = new Tail(world, null, previousPosition, color);
            }
                
        }

        private bool CheckTail(Vector2 position)
        {
            if (world.tails.ContainsKey(position))
            {
                OnKilled();
                return true;
            }
            return false;
        }

        private bool CheckFruit()
        {
            if (world.GetFruit(position))
            {
                SpawnTail(); // TODO: add out var message to output if spawning tail was a success and if not, why.
                world.SpawnFruit();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles player death, either from enemies or environmental hazards.
        /// Plays death animation and sound effect.
        /// </summary>
        public void OnKilled()
        {
            isAlive = false;

            // Play death sound
            killedSound.Play();
            Debug.WriteLine($"{world.tails.Count}");

            // Play death animation
            // TBD
        }

        /// <summary>
        /// Draws the player with appropriate animation, facing direction, and color effects.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        /// <param name="spriteBatch">SpriteBatch used for drawing.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Get sprite index and flip based on movement vertical/horizontal.
            switch (direction)
            {
                case FaceDirection.Left:
                    flip = SpriteEffects.FlipHorizontally;
                    sprite.Index = 1;
                    break;
                case FaceDirection.Right:
                    flip = SpriteEffects.None;
                    sprite.Index = 1;
                    break;
                case FaceDirection.Down:
                    flip = SpriteEffects.FlipVertically;
                    sprite.Index = 0;
                    break;
                case FaceDirection.Up:
                    flip = SpriteEffects.None;
                    sprite.Index = 0;
                    break;
            }

            // Apply color effects
            Color color = Color.White;

            // Draw the player sprite with current animation, position, and effects
            sprite.Draw(gameTime, spriteBatch, Position, flip, color);
            if (tail != null)
                tail.Draw(gameTime, spriteBatch);
        }
    }
}
