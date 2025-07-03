
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoSnake.Core
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public KeyboardState CurrentKeyboardState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public MouseState LastMouseState;

        /// <summary>
        /// Cursor move speed in pixels per second
        /// </summary>
        private const float cursorMoveSpeed = 250.0f;

        private Vector2 currentCursorLocation;
        /// <summary>
        /// Current location of our Cursor
        /// </summary>
        public Vector2 CurrentCursorLocation => currentCursorLocation;

        private Vector2 lastCursorLocation;
        /// <summary>
        /// Last location of our Cursor
        /// </summary>
        public Vector2 LastCursorLocation => currentCursorLocation;

        private bool isMouseWheelScrollDown;
        /// <summary>
        /// Has the user scrolled the mouse wheel down?
        /// </summary>
        public bool IsMouseWheelScrollDown => isMouseWheelScrollDown;

        private bool isMouseWheelScrollUp;
        /// <summary>
        /// Has the user scrolled the mouse wheel down?
        /// </summary>
        public bool IsMouseWheelScrollUp => isMouseWheelScrollUp;

        private Matrix inputTransformation; // Used to transform input coordinates between screen and game space

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            if (!Game1.IsDesktop) throw new PlatformNotSupportedException();
        }
        /// <summary>
        /// Reads the latest state of the input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshop of timing values.</param>
        /// <param name="viewport">The viewport to constrain cursor movement within.</param>
        public void Update(GameTime gameTime, Viewport viewport)
        {
            // Update keyboard state
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            // Update mouse state
            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            if (IsLeftMouseButtonClicked())
            {
                lastCursorLocation = currentCursorLocation;
                // Transform mouse position to game coordinates
                currentCursorLocation = transformCursorLocation(new Vector2(CurrentMouseState.X, CurrentMouseState.Y));
                //touchCount = 1;
            }
            if (IsMiddleMouseButtonClick())
                Debug.WriteLine("Middle mouse clicked!");

            // Reset mouse wheel flags
            isMouseWheelScrollDown = false;
            isMouseWheelScrollUp = false;

            // Detect mouse wheel scrolling
            if (CurrentMouseState.ScrollWheelValue != LastMouseState.ScrollWheelValue)
            {
                int scrollWheelDelta = CurrentMouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue;

                // Handle the scroll wheel event based on the delta
                if (scrollWheelDelta > 0) isMouseWheelScrollUp = true;
                else if (scrollWheelDelta < 0) isMouseWheelScrollDown = true;
            }

            // Update the cursor location using keyboard
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move cursor with keyboard arrow keys
            if (CurrentKeyboardState.IsKeyDown(Keys.Up))
                currentCursorLocation.Y -= deltaTime * cursorMoveSpeed;
            if (CurrentKeyboardState.IsKeyDown(Keys.Down))
                currentCursorLocation.Y += deltaTime * cursorMoveSpeed;
            if (CurrentKeyboardState.IsKeyDown(Keys.Left))
                currentCursorLocation.X -= deltaTime * cursorMoveSpeed;
            if (CurrentKeyboardState.IsKeyDown(Keys.Right))
                currentCursorLocation.X += deltaTime * cursorMoveSpeed;

            currentCursorLocation.X = MathHelper.Clamp(currentCursorLocation.X, 0f, viewport.Width);
            currentCursorLocation.Y = MathHelper.Clamp(currentCursorLocation.Y, 0f, viewport.Height);
        }
        /// <summary>
        /// Checks if left mouse button was clicked (pressed and then released)
        /// </summary>
        /// <returns>True if left mouse button was clicked, false otherwise.</returns>
        internal bool IsLeftMouseButtonClicked()
        {
            return CurrentMouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed;
        }
        /// <summary>
        /// Checks if middle mouse button was clicked (pressed and then released)
        /// </summary>
        /// <returns>True if middle mouse button was clicked, false otherwise.</returns>
        internal bool IsMiddleMouseButtonClick()
        {
            return CurrentMouseState.MiddleButton == ButtonState.Released && LastMouseState.MiddleButton == ButtonState.Pressed;
        }
        /// <summary>
        /// Checks if right mouse button was clicked (pressed and then released)
        /// </summary>
        /// <returns>True if right mouse button was clicked, false otherwise.</returns>
        internal bool IsRightMouseButtonClicked()
        {
            return CurrentMouseState.RightButton == ButtonState.Released && LastMouseState.RightButton == ButtonState.Pressed;
        }
        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was newly pressed, false otherwise.</returns>
        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
        }
        /// <summary>
        /// Checks for a "menu select" input action.
        /// </summary>
        /// <returns>True if menu select action occurred, false otherwise.</returns>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter);
        }
        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// </summary>
        /// <returns>True if menu cancel action occurred, false otherwise.</returns>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape);
        }
        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        /// <returns>True if menu up action occurred, false otherwise.</returns>
        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up) || IsNewKeyPress(Keys.W) || IsMouseWheelScrollUp;
        }
        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        /// <returns>True if menu cancel down occurred, false otherwise.</returns>
        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down) || IsNewKeyPress(Keys.S) || IsMouseWheelScrollDown;
        }
        /// <summary>
        /// Checks for a "pause game" input action.
        /// </summary>
        /// <param name="rectangle">Optional rectangle to check for clicks within.</param>
        /// <returns>True if pause action occurred, false otherwise.</returns>
        public bool IsPauseGame(Rectangle? rectangle = null)
        {
            bool pointInRect = false;

            // Check if the cursor is in the provided rectangle and was clicked
            if (rectangle.HasValue)
                if (rectangle.Value.Contains(CurrentCursorLocation) && (IsLeftMouseButtonClicked())) pointInRect = true;

            return IsNewKeyPress(Keys.Escape) || pointInRect;
        }
        /// <summary>
        /// Checks if the player has selected next.
        /// </summary>
        /// <returns>True if select next action occurrred, false otherwise.</returns>
        public bool IsSelectNext()
        {
            return IsNewKeyPress(Keys.Right) || IsNewKeyPress(Keys.D);
        }
        /// <summary>
        /// Checks if the player has selected previous.
        /// </summary>
        /// <returns>True if select previous action occurrred, false otherwise.</returns>
        public bool IsSelectPrevious()
        {
            return IsNewKeyPress(Keys.Left) || IsNewKeyPress(Keys.A);
        }
        /// <summary>
        /// Updates the matrix used to transform input coordinates.
        /// </summary>
        /// <param name="inputTransformation">the transformation matrix to apply</param>
        internal void UpdateInputTransformation(Matrix inputTransformation)
        {
            this.inputTransformation = inputTransformation;
        }
        /// <summary>
        /// Transforms mouse positions from screen space to game space.
        /// </summary>
        /// <param name="mousePosition">The screen-space position to transform.</param>
        /// <returns>The transformed position in game space.</returns>
        public Vector2 transformCursorLocation(Vector2 mousePosition)
        {
            // Transform back to cursor location
            return Vector2.Transform(mousePosition, inputTransformation);
        }
        internal bool IsUIClicked(Rectangle rectangle)
        {
            return (rectangle.Contains(CurrentCursorLocation) && (IsLeftMouseButtonClicked()));
        }
    }
}
