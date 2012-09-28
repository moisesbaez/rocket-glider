using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Glider
{
    class Background : Sprite
    {
        #region Declarations
        Sprite[] positions;  //an array of background sprites
        enum speed2 { slow = -10, normal = -12, fast = -14, fastest = -16};  //list of speeds
        int speed;  //current speed of the background
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new background sprite.
        /// </summary>
        /// <param name="position">Location of the background in the world.</param>
        /// <param name="texture">Spritesheet that stores the background texture.</param>
        /// <param name="initialFrame">First frame of the background animation.</param>
        /// <param name="velocity">Velocity of the background.</param>
        /// <param name="frameCount">Amount of frames in the animation.</param>
        public Background(Vector2 position, Texture2D texture, Rectangle initialFrame, Vector2 velocity, int frameCount)
            : base(position, texture, initialFrame, velocity, frameCount)
        {
            positions = new Sprite[2];  //initialize 2 background sprites
            Speed = -10;  //set the initial speed

            for (int i = 0; i < positions.Length; i++)  //set the location of each background sprite
                positions[i] = new Sprite(new Vector2(i * FrameWidth, 0), texture, initialFrame, velocity, frameCount);
        }
        #endregion

        #region Speed Property
        /// <summary>
        /// Gets or sets the speed of the background.
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Updates the backgrounds on the screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public override void Update(GameTime gameTime)
        {
            int currentSpeed = Speed;  //get the current speed of the background
            for (int i = 0; i < positions.Length; i++)  //for every background in the array
            {
                positions[i].WorldLocation +=  new Vector2(Speed, 0);  //move it across the screen

                if (Velocity.X <= 0)  //if we are moving to the left
                {
                    if (positions[i].WorldLocation.X == -FrameWidth)  //has the background just fallen off screen?
                    {
                        positions[i].WorldLocation = new Vector2(FrameWidth * (positions.Length - 1), 0);  //move it behind the other background
                    }
                    else if (positions[i].WorldLocation.X < -FrameWidth)  //has the background fallen way beyond the screen?
                    {
                        float offset = Math.Abs((positions[i].WorldLocation.X + FrameWidth));  //figure out by how many pixels it is to the left side of the world
                        positions[i].WorldLocation = new Vector2((FrameWidth * (positions.Length - 1)) - offset, 0);  //move it behind the other background accounting for the offset
                    }
                }
            }

        }

        /// <summary>
        /// Draws the background on the screen.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
                positions[i].Draw(spriteBatch);  //draw all backgrounds in the array
        }
        #endregion
    }
}
