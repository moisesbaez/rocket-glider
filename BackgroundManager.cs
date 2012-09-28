using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glider
{
    static class BackgroundManager
    {
        #region Declarations
        public static Background bgSprite1;  //create 2 backgrounds, one for the foreground and one for the back
        public static Background bgSprite2;
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the backgrounds.
        /// </summary>
        /// <param name="texture1">The background texture.</param>
        /// <param name="texture2">The foreground texture.</param>
        /// <param name="initialFrame">The first frame of the animation.</param>
        /// <param name="frameCount">The amount of frames in the animation.</param>
        public static void Initialize(Texture2D texture1, Texture2D texture2, Rectangle initialFrame, int frameCount)
        {
            bgSprite1 = new Background(Vector2.Zero, texture1, initialFrame, Vector2.Zero, 1);
            bgSprite2 = new Background(Vector2.Zero, texture2, initialFrame, Vector2.Zero, 1);

            bgSprite1.Speed = -6;  //the background will travel at a slower rate of speed than the foreground
        }

        /// <summary>
        /// Resets the background speeds back to their initial values.
        /// </summary>
        public static void Reset()
        {
            bgSprite1.Speed = -6;
            bgSprite2.Speed = -10;
        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Update the positions of the backgrounds.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public static void Update(GameTime gameTime)
        {
            bgSprite1.Update(gameTime);
            bgSprite2.Update(gameTime);
        }

        /// <summary>
        /// Draw the background.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public static void DrawFirst(SpriteBatch spriteBatch)
        {
            bgSprite1.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw the foreground.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public static void DrawLast(SpriteBatch spriteBatch)
        {
            bgSprite2.Draw(spriteBatch);
        }
        #endregion
    }
}
