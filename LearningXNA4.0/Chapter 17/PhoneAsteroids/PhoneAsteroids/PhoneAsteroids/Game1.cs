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

namespace PhoneAsteroids
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

        // Random number generator
        public Random random { get; protected set; }

        // Textures
        Texture2D attackTexture;

        // Audio
        SoundEffect shotSound;

        public Game1()
        {
            // Initialize random number generator
            random = new Random();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Force the game to stay at landscape left
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

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
            camera = new Camera(this);
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

            // Load textures
            attackTexture = Content.Load<Texture2D>(@"textures\attack");

            // Load audio
            shotSound = Content.Load<SoundEffect>(@"audio\shot");
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

            // Did the user press the attack button on the touch panel?
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed &&
                    GetAttackTextureRect().Contains(
                    new Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y)))
                {
                    shotSound.Play();
                    modelManager.FireShot();
                }
            }

            base.Update(gameTime);
        }

        protected Rectangle GetAttackTextureRect()
        {
            return new Rectangle(
                graphics.PreferredBackBufferWidth - attackTexture.Width - 10,
                graphics.PreferredBackBufferHeight - attackTexture.Height - 10,
                attackTexture.Width,
                attackTexture.Height);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw the attack texture on the screen
            spriteBatch.Draw(attackTexture,
                GetAttackTextureRect(),
                Color.White);

            spriteBatch.End();

            ResetGraphicsDevice();


            base.Draw(gameTime);
        }

        private void ResetGraphicsDevice()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
