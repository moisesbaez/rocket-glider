using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Glider
{
    static class CameraManager
    {
        #region Declarations
        private static Vector2 position = Vector2.Zero;  //stores position of the camera in the game world
        private static Vector2 viewPortSize = Vector2.Zero;  //stores the diagonal position of the camera
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0);  //stores the whole game world region
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the camera position.
        /// </summary>
        public static Vector2 Position
        {
            get { return position; }
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, worldRectangle.X, worldRectangle.Width - ViewPortWidth), MathHelper.Clamp(value.Y, worldRectangle.Y, worldRectangle.Height - ViewPortHeight));  //enforces limitations so the camera doesn't point outside the game world
            }
        }

        /// <summary>
        /// Gets or sets the world size.
        /// </summary>
        public static Rectangle WorldRectangle
        {
            get { return worldRectangle; }
            set { worldRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the width of the viewable screen.
        /// </summary>
        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }

        /// <summary>
        /// Gets or sets the height of the viewable screen
        /// </summary>
        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }

        /// <summary>
        /// Returns the viewable screen position.
        /// </summary>
        public static Rectangle ViewPort
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, ViewPortWidth, ViewPortHeight);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Moves the camera
        /// </summary>
        /// <param name="offset">Moves the camera position by a certain value.</param>
        public static void MoveCamera(Vector2 offset)
        {
            Position += offset;
        }

        /// <summary>
        /// Determines if the object to be draw falls within the camera.
        /// </summary>
        /// <param name="bounds">Position of the object.</param>
        /// <returns>True if the object falls within the bounds of the camera.</returns>
        public static bool ObjectIsVisible(Rectangle bounds)
        {
            return (ViewPort.Intersects(bounds));
        }

        /// <summary>
        /// Transforms coordinates into screen coordinates.
        /// </summary>
        /// <param name="point">Location of the sprite in the world.</param>
        /// <returns>Screen position of a sprite.</returns>
        public static Vector2 Transform(Vector2 point)
        {
            return point - position;
        }

        /// <summary>
        /// Transforms coordinates into screen coordinates.
        /// </summary>
        /// <param name="rectangle">Location of a sprite in the world.</param>
        /// <returns>Screen position of a sprite.</returns>
        public static Rectangle Transform(Rectangle rectangle)
        {
            return new Rectangle(rectangle.Left - (int)position.X, rectangle.Top - (int)position.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Resets the camera back to its initial position.
        /// </summary>
        public static void Reset()
        {
            position = Vector2.Zero;
        }
        #endregion

    }
}
