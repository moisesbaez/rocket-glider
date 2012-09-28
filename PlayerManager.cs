using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Glider
{
    static class PlayerManager
    {
        #region Declarations
        public static Sprite playerSprite;
        private static Vector2 moveAngle = new Vector2(0, 1);  //direction the player points
        private static Vector2 initialLocation;  //initial location when the player first begins to play
        
        private static float playerSpeed = 150.0f;  //speed of the players thrust
        private static float fuel = 50.0f;  //stores the amount of the fuel

        private static int score = 0;  //stores the players score

        public static float FreeFall = 25.0f;  //the rate the player descends

        public static Rectangle ThrustUpRect = new Rectangle(607, 390, 85, 90);  //location of the thrust rectangles
        public static Rectangle ThrustDownRect = new Rectangle(704, 390, 85, 90);

        private static float thrustDuration;  //duration of the thrust sound
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the player score.
        /// </summary>
        public static int Score
        {
            get { return score; }
            set { score = value; }
        }

        /// <summary>
        /// Get or sets the amount of fuel
        /// </summary>
        public static float Fuel
        {
            get { return fuel; }
            set { fuel = value; }
        }

        /// <summary>
        /// Determines whether the player has been destroyed.
        /// </summary>
        public static bool PlayerDestroyed
        {
            get { return (playerSprite.WorldLocation.Y > 940 || CameraManager.ViewPort.Bottom >= 1400); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the player.
        /// </summary>
        /// <param name="texture">Spritesheet texture that contains the player sprite.</param>
        /// <param name="initialFrame">First frame of the player animation.</param>
        /// <param name="worldLocation">Initial location of the player in the world.</param>
        /// <param name="frameCount">Amount of frames in the player animation.</param>
        public static void Initialize(Texture2D texture, Rectangle initialFrame, Vector2 worldLocation, int frameCount)
        {
            initialLocation = worldLocation;  //store the initial location of the player
            playerSprite = new Sprite(worldLocation, texture, initialFrame, Vector2.Zero, frameCount);  //create the player sprite

            playerSprite.BoundingXPadding = 4;  //shrink the sprite by 4 pixels on each side
            playerSprite.BoundingYPadding = 4;
        }

        /// <summary>
        /// Resets the player variables back to starting values.
        /// </summary>
        public static void Reset()
        {
            playerSprite.WorldLocation = initialLocation;
            Fuel = 50.0f;
            Score = 0;
            FreeFall = 25.0f;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Clamps the player within the viewable screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        private static void clampToWorld(GameTime gameTime)
        {
            float currentX = playerSprite.WorldLocation.X;  //store the current location of the player
            float currentY = playerSprite.WorldLocation.Y;

            currentX = MathHelper.Clamp(currentX, CameraManager.ViewPort.Left, CameraManager.ViewPort.Right - playerSprite.FrameWidth);  //clamp the X and Y values between the top and bottom of the screen
            currentY = MathHelper.Clamp(currentY, CameraManager.ViewPort.Top, CameraManager.ViewPort.Bottom - playerSprite.FrameHeight);

            playerSprite.WorldLocation = new Vector2(currentX, currentY);  //update the position with the clamped values
        }

        /// <summary>
        /// Moves the viewable screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        /// <param name="moveAngle">Direction that the screen moves.</param>
        /// <param name="moveSpeed">Speed that the screen moves.</param>
        private static void repositionCamera(GameTime gameTime, Vector2 moveAngle, float moveSpeed)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveScale = moveSpeed * elapsed;
            CameraManager.MoveCamera(new Vector2(0, moveAngle.Y) * moveScale);
        }

        /// <summary>
        /// Moves the player slowly to the bottom of the world.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        private static void freeFall(GameTime gameTime)
        {
            playerSprite.RotateTo(new Vector2(0, 0));
            playerSprite.Velocity = moveAngle * FreeFall;
            repositionCamera(gameTime, moveAngle, FreeFall);  //move the camera to follow the player
        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Update the player.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public static void Update(GameTime gameTime)
        {
            handleInput(gameTime);  //handle any input from the player
            clampToWorld(gameTime);  //make sure the player stays within the world
            playerSprite.Update(gameTime);  //update the player sprite
            score += Math.Abs(BackgroundManager.bgSprite1.Speed);  //increase the score according to the speed of the background
        }

        /// <summary>
        /// Draw the player.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }
        #endregion

        #region Input Handling Methods

        /// <summary>
        /// Handles the input from the touch screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public static void handleInput(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            TouchCollection touchCollection = TouchPanel.GetState();  //initialize a collection of touches
            moveAngle.Normalize();

            //check if there is touch information and the touchscreen is either being pressed or moved
            if (touchCollection.Count > 0 && (touchCollection[0].State == TouchLocationState.Pressed || touchCollection[0].State == TouchLocationState.Moved))
            {
                Rectangle touchPosition = new Rectangle((int)touchCollection[0].Position.X, (int)touchCollection[0].Position.Y, 1, 1);  //store the rectangle where the screen was touched
                if (touchPosition.Intersects(ThrustUpRect))  //if the touch position falls within the thrust up control
                {
                    if (Fuel > 0.0f)  //check if there is fuel to thrust up
                    {
                        playerSprite.RotateTo(new Vector2(1, -1));  //rotate the player to 45 degrees
                        playerSprite.Velocity = -moveAngle * playerSpeed;  //update its movement velocity to go up
                        repositionCamera(gameTime, -moveAngle, playerSpeed);  //adjust the camera to follow the player
                        Fuel -= 0.1f;  //decrease the amount of fuel
                        if (SoundManager.PlaySounds)
                        {
                            if (thrustDuration >= SoundManager.ThrustDuration.TotalMilliseconds)  //if the thrust sound has completed playing
                            {
                                SoundManager.PlayRocketThrust();  //play the sound again
                                thrustDuration = 0;  //reset the sound duration counter
                            }
                        }
                    }
                    else
                        freeFall(gameTime);  //if there is no more fuel then don't thrust, just continue falling
                }
                else if (touchPosition.Intersects(ThrustDownRect))  //check if there is fuel to thrust down
                {
                    if (Fuel > 0.0f)  //check if there is fuel to thrust down
                    {
                        playerSprite.RotateTo(new Vector2(1, 1));  //rotate the player to -45 degrees
                        playerSprite.Velocity = moveAngle * playerSpeed;  //update its movemnt velocity to go down
                        repositionCamera(gameTime, moveAngle, playerSpeed);  //adjust the camera to follow the player
                        Fuel -= 0.1f;  //decrease the amount of fuel
                        if (SoundManager.PlaySounds)
                        {
                            if (thrustDuration >= SoundManager.ThrustDuration.TotalMilliseconds)  //if the thrust sound has completed playing
                            {
                                SoundManager.PlayRocketThrust();  //play the sound again
                                thrustDuration = 0;  //reset the sound duration counter
                            }
                        }
                    }
                    else
                        freeFall(gameTime);  //if there is no more fuel then don't thrust, just continue falling
                }
                else  //if we didn't touch the thrusts
                {
                    freeFall(gameTime);  //continue free falling

                    //if we touched anywhere above the player move the player up by 4 pixels
                    if (PlayerManager.playerSprite.ScreenLocation.Y > touchCollection[0].Position.Y && Math.Abs(PlayerManager.playerSprite.ScreenLocation.Y - touchCollection[0].Position.Y) >= 4)
                    {
                        playerSprite.WorldLocation += new Vector2(0, -4);
                    }
                    //if we touched anywhere below the player move hte player down by 4 pixels
                    else if (PlayerManager.playerSprite.ScreenLocation.Y < touchCollection[0].Position.Y && Math.Abs(PlayerManager.playerSprite.ScreenLocation.Y - touchCollection[0].Position.Y) >= 4)
                    {
                        playerSprite.WorldLocation += new Vector2(0, 4);
                    }
                }
            }
            else  //if we are not touching the screen just continue falling
            {
                freeFall(gameTime);
            }
            thrustDuration += elapsed;  //increase the sound duration counter
        }
        #endregion

    }
}
