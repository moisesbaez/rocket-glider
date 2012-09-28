using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Glider
{
    class MenuItem
    {
        #region Declarations
        private Rectangle frame;  //location of the menu item on the screen
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        /// <param name="frame">Location of the menu item on the screen.</param>
        public MenuItem(Rectangle frame)
        {
            this.frame = frame;
        }
        #endregion

        #region Menu Item Properties
        /// <summary>
        /// Gets the center of the menu item.
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(frame.X + (Width / 2), frame.Y + (Height / 2)); }
        }

        /// <summary>
        /// Gets the width of the menu item.
        /// </summary>
        public float Width
        {
            get { return frame.Width; }
        }

        /// <summary>
        /// Gets the height of the menu item.
        /// </summary>
        public float Height
        {
            get { return frame.Height; }
        }

        /// <summary>
        /// Gets the bounds of the menu item for touching purposes.
        /// </summary>
        public Rectangle HitBounds
        {
            get
            {
                Rectangle bounds = new Rectangle((int)(Center.X - Width / 2),
                                                 (int)(Center.Y - Height / 2),
                                                 (int)Width,
                                                 (int)Height);
                return bounds;
            }
        }
        #endregion
    }
}
