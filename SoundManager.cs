using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Glider
{
    static class SoundManager
    {
        #region Declarations
        //Sound on/off switch
        private static bool soundOn;

        //Create the sound effects that will be used
        private static SoundEffect accelerate;
        private static SoundEffect decelerate;

        private static SoundEffect fuelUp;
        private static SoundEffect fuelDown;

        private static SoundEffect rocketThrust;
        #endregion

        #region Sound Properties
        /// <summary>
        /// Get the duration of the thrust sound.
        /// </summary>
        public static TimeSpan ThrustDuration
        {
            get { return rocketThrust.Duration; }
        }

        /// <summary>
        /// Turns the sound on or off.
        /// </summary>
        public static bool PlaySounds
        {
            get { return soundOn; }
            set { soundOn = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize all sounds.
        /// </summary>
        /// <param name="Content">The content manager that loads binary files.</param>
        public static void Initialize(ContentManager Content)
        {
            accelerate = Content.Load<SoundEffect>(@"Sounds\accelerate");
            decelerate = Content.Load<SoundEffect>(@"Sounds\decelerate");
            fuelUp = Content.Load<SoundEffect>(@"Sounds\fuelUp");
            fuelDown = Content.Load<SoundEffect>(@"Sounds\fuelDown");
            rocketThrust = Content.Load<SoundEffect>(@"Sounds\rocketThrust2");
        }

        /// <summary>
        /// Plays the speed up sound.
        /// </summary>
        public static void PlaySpeedUp()
        {
            try
            {
                accelerate.Play();  //play the enemy shot sound effect
            }
            catch
            {  }
        }

        /// <summary>
        /// Plays the speed down sound.
        /// </summary>
        public static void PlaySpeedDown()
        {
            try
            {
                decelerate.Play();  //play the enemy shot sound effect
            }
            catch
            { }
        }

        /// <summary>
        /// Plays the fuel up sound.
        /// </summary>
        public static void PlayFuelUp()
        {
            try
            {
                fuelUp.Play();  //play the enemy shot sound effect
            }
            catch
            { }
        }

        /// <summary>
        /// Plays the fuel down sound.
        /// </summary>
        public static void PlayFuelDown()
        {
            try
            {
                fuelDown.Play();  //play the enemy shot sound effect
            }
            catch
            { }
        }

        /// <summary>
        /// Plays the rocket thrust sound.
        /// </summary>
        public static void PlayRocketThrust()
        {
            try
            {
                rocketThrust.Play();  //play the enemy shot sound effect
            }
            catch
            { }
        }
        #endregion
    }
}
