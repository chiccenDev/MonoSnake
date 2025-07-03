using Microsoft.Xna.Framework;

namespace MonoSnake.Core
{
    internal struct Circle
    {
        /// <summary>
        /// Center position of the circle.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Construct a new circle.
        /// </summary>
        /// <param name="position">The <see cref="Vector2"> position which will be the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public Circle(Vector2 position, float radius)
        {
            Center = position;
            Radius = radius;
        }

        public bool Intersects(Rectangle rectangle)
        {
            // Find the closest point on the rectangle to the circle's center.
            // This ensured the point lies within the rectangle's bounds.
            Vector2 closestPoint = new Vector2(
                MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right),
                MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom)
            );

            // Calculate the vector from the circle's center to the closest point.
            Vector2 direction = Center - closestPoint;

            // Calculate the squared distance (avoids costlier square root operation)
            float distanceSquared = direction.LengthSquared();

            // The circle and rectangle intersect if this distance is less than the circle's radius squared.
            return ((distanceSquared > 0) && (distanceSquared < Radius * Radius));
        }
    }
}
