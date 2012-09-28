using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Advertising.Mobile.Xna;

namespace Glider
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Create the ad objects
        AdManager adManager;
        Ad bannerAd;
        float adDuration = 0;

        //Create the textures that store the graphics
        Texture2D spriteSheet;
        Texture2D background1;
        Texture2D background2;
        
        //Create the textures that are the game screens
        Texture2D gameScreen;
        Texture2D howToScreen;
        Texture2D highScores;
        Texture2D contactInfo;
        Texture2D myScoresScreen;
        Texture2D submitScore;
        Texture2D pausedScreen;
        Texture2D offonSprite;

        //Create fonts that will be used to draw the scores in various sizes
        SpriteFont highScoresFont;
        SpriteFont scoreFont;
        SpriteFont submitScoreFont;

        //Create menu options that are touchable on screen
        MenuItem startGameButton;
        MenuItem howToButton;
        MenuItem highScoresButton;
        MenuItem contactInfoButton;
        MenuItem myScoresButton;
        MenuItem soundButton;
        MenuItem yesScore;
        MenuItem noScore;
        MenuItem yesExit;
        MenuItem noExit;

        //Stores the location of where you tapped on screen
        Vector2 tapLocation = Vector2.Zero;

        //Values of the fuel tank dimensions on the left side of game screen
        const int MaxFuelTankHeight = 428;
        const int FuelTankWidth = 44;

        Vector2 fuelTankOverlayStart = new Vector2(740, 44);  //location of the fuel tank sprite in the spritesheet
        Vector2 fuelTankPosition = new Vector2(12, 46);  //location of the fuel tank sprite in the game

        //Various states possible in the game
        enum GameStates { TitleScreen, Playing, HowTo, HighScores, ContactInfo, GameOver, Paused, MyScores };

        //First state of the game is the TitleScreen
        GameStates gameState = GameStates.TitleScreen;
        
        //Boolean value to check to see if we have already called to receive scores online
        bool enteredOnce = false;

        //Vectors to center text on a position on screen
        Vector2 textSize = Vector2.Zero;
        Vector2 textCenter = new Vector2(405, 180);

        //Create the keyboard input objects
        IAsyncResult kbResult;
        string typedText;

        //Local Scores and Settings
        DataSaver<LocalScores> personalScoresSaver;
        LocalScores ScoresData;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Create the ad manager
            adManager = new AdManager(this, "074b2c6e-51bf-4c69-aa42-35b4c5aa21e6");
            adManager.TestMode = false;
            Components.Add(adManager);

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Max graphics width and height on the Windows Phone
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
            
            //Only landscape orientations are supported
            this.graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            
            //Only taps are registered on the touch screen
            TouchPanel.EnabledGestures = GestureType.Tap;

            graphics.IsFullScreen = true;

            //Create the object to save and load data from isolated storage
            personalScoresSaver = new DataSaver<LocalScores>(); 
            
            this.graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the personal scores and sound setting
            ScoresData = personalScoresSaver.LoadMyData("LocalScores");
            if (ScoresData == null)
                ScoresData = new LocalScores();
            SoundManager.PlaySounds = ScoresData.soundOn;

            //Create banner ad on the screen
            bannerAd = adManager.CreateAd("10016958", new Rectangle(160, 0, 480, 80), RotationMode.Manual, false);

            //Load all the graphics
            gameScreen = Content.Load<Texture2D>(@"MenuScreens\gamestartscreen");
            howToScreen = Content.Load<Texture2D>(@"MenuScreens\howtoscreen");
            highScores = Content.Load<Texture2D>(@"MenuScreens\highscoresscreen");
            contactInfo = Content.Load<Texture2D>(@"MenuScreens\contactscreen");
            myScoresScreen = Content.Load<Texture2D>(@"MenuScreens\myscoresscreen");
            spriteSheet = Content.Load<Texture2D>("spritesheet");
            background1 = Content.Load<Texture2D>("background01");
            background2 = Content.Load<Texture2D>("background02");
            pausedScreen = Content.Load<Texture2D>(@"MenuScreens\pausedscreen");
            submitScore = Content.Load<Texture2D>(@"MenuScreens\enterhighscore");
            offonSprite = Content.Load<Texture2D>(@"MenuScreens\offonsprite");
            
            //Load all the fonts
            highScoresFont = Content.Load<SpriteFont>(@"Fonts\highscores");
            submitScoreFont = Content.Load<SpriteFont>(@"Fonts\submitscores");
            scoreFont = Content.Load<SpriteFont>(@"Fonts\score");

            //Set the size of the WHOLE world and the viewable screen
            CameraManager.WorldRectangle = new Rectangle(0, 0, 1600, 1440);
            CameraManager.ViewPortWidth = 800;
            CameraManager.ViewPortHeight = 480;

            //Initialize all the managers
            BackgroundManager.Initialize(background1, background2, new Rectangle(0, 0, 1600, 1440), 1);
            PlayerManager.Initialize(spriteSheet, new Rectangle(0, 0, 161, 86), new Vector2(65, 200), 2);
            ItemManager.Initialize(spriteSheet);
            SoundManager.Initialize(Content);
            HighScoreTable.Initialize();

            //Set the locations of the buttons for touching
            startGameButton = new MenuItem(new Rectangle(40, 240, 250, 55));
            howToButton = new MenuItem(new Rectangle(40, 325, 275, 55));
            myScoresButton = new MenuItem(new Rectangle(40, 416, 235, 55));
            highScoresButton = new MenuItem(new Rectangle(488, 240, 270, 55));
            contactInfoButton = new MenuItem(new Rectangle(465, 325, 305, 55));
            soundButton = new MenuItem(new Rectangle(488, 416, 270, 55));

            yesScore = new MenuItem(new Rectangle(257, 282, 121, 51));
            noScore = new MenuItem(new Rectangle(439, 282, 121, 51));
            yesExit = new MenuItem(new Rectangle(257, 307, 121, 51));
            noExit = new MenuItem(new Rectangle(439, 307, 121, 51));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            switch (gameState)
            {
                case GameStates.TitleScreen:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();  //exit the game if the back button is pressed from the titlescreen
                    if (isTapped(startGameButton))
                    {
                        gameState = GameStates.Playing;  //switch to the playing state if the start game button is tapped
                        ResetGame();  //reset all game values to their starting values
                    }
                    else if (isTapped(howToButton))
                        gameState = GameStates.HowTo;  //switch to the how to screen if the how to button is tapped
                    else if (isTapped(highScoresButton))
                        gameState = GameStates.HighScores;  //switch to the high scores state if the high scores button is tapped
                    else if (isTapped(contactInfoButton))
                        gameState = GameStates.ContactInfo;  //switch to the contact info state if the contact info button is tapped
                    else if (isTapped(myScoresButton))
                        gameState = GameStates.MyScores;  //switch to the personal scores state if the my scores button is tapped
                    else if (isTapped(soundButton))
                    {
                        if (SoundManager.PlaySounds)
                            SoundManager.PlaySounds = false;
                        else
                            SoundManager.PlaySounds = true;
                        ScoresData.soundOn = SoundManager.PlaySounds;
                        personalScoresSaver.SaveMyData(ScoresData, "LocalScores");
                    }
                    tapLocation = Vector2.Zero;  //reset the tap location before switching states!
                    break;
                case GameStates.MyScores:
                case GameStates.ContactInfo:
                case GameStates.HowTo:
                case GameStates.HighScores:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        gameState = GameStates.TitleScreen;  //go back to the title screen state if back is pressed on the above 4 cases
                    break;
                case GameStates.Paused:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        gameState = GameStates.Playing;  //close the paused menu and go back into the game
                    else if (isTapped(noExit))
                        gameState = GameStates.Playing;  //go back to the game
                    else if (isTapped(yesExit))
                        gameState = GameStates.TitleScreen;  //go back to the title screen
                    tapLocation = Vector2.Zero;
                    bannerAd.Visible = true;
                    break;
                case GameStates.GameOver:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || isTapped(noScore))
                    {
                        gameState = GameStates.TitleScreen;  //go back to the title screen if we press back or select not to submit score when the game is over
                        bannerAd.Visible = true;
                        ScoresData.AddScore(PlayerManager.Score);
                        personalScoresSaver.SaveMyData(ScoresData, "LocalScores");
                    }
                    else if (isTapped(yesScore))
                    {
                        ScoresData.AddScore(PlayerManager.Score);
                        personalScoresSaver.SaveMyData(ScoresData, "LocalScores");
                        ShowKeyboard();  //show the keyboard if score is to be submitted
                    }
                    tapLocation = Vector2.Zero;  //reset the tap location before switching states!
                    break;
                case GameStates.Playing:
                    //Update the player, items, and background while the game is playing
                    bannerAd.Visible = false;  //turn off the ads while the game is playing
                    PlayerManager.Update(gameTime);
                    ItemManager.Update(gameTime);
                    BackgroundManager.Update(gameTime);
                    
                    if (PlayerManager.PlayerDestroyed)
                        gameState = GameStates.GameOver;  //if the player has fallen too far below the grass, switch to the game over state
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        gameState = GameStates.Paused;  //open the paused menu if the back button is pressed during the game
                    break;
            }
            
            //Update ad banner every 30 secs and only if it's visible
            adDuration += elapsed;
            if (adDuration > 30 && bannerAd.Visible == true)
            {
                bannerAd.RequestNextAd();
                adDuration = 0;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    spriteBatch.Draw(gameScreen, Vector2.Zero, Color.White);  //draw the title screen
                    if (SoundManager.PlaySounds)  //check if the sound is on and draw the appropriate switch on/off
                        spriteBatch.Draw(offonSprite, new Vector2(659, 413), new Rectangle(101, 0, 92, 50), Color.White);
                    else
                        spriteBatch.Draw(offonSprite, new Vector2(659, 413), new Rectangle(0, 0, 100, 50), Color.White);
                    break;
                case GameStates.ContactInfo:
                    spriteBatch.Draw(contactInfo, Vector2.Zero, Color.White);  //draw the contact info screen
                    break;
                case GameStates.HowTo:
                    spriteBatch.Draw(howToScreen, Vector2.Zero, Color.White);  //draw the how to screen
                    break;
                case GameStates.HighScores:
                    spriteBatch.Draw(highScores, Vector2.Zero, Color.White);  //draw the high score screen
                    getHighScores();  //call to get high scores online
                    break;
                case GameStates.MyScores:
                    spriteBatch.Draw(myScoresScreen, Vector2.Zero, Color.White);  //draw the my scores screen
                    getLocalScores();  //call to get the scores and draw them
                    break;
                case GameStates.Paused:  //game objects will still be draw in these states, but no longer updated
                case GameStates.GameOver:  
                case GameStates.Playing:
                    BackgroundManager.DrawFirst(spriteBatch);  //draw the background clouds
                    ItemManager.Draw(spriteBatch);  //draw the items
                    PlayerManager.Draw(spriteBatch);  //draw the player
                    BackgroundManager.DrawLast(spriteBatch);  //draw the foreground clouds
                    DrawFuelTanks();
                    DrawScore(); 
                    DrawThrustButtons();
                    if (gameState == GameStates.GameOver)  //if we are in the game over state
                    {
                        textSize = submitScoreFont.MeasureString(PlayerManager.Score.ToString());  //figure out the center of the score
                        spriteBatch.Draw(submitScore, new Vector2(215, 60), Color.White);  //draw the game over screen
                        spriteBatch.DrawString(submitScoreFont, PlayerManager.Score.ToString(), textCenter - (textSize / 2), Color.White);  //draw the score on the screen
                    }
                    if (gameState == GameStates.Paused)  //if we have paused the game, draw paused text
                    {
                        spriteBatch.Draw(pausedScreen, new Vector2(215, 90), Color.White);
                        /*spriteBatch.DrawString(submitScoreFont, "P A U S E D", new Vector2(282, 202), Color.Black);
                        spriteBatch.DrawString(submitScoreFont, "P A U S E D", new Vector2(280, 200), Color.Gainsboro);
                        spriteBatch.DrawString(highScoresFont, "Touch the screen to resume play", new Vector2(182, 277), Color.Black);
                        spriteBatch.DrawString(highScoresFont, "Touch the screen to resume play", new Vector2(180, 275), Color.Gainsboro);
                        spriteBatch.DrawString(highScoresFont, "Press the Back button to return to title screen", new Vector2(77, 307), Color.Black);
                        spriteBatch.DrawString(highScoresFont, "Press the Back button to return to title screen", new Vector2(75, 305), Color.Gainsboro);*/
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Drawing HUD Methods
        /// <summary>
        /// Draws the fuel tanks onto the screen.
        /// </summary>
        private void DrawFuelTanks()
        {
            int fuelHeight = (int)(MaxFuelTankHeight * (PlayerManager.Fuel / 100));  //gets the height for the amount of fuel the player has
            spriteBatch.Draw(spriteSheet, Vector2.Zero, new Rectangle(648, 0, 64, 484), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(spriteSheet, new Rectangle((int)fuelTankPosition.X,
                                                        (int)fuelTankPosition.Y + (MaxFuelTankHeight - fuelHeight),  //selects a portion of the sprite for drawing: max height - current height + the Y location of the sprite on the spritesheet
                                                        FuelTankWidth,
                                                        fuelHeight),
                                          new Rectangle((int)fuelTankOverlayStart.X,
                                                        (int)fuelTankOverlayStart.Y + (MaxFuelTankHeight - fuelHeight),  //Y location where the fuel tank needs to be drawn on screen, similar to above
                                                        FuelTankWidth,
                                                        fuelHeight), Color.GhostWhite);
        }

        /// <summary>
        /// Draw the score information onto the screen.
        /// </summary>
        private void DrawScore()
        {
            spriteBatch.Draw(spriteSheet, new Vector2(588, 5), new Rectangle(0, 564, 212, 44), Color.GhostWhite, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(scoreFont, "Score: ", new Vector2(603, 12), Color.GhostWhite);
            spriteBatch.DrawString(scoreFont, PlayerManager.Score.ToString(), new Vector2(680, 12), Color.GhostWhite, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
        }

        /// <summary>
        /// Draw the thrust buttons on screen.
        /// </summary>
        private void DrawThrustButtons()
        {
            spriteBatch.Draw(spriteSheet, new Vector2(615, 395), new Rectangle(0, 419, 68, 69), Color.GhostWhite);
            spriteBatch.Draw(spriteSheet, new Vector2(712, 395), new Rectangle(0, 491, 68, 69), Color.GhostWhite);
        }
        #endregion

        #region Touch Input Methods
        /// <summary>
        /// Checks to see if a menu item has been touched.
        /// </summary>
        /// <param name="item">The menu item that needs to be checked.</param>
        /// <returns>True if the menu item was indeed touched.</returns>
        private bool isTapped(MenuItem item)
        {
            tapPosition();
            if (item.HitBounds.Contains((int)tapLocation.X, (int)tapLocation.Y))
                return true;  //return true if the location of the tap fell within the bounds of the menu item
            else
                return false;
        }

        /// <summary>
        /// Stores the location of where the player touched the screen.
        /// </summary>
        private void tapPosition()
        {
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.Tap)
                {
                    tapLocation = new Vector2(gesture.Position.X, gesture.Position.Y);
                }
            }
        }
        #endregion

        #region High Score Methods
        /// <summary>
        /// Retrieve high scores online.
        /// </summary>
        private void getHighScores()
        {
            if (!enteredOnce && HighScoreTable.names[0].Equals(" "))  //if we haven't before called to retrieve scores and the high score table is empty
            {
                enteredOnce = true;  //we have just entered once, so we will never enter again
                HighScoreTable.RetrieveScores();  //call to retrieve the scores
            }
            else  //we have the scores, draw them on screen.
            {
                for (int i = 0; i < 10; i++)
                {
                    spriteBatch.DrawString(highScoresFont, HighScoreTable.ranks[i], new Vector2(160, i * 32 + 125), Color.White);
                    spriteBatch.DrawString(highScoresFont, HighScoreTable.names[i], new Vector2(200, i * 32 + 125), Color.White);
                    spriteBatch.DrawString(highScoresFont, HighScoreTable.scores[i].ToString(), new Vector2(515, i * 32 + 125), Color.White); 
                }
            }
        }

        /// <summary>
        /// Draw the local scores stored on the phone.
        /// </summary>
        private void getLocalScores()
        {
            for (int i = 0; i < ScoresData.scoreList.Count; i++)
            {
                spriteBatch.DrawString(highScoresFont, (i+1).ToString(), new Vector2(280, i * 32 + 125), Color.White);
                spriteBatch.DrawString(highScoresFont, ScoresData.scoreList[i].ToString(), new Vector2(405, i * 32 + 125), Color.White);
            }
        }

        /// <summary>
        /// Draw the keyboard onto the screen.
        /// </summary>
        private void ShowKeyboard()
        {
            kbResult = Guide.BeginShowKeyboardInput(PlayerIndex.One,
                                                    "Keyboard Touch Input", "Please type your nickname",
                                                    ((typedText == null) ? "" : typedText),
                                                    GetTypedChars, null);
        }

        /// <summary>
        /// Store the text that was typed in by the user.
        /// </summary>
        /// <param name="r">Result of the keyboard input.</param>
        protected void GetTypedChars(IAsyncResult r)
        {
            typedText = Guide.EndShowKeyboardInput(r);
            HighScoreTable.SendScore(typedText, PlayerManager.Score);  //call to send the score to the database
            gameState = GameStates.TitleScreen;  //leave the game, go to the title screen
            bannerAd.Visible = true;  //set the ad to visible since we are leaving the playing state
        }
        #endregion

        #region Game Reset Method
        /// <summary>
        /// Resets the game if we start the game again.
        /// </summary>
        private static void ResetGame()
        {
            CameraManager.Reset();
            ItemManager.Reset();
            BackgroundManager.Reset();
            PlayerManager.Reset();
        }
        #endregion
    }
}

