using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glider
{
    class Sprite
    {
        #region Declarations
        public Texture2D texture;  //spritesheet texture that holds the sprite and its animation
        private List<Rectangle> frames = new List<Rectangle>();  //store the frames for the sprite animation

        private Vector2 worldLocation = Vector2.Zero;  //location of the sprite in the world
        private Vector2 velocity = Vector2.Zero;  //velocity the sprite travels at

        private int currentFrame;  //current frame of the animation
        private float frameTime = 0.1f;  //time per frame
        private float timeForCurrentFrame = 0.0f;  //time of the current animated frame

        private float rotation = 0.0f;  //rotation of the sprite

        private Color tintColor = Color.White;

        public bool Expired = false;  //value to check if the sprite is no longer active
        public bool Animate = true;  //value to check if the sprite needs to be animated

        public bool Collidable = true;  //value to check if the sprite is able to collide with other objects
        public int CollisionRadius = 0;  //radius of the sprite for collisions
        public int BoundingXPadding = 8;  //shrinks the sprite X and Y values for collisions
        public int BoundingYPadding = 8;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new sprite object.
        /// </summary>
        /// <param name="texture">Spritesheet used to draw the sprite</param>
        /// <param name="position">Position where the sprite will be drawn</param>
        /// <param name="initialFrame">The first frame of the sprites animation</param>
        /// <param name="velocity">The velocity the sprite travels at</param>
        public Sprite(Vector2 position, Texture2D texture, Rectangle initialFrame, Vector2 velocity, int frameCount)
        {
            this.texture = texture;
            worldLocation = position;
            this.velocity = velocity;

            frames.Add(initialFrame);  //add the first frame to the animation list

            for (int i = 1; i < frameCount; i++)  //for every animated frame add it to the animation list
                AddFrame(new Rectangle(initialFrame.X + (FrameWidth * i),
                                       initialFrame.Y,
                                       FrameWidth,
                                       FrameHeight));
        }
        #endregion

        #region Drawing and Animation Properties
        /// <summary>
        /// Gets the width of the sprite.
        /// </summary>
        public int FrameWidth
        {
            get { return frames[0].Width; }
        }

        /// <summary>
        /// Gets the height of the sprite.
        /// </summary>
        public int FrameHeight
        {
            get { return frames[0].Height; }
        }

        /// <summary>
        /// Gets or sets the color of the sprite.
        /// </summary>
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the sprite.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        /// <summary>
        /// Gets or sets the current frame of the animation.
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }  //makes sure the value is between 0 and the amount of frames of the animation
        }

        /// <summary>
        /// Gets or sets the speed in which the animation is updated.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }  //makes sure the value is > 0
        }

        /// <summary>
        /// Gets the rectangle associated with the current frame.
        /// </summary>
        public Rectangle SourceFrame
        {
            get { return frames[currentFrame]; }
        }
        #endregion

        #region Positional Properties
        /// <summary>
        /// Gets or sets the position of the sprite.
        /// </summary>
        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }

        /// <summary>
        /// Gets the position of the sprite in screen coordinates.
        /// </summary>
        public Vector2 ScreenLocation
        {
            get { return CameraManager.Transform(WorldLocation); }  //transform world coordinates to screen coordinates
        }

        /// <summary>
        /// Gets or sets the velocity the sprite travels at.
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Gets the location the sprite needs to be drawn in the world.
        /// </summary>
        public Rectangle WorldRectangle
        {
            get { return new Rectangle((int)WorldLocation.X, (int)WorldLocation.Y, FrameWidth, FrameHeight); }
        }

        /// <summary>
        /// Gets the location the sprite needs to be drawn on the screen.
        /// </summary>
        public Rectangle ScreenRectangle
        {
            get { return CameraManager.Transform(WorldRectangle); }
        }

        /// <summary>
        /// Gets the center point of a sprite.
        /// </summary>
        public Vector2 RelativeCenter
        {
            get { return new Vector2(FrameWidth / 2, FrameHeight / 2); }
        }

        /// <summary>
        /// Gets the center point of the sprite in the world.
        /// </summary>
        public Vector2 WorldCenter
        {
            get { return WorldLocation + RelativeCenter; }
        }

        /// <summary>
        /// Gets the center point of the sprite in the screen.
        /// </summary>
        public Vector2 ScreenCenter
        {
            get { return CameraManager.Transform(WorldCenter); }
        }
        #endregion

        #region Animation-Related Methods
        /// <summary>
        /// Adds a frame of the animation to the animation list.
        /// </summary>
        /// <param name="newFrame">Location of the next sprite in the spritesheet.</param>
        public void AddFrame(Rectangle newFrame)
        {
            frames.Add(newFrame);
        }

        /// <summary>
        /// Rotates the sprite a certain direction.
        /// </summary>
        /// <param name="direction">Location the sprite must turn to.</param>
        public void RotateTo(Vector2 direction)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
        #endregion

        #region Collision Related Properties
        /// <summary>
        /// Accounts for the padding around a sprite at its location.
        /// </summary>
        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle((int)WorldLocation.X + BoundingXPadding,
                                     (int)WorldLocation.Y + BoundingYPadding,
                                     FrameWidth - (BoundingXPadding * 2),
                                     FrameHeight - (BoundingYPadding * 2));
            }  //Multiply by 2 to get both sides of the bounding box
        }
        #endregion

        #region Collision Detection Methods
        /// <summary>
        /// Checks to see if bounding boxes have collided.
        /// </summary>
        /// <param name="OtherBox">Second bounding box to check.</param>
        /// <returns>True if both boxes intersect.</returns>
        public bool IsBoxColliding(Rectangle OtherBox)
        {
            if ((Collidable) && (!Expired))  //is it a collidable object that still is active?
                return BoundingBoxRect.Intersects(OtherBox);  //check if they intersect
            else
                return false;
        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Updates the sprite on the screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!Expired)  //only update those sprites that are still active
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeForCurrentFrame += elapsed;  //increase the frame animation counter

                if (Animate)  //if the sprite needs to be animated
                {
                    if (timeForCurrentFrame >= frameTime)  //has the current frame exceeded its time on screen?
                    {
                        CurrentFrame = (CurrentFrame + 1) % (frames.Count);  //switch to the next frame of the animation
                        timeForCurrentFrame = 0.0f;  //reset the current frame animation counter
                    }
                }
                WorldLocation += (Velocity * elapsed);  //update the position of the sprite on screen
            }
        }

        /// <summary>
        /// Draws a sprite to the screen.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (CameraManager.ObjectIsVisible(WorldRectangle))  //Only draw the items when they have entered the world
                spriteBatch.Draw(texture, ScreenCenter, SourceFrame, tintColor, Rotation, RelativeCenter, 1.0f, SpriteEffects.None, 1.0f);
        }
        #endregion
    }
}
