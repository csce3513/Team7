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

            // See if the player has fired a shot
            FireShots(gameTime);

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

            spriteBatch.Begin();

            spriteBatch.Draw(crosshairTexture,
                new Vector2((Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2),
                    (Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)),
                    Color.White);

            spriteBatch.End();
        }

        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    // Add a shot to the model manager
                    modelManager.AddShot(
                        camera.cameraPosition + new Vector3(0, -5, 0),
                        camera.GetCameraDirection * shotSpeed);

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
    }
}
