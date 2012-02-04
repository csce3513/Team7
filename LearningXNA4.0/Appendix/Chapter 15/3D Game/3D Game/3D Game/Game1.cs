using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3D_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Model stuff
        ModelManager modelManager;

        // Camera
        public Camera camera { get; protected set; }

        // Random
        public Random rnd { get; protected set; }

        // Shot variables
        float shotSpeed = 10;
        int shotDelay = 300;
        int shotCountdown = 0;

        // Crosshair
        Texture2D crosshairTexture;

        // Audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // Game state items
        public enum GameState { START, PLAY, LEVEL_CHANGE, END }
        GameState currentGameState = GameState.START;

        // Splash screen and scoring
        SplashScreen splashScreen;
        int score = 0;
        SpriteFont scoreFont;

        // Powerup stuff
        int originalShotDelay = 300;
        public enum PowerUps { NONE, RAPID_FIRE, MULTI_SHOT }
        int shotDelayRapidFire = 100;
        int rapidFireTime = 10000;
        int powerUpCountdown = 0;
        string powerUpText = "";
        int powerUpTextTimer = 0;
        SpriteFont powerUpFont;
        PowerUps currentPowerUp = PowerUps.NONE;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            rnd = new Random();

            // Set preferred resolution
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 1024;

// If not running debug, run in full screen
#if !DEBUG
    graphics.IsFullScreen = true;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize Camera
            camera = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            // Initialize model manager
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            modelManager.Enabled = false;
            modelManager.Visible = false;

            // Splash screen component
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.SetData("Welcome to Space Defender!",
                currentGameState);

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

            // Load the crosshair texture
            crosshairTexture = Content.Load<Texture2D>(@"textures\crosshair");

            // Load sounds and play initial sounds
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            
            // Play the soundtrack
            trackCue = soundBank.GetCue("Tracks");
            trackCue.Play();

            // Load fonts
            scoreFont = Content.Load<SpriteFont>(@"Fonts\ScoreFont");
            powerUpFont = Content.Load<SpriteFont>(@"fonts\PowerupFont");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Only check for shots if you're in the play game state
            if (currentGameState == GameState.PLAY)
            {
                // See if the player has fired a shot
                FireShots(gameTime);
            }

            // Update power-up timer
            UpdatePowerUp(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            base.Draw(gameTime);

            // Only draw crosshair if in play game state
            if (currentGameState == GameState.PLAY)
            {

                // Draw the crosshair
                spriteBatch.Begin();

                spriteBatch.Draw(crosshairTexture,
                    new Vector2((Window.ClientBounds.Width / 2)
                        - (crosshairTexture.Width / 2),
                        (Window.ClientBounds.Height / 2)
                        - (crosshairTexture.Height / 2)),
                        Color.White);

                // Draw the current score
                string scoreText = "Score: " + score;
                spriteBatch.DrawString(scoreFont, scoreText,
                    new Vector2(10, 10), Color.Red);

                // Let the player know how many misses he has left
                spriteBatch.DrawString(scoreFont, "Misses Left: " +
                    modelManager.missesLeft,
                    new Vector2(10, scoreFont.MeasureString(scoreText).Y + 20),
                    Color.Red);

                // If power-up text timer is live, draw power-up text
                if (powerUpTextTimer > 0)
                {
                    powerUpTextTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    Vector2 textSize = powerUpFont.MeasureString(powerUpText);
                    spriteBatch.DrawString(powerUpFont,
                        powerUpText,
                        new Vector2((Window.ClientBounds.Width / 2) -
                        (textSize.X / 2),
                        (Window.ClientBounds.Height / 2) -
                        (textSize.Y / 2)),
                        Color.Goldenrod);
                }

                spriteBatch.End();
            }
        }

        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed)
                {

                    if (currentPowerUp != PowerUps.MULTI_SHOT)
                    {
                        //Normal mode - fire one shot

                        // Add a shot to the model manager
                        modelManager.AddShot(
                            camera.cameraPosition + new Vector3(0, -5, 0),
                            camera.GetCameraDirection * shotSpeed);
                    }
                    else
                    {
                        //Multi-shot mode!

                        //Add shot in spread to the top right
                        Vector3 initialPosition = camera.cameraPosition +
                            Vector3.Cross(camera.GetCameraDirection, camera.cameraUp) * 5
                            + (camera.cameraUp * 5);
                        modelManager.AddShot(
                            initialPosition + new Vector3(0, -5, 0),
                            camera.GetCameraDirection * shotSpeed);

                        //Add shot in spread to the bottom right
                        initialPosition = camera.cameraPosition +
                            Vector3.Cross(camera.GetCameraDirection, camera.cameraUp) * 5
                            - (camera.cameraUp * 5);
                        modelManager.AddShot(
                            initialPosition + new Vector3(0, -5, 0),
                            camera.GetCameraDirection * shotSpeed);

                        //Add shot in spread top left
                        initialPosition = camera.cameraPosition -
                            Vector3.Cross(camera.GetCameraDirection, camera.cameraUp) * 5
                            + (camera.cameraUp * 5);
                        modelManager.AddShot(
                            initialPosition + new Vector3(0, -5, 0),
                            camera.GetCameraDirection * shotSpeed);

                        //Add shot in spread bottom left
                        initialPosition = camera.cameraPosition -
                            Vector3.Cross(camera.GetCameraDirection, camera.cameraUp) * 5
                            - (camera.cameraUp * 5);
                        modelManager.AddShot(
                            initialPosition + new Vector3(0, -5, 0),
                            camera.GetCameraDirection * shotSpeed);
                    }

                    // Play shot audio
                    PlayCue("Shot");

                    // Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public void PlayCue(string cue)
        {
            soundBank.PlayCue(cue);
        }

        public void ChangeGameState(GameState state, int level)
        {
            CancelPowerUps();

            currentGameState = state;

            switch (currentGameState)
            {
                case GameState.LEVEL_CHANGE:
                    splashScreen.SetData("Level " + (level + 1),
                        GameState.LEVEL_CHANGE);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    // Stop the soundtrack loop
                    trackCue.Stop(AudioStopOptions.Immediate);
                    break;

                case GameState.PLAY:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;

                    if (trackCue.IsPlaying)
                        trackCue.Stop(AudioStopOptions.Immediate);

                    // To play a stopped cue, get the cue from the soundbank again
                    trackCue = soundBank.GetCue("Tracks");
                    trackCue.Play();
                    break;

                case GameState.END:
                    splashScreen.SetData("Game Over.\nLevel: " + (level + 1) +
                        "\nScore: " + score, GameState.END);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    // Stop the soundtrack loop
                    trackCue.Stop(AudioStopOptions.Immediate);
                    break;
            }
        }

        public void AddPoints(int points)
        {
            score += points;
        }

        private void CancelPowerUps()
        {
            modelManager.consecutiveKills = 0;
            shotDelay = originalShotDelay;
            currentPowerUp = PowerUps.NONE;

        }

        protected void UpdatePowerUp(GameTime gameTime)
        {
            if (powerUpCountdown > 0)
            {
                powerUpCountdown -= gameTime.ElapsedGameTime.Milliseconds;
                if (powerUpCountdown <= 0)
                {
                    CancelPowerUps();
                    powerUpCountdown = 0;
                }
            }
        }

        public void StartPowerUp()
        {
            if (rnd.Next(2) == 0)
                currentPowerUp = PowerUps.RAPID_FIRE;
            else
                currentPowerUp = PowerUps.MULTI_SHOT;

            switch (currentPowerUp)
            {
                case PowerUps.RAPID_FIRE:
                    shotDelay = shotDelayRapidFire;
                    powerUpCountdown = rapidFireTime;
                    powerUpText = "Rapid Fire Mode!";
                    powerUpTextTimer = 1000;
                    soundBank.PlayCue("RapidFire");
                    break;
                case PowerUps.MULTI_SHOT:
                    powerUpCountdown = rapidFireTime;
                    powerUpText = "Multi-shot Mode!";
                    powerUpTextTimer = 1000;
                    soundBank.PlayCue("RapidFire");
                    break;
            }
        }
    }
}
