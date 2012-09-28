using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glider
{
    class Item : Sprite
    {
        #region Declarations
        string spriteName = "";  //name of the item
        enum speed2 { slow = -8, normal = -10, fast = -12, fastest = -14 };  //list of speeds
        int speed;  //current speed of the item
        #endregion

        #region Item Properties
        /// <summary>
        /// Get or set the speed of the item
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>
        /// Get the name of the item
        /// </summary>
        public string SpriteName
        {
            get { return spriteName; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new item sprite.
        /// </summary>
        /// <param name="position">Location of the item in the world.</param>
        /// <param name="texture">Spritesheet that holds the item sprite.</param>
        /// <param name="initialFrame">Location of the first frame of the animation.</param>
        /// <param name="velocity">Velocity of the item.</param>
        /// <param name="spriteName">Name of the item.</param>
        /// <param name="frameCount">Amount of frames in the animation.</param>
        /// <param name="currentSpeed">Speed the item travels.</param>
        public Item(Vector2 position, Texture2D texture, Rectangle initialFrame, Vector2 velocity, string spriteName, int frameCount, string currentSpeed)
            : base(position, texture, initialFrame, velocity, frameCount)
        {
            this.spriteName = spriteName;

            //Set the speed according to the speed parameter passed when making the item
            switch (currentSpeed)
            {
                case "slow":
                    Speed = (int)speed2.slow;
                    break;
                case "normal":
                    Speed = (int)speed2.normal;
                    break;
                case "fast":
                    Speed = (int)speed2.fast;
                    break;
                case "fastest":
                    Speed = (int)speed2.fastest;
                    break;
            }

        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Update the item on screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public override void Update(GameTime gameTime)
        {
            if (speed <= 0)  //if we are travelling to the left
                WorldLocation += new Vector2(Speed, 0);  //update its position in the world

            if (WorldLocation.X < -base.FrameWidth)  //once the item has fallen of the world rectangle
                Expired = true;  //it is no longer active

            base.Update(gameTime);  //update the base Sprite information
        }

        /// <summary>
        /// Draws the item onto the game.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Expired)  //draw items as long as they haven't expired yet
                base.Draw(spriteBatch);  //call the base sprite drawing method
        }
        #endregion
    }
}
