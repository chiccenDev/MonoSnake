using System;
using Microsoft.Xna.Framework;

namespace MonoSnake.Effects
{
    /// <summary>
    /// The data for a single particle in the particle system.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// The amount of drag applies to velocity per second,
        /// Help simulate some air resistance or even gravity.
        /// </summary>
        public float DragPerSecond = 0.9f;

        /// <summary>
        /// The current color of the particle.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The direction that this particle is moving in.
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        /// The total lifetime of the particle.
        /// </summary>
        public float LifeTime;

        /// <summary>
        /// As LifeTime iteself changes over time.
        /// </summary>
        public float InitialLifeTime;

        /// <summary>
        /// The position of the particle.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The previous position of the particle.
        /// </summary>
        public Vector2 PreviousPosition;

        /// <summary>
        /// How much to scale the particle.
        /// </summary>
        public float Scale;

        /// <summary>
        /// The length of the lines drawn for each particle.
        /// </summary>
        public float TailLength;

        /// <summary>
        /// The velocity of the particle.
        /// </summary>
        Vector2 Velocity;

        /// <summary>
        /// Check if the particle is still alive.
        /// </summary>
        public bool IsAlive => LifeTime > 0;

        /// <summary>
        /// Triggered when the particle "dies".
        /// Be careful of circular referencing emitters or you'll have endles particles.
        /// </summary>
        public event Action<Vector2> OnDeath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Particle"/> class.
        /// </summary>
        /// <param name="position">The position of the particle.</param>
        /// <param name="direction">The direction that this particle is moving in.</param>
        /// <param name="speed">The speed at which the particle travels in pixels per second.</param>
        /// <param name="lifetime">The total lifetime of the particle.</param>
        /// <param name="color">The color of the particle.</param>
        /// <param name="scale">How much to scale the particle.</param>
        /// <param name="tailLength">The length of the lines drawn for each particle.</param>
        public Particle(Vector2 position, Vector2 direction, float speed, float lifetime, Color color, float scale, float tailLength = 0f)
        {
            Position = position;
            PreviousPosition = position;
            Velocity = direction * speed;
            LifeTime = lifetime;
            InitialLifeTime = lifetime;
            Color = color;
            Scale = scale;
            TailLength = tailLength;
        }

        public void Update(GameTime gameTime)
        {
            // Get elapse time in seconds
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Avoid further processing if time is frozen (e.g. game paused)
            if (elapsedTime <= 0) return;

            // Store the previous position before updating
            PreviousPosition = Position;

            // Update particle's position based on velocity and elapsed time
            Position += Velocity * elapsedTime;

            // Apply drag to reduce velocity over time (simulates air resistance)
            float dragFactor = Math.Max(1 - (elapsedTime * DragPerSecond), 0);
            Velocity *= dragFactor;

            // Reduce the particle's remaining lifespan
            LifeTime -= elapsedTime;

            // Gradually fade the particle's color based on remaining life
            Color.A = (byte)(255f * LifeTime / InitialLifeTime);

            // Trigger death event if the particle's lifespan has expired
            if (!IsAlive) OnDeath?.Invoke(Position);
        }
    }
}
