using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glider
{
    static class ItemManager
    {
        #region Declarations
        public static List<Item> items = new List<Item>();  //stores the list of items that have been generated
        public static Texture2D itemsTexture;  //the spritesheet which hold the item sprites
        private static float spawnTimer = 3.0f;  //time till next item spawn
        private static float spawnRate = 2.0f;  //rate that an item will be generated

        private static string[] weightedArray = new string[10];  //array to generate items according to percentages

        private static Random randomPosition = new Random();  //random number to generate a Y position
        private static Random randomChoice;  //choose an item to spawn at random

        private static string currentSpeed = "slow";  //the current speed that items will move at
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the items.
        /// </summary>
        /// <param name="texture">Spritesheet that contains the item sprites.</param>
        public static void Initialize(Texture2D texture)
        {
            itemsTexture = texture;
            
            int[] weightedItem = new int[4] {3, 3, 1, 3};  //create the percentages for each item
            string[] itemString = new string[4] { "speedup", "speeddown", "fuel", "obstacle" };  //assign a string to each percentage

            //This loop inserts the strings over the array. Over time the values generated will be close to the percentages outlined above.
            int position = 0;
            for(int i = 0; i < itemString.Length; i++)
                for (int j = 0; j < weightedItem[i]; j++)
                {
                    weightedArray[position] = itemString[i];
                    position++;
                }
        }

        /// <summary>
        /// Resets the item manager to the initial values.
        /// </summary>
        public static void Reset()
        {
            items.Clear();
            currentSpeed = "slow";
            spawnTimer = 3.0f;
            spawnRate = 2.0f;
        }
        #endregion

        #region Update and Draw Methods
        /// <summary>
        /// Update the items on the screen.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        public static void Update(GameTime gameTime)
        {
            SpawnItems(gameTime);  //generate an item

            for (int i = items.Count - 1; i >= 0; i--)  //a List object is LIFO, so we have to start from the last position to get the oldest item first
            {
                items[i].Update(gameTime);  //update the item position
                if (items[i].Expired)  //if the item has expired remove it
                    items.RemoveAt(i);
                else  //if it hasn't expired
                {
                    if (items[i].IsBoxColliding(PlayerManager.playerSprite.BoundingBoxRect))  //check to see if its colliding with the player
                    {
                        if (items[i].Collidable)  //if it is colliding with the player and its still collidable
                        {
                            items[i].Collidable = false;  //disable it so we don't collide with it anymore
                            ItemDecision(items[i].SpriteName);  //determine which item we collided with and perform an action
                            if (items[i].SpriteName == "fuel")  //if we collided with the fuel item, remove it
                                items.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the items onto the screen.
        /// </summary>
        /// <param name="spriteBatch">Object used to draw textures on the screen.</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Item item in items)  //draw each item in the list of items
                item.Draw(spriteBatch);
        }
        #endregion

        #region Item Generating Methods
        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="itemPosition">The position to spawn the item.</param>
        /// <param name="initialFrame">First frame of the item animation.</param>
        /// <param name="spriteName">Name of the sprite being generated.</param>
        /// <param name="frameCount">Amount of frames in the animation.</param>
        /// <param name="currentSpeed">Speed the item needs to be moving at.</param>
        private static void AddItem(Vector2 itemPosition, Rectangle initialFrame, string spriteName, int frameCount, string currentSpeed)
        {
            Item newItem = new Item(itemPosition, itemsTexture, initialFrame, Vector2.Zero, spriteName, frameCount, currentSpeed);  //create a new Item sprite
            items.Add(newItem);  //add it to the list of items
        }

        /// <summary>
        /// Spawn a random item onto the game.
        /// </summary>
        /// <param name="gameTime">Timing values since the start of the program.</param>
        private static void SpawnItems(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            spawnTimer += elapsed;  //increase the time since last spawn
            
            if (spawnTimer > spawnRate)  //if we have waited the amount of time necessary to spawn more items
            {
                switch (GenerateItem())  //generate a random item according to its sprite name
                {
                    case "speedup":
                        AddItem(new Vector2(CameraManager.WorldRectangle.Right, GeneratePosition()), new Rectangle(0, 88, 150, 49), "speedup", 4, currentSpeed);
                        break;
                    case "speeddown":
                        AddItem(new Vector2(CameraManager.WorldRectangle.Right, GeneratePosition()), new Rectangle(0, 139, 150, 49), "speeddown", 4, currentSpeed);
                        break;
                    case "fuel":
                        AddItem(new Vector2(CameraManager.WorldRectangle.Right, GeneratePosition()), new Rectangle(0, 344, 50, 70), "fuel", 1, currentSpeed);
                        break;
                    case "obstacle":
                        AddItem(new Vector2(CameraManager.WorldRectangle.Right, GeneratePosition()), new Rectangle(0, 190, 103, 152), "obstacle", 4, currentSpeed);
                        break;
                }
                spawnTimer = 0;  //reset the time since last spawn
            }
        }

        /// <summary>
        /// Generate a random item.
        /// </summary>
        /// <returns>The name of the item that needs to be created.</returns>
        private static string GenerateItem()
        {
            randomChoice = new Random();
            return weightedArray[randomChoice.Next(0, 10)];  //generate a number between 0 and 9 and return the string found in that position
        }

        /// <summary>
        /// Generates a random position for an item.
        /// </summary>
        /// <returns></returns>
        private static int GeneratePosition()
        {
            //the position is relative to the viewable screen and won't be spawned below the grass
            return randomPosition.Next((int)MathHelper.Clamp(CameraManager.ViewPort.Top, 0, 747), (int)MathHelper.Clamp(CameraManager.ViewPort.Bottom, 480, 748));
        }
        #endregion

        #region Game Modifying Methods
        /// <summary>
        /// Modify the game according to the collided item.
        /// </summary>
        /// <param name="spriteName">Name of the item that was collided with.</param>
        private static void ItemDecision(string spriteName)
        {
            switch (spriteName)
            {
                case "speedup":
                    IncreaseSpeed();
                    if (SoundManager.PlaySounds)
                        SoundManager.PlaySpeedUp();
                    break;
                case "speeddown":
                    DecreaseSpeed();
                    if (SoundManager.PlaySounds)
                        SoundManager.PlaySpeedDown();
                    break;
                case "fuel":
                    AddFuel();
                    if (SoundManager.PlaySounds)
                        SoundManager.PlayFuelUp();
                    break;
                case "obstacle":
                    if (PlayerManager.Fuel <= 1.0f)  //if we have very little fuel left
                        PlayerManager.FreeFall = 150.0f;  //increase the free fall speed if we hit a balloon to end the game faster
                    RemoveFuel();
                    if (SoundManager.PlaySounds)
                        SoundManager.PlayFuelDown();
                    break;
            }
        }

        /// <summary>
        /// Increase the speed items and background travel across the screen.
        /// </summary>
        public static void IncreaseSpeed()
        {
            //we must change the speed of each item and of the backgrounds, the spawn rate too or else they will come out too slow or too fast
            foreach (Item item in items)
            {
                switch (item.Speed)
                {
                    case -8:
                        item.Speed = -10;
                        BackgroundManager.bgSprite1.Speed = -8;
                        BackgroundManager.bgSprite2.Speed = -12;
                        spawnRate = 1.5f;
                        currentSpeed = "normal";
                        break;
                    case -10:
                        item.Speed = -14;
                        BackgroundManager.bgSprite1.Speed = -12;
                        BackgroundManager.bgSprite2.Speed = -16;
                        spawnRate = 1.5f;
                        currentSpeed = "fast";
                        break;
                    case -12:
                        item.Speed = -18;
                        BackgroundManager.bgSprite1.Speed = -16;
                        BackgroundManager.bgSprite2.Speed = -20;
                        spawnRate = 1.0f;
                        currentSpeed = "fastest";
                        break;
                    case -14:
                        break;
                }
            }
        }

        /// <summary>
        /// Decrease the speed the items move across the screen.
        /// </summary>
        public static void DecreaseSpeed()
        {
            //we must change the speed of each item and of the backgrounds, the spawn rate too or else they will come out too slow or too fast
            foreach (Item item in items)
            {
                switch (item.Speed)
                {
                    case -8:
                        break;
                    case -10:
                        item.Speed = -8;
                        BackgroundManager.bgSprite1.Speed = -6;
                        BackgroundManager.bgSprite2.Speed = -10;
                        spawnRate = 2.0f;
                        currentSpeed = "slow";
                        break;
                    case -12:
                        item.Speed = -10;
                        BackgroundManager.bgSprite1.Speed = -8;
                        BackgroundManager.bgSprite2.Speed = -12;
                        spawnRate = 1.5f;
                        currentSpeed = "normal";
                        break;
                    case -14:
                        item.Speed = -12;
                        BackgroundManager.bgSprite1.Speed = -10;
                        BackgroundManager.bgSprite2.Speed = -14;
                        spawnRate = 1.0f; 
                        currentSpeed = "fast";
                        break;
                }
            }
        }

        /// <summary>
        /// Add fuel to the tank.
        /// </summary>
        public static void AddFuel()
        {
            if (PlayerManager.Fuel <= 90.0f)
                PlayerManager.Fuel += 10.0f;
            else
                PlayerManager.Fuel = 100.0f;
        }

        /// <summary>
        /// Remove fuel from the tank.
        /// </summary>
        public static void RemoveFuel()
        {
            if (PlayerManager.Fuel >= 10.0f)
                PlayerManager.Fuel -= 10.0f;
            else
                PlayerManager.Fuel = 0.0f;
        }
        #endregion
    }
}
