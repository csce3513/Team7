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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace _3D_Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; protected set; }
        ModelManager modelManager;

        //Randomness
        public Random rnd { get; protected set; }

        //Shots
        float shotSpeed = 5;
        int shotDelay = 150;
        int shotCountdown = 0;

        //Crosshair
        Texture2D crosshairTexture;

        //Audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 1024;
#if !DEBUG
            graphics.IsFullScreen = true;
#endif

            rnd = new Random();
        }

        protected override void Initialize()
        {
            //Initialize camera
            camera = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            //Initialize model manager
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load crosshair
            crosshairTexture = Content.Load<Texture2D>(@"textures\crosshair");

            // Load sounds and play initial sounds
            audioEngine = new AudioEngine(
                @"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine,
                @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine,
                @"Content\Audio\Sound Bank.xsb");

            trackCue = soundBank.GetCue("Tracks");
            trackCue.Play();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();

            // See if the player has fired a shot
            FireShots(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            //Draw crosshair
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

                    //Add shot in spread to the right
                    Vector3 initialPosition = camera.cameraPosition +
                        Vector3.Cross(camera.GetCameraDirection,
                        camera.cameraUp) * 5;

                    modelManager.AddShot(
                        initialPosition + new Vector3(0, -5, 0),
                        camera.GetCameraDirection * shotSpeed);

                    //Add shot in spread to the left
                    initialPosition = camera.cameraPosition -
                        Vector3.Cross(camera.GetCameraDirection,
                        camera.cameraUp) * 5;

                    modelManager.AddShot(
                        initialPosition + new Vector3(0, -5, 0),
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