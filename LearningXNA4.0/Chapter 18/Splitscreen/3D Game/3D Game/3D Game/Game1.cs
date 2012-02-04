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

        // Camera stuff
        public Camera camera1 { get; protected set; }
        public Camera camera2 { get; protected set; }
        public Camera currentDrawingCamera { get; protected set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create viewports
            Viewport vp1 = GraphicsDevice.Viewport;
            Viewport vp2 = GraphicsDevice.Viewport;
            vp1.Height = (GraphicsDevice.Viewport.Height / 2);

            vp2.Y = vp1.Height;
            vp2.Height = vp1.Height;


            // Add camera components
            camera1 = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up, vp1);
            Components.Add(camera1);

            camera2 = new Camera(this, new Vector3(0, 0, -50),
                Vector3.Zero, Vector3.Up, vp2);
            Components.Add(camera2);

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

            // TODO: use this.Content to load your game content here
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

            // Move the cameras
            KeyboardState keyboardState = Keyboard.GetState();

            // Move camera1 with WASD keys
            if (keyboardState.IsKeyDown(Keys.W))
                camera1.MoveForwardBackward(true);
            if (keyboardState.IsKeyDown(Keys.S))
                camera1.MoveForwardBackward(false);
            if (keyboardState.IsKeyDown(Keys.A))
                camera1.MoveStrafeLeftRight(true);
            if (keyboardState.IsKeyDown(Keys.D))
                camera1.MoveStrafeLeftRight(false);

            // Move camera2 with IJKL keys
            if (keyboardState.IsKeyDown(Keys.I))
                camera2.MoveForwardBackward(true);
            if (keyboardState.IsKeyDown(Keys.K))
                camera2.MoveForwardBackward(false);
            if (keyboardState.IsKeyDown(Keys.J))
                camera2.MoveStrafeLeftRight(true);
            if (keyboardState.IsKeyDown(Keys.L))
                camera2.MoveStrafeLeftRight(false);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear border between screens
            GraphicsDevice.Clear(Color.Black);

            // Set current drawing camera for player 1
            // and set the viewport to player 1's viewport,
            // then clear and call base.Draw to invoke
            // the Draw method on the ModelManager component
            currentDrawingCamera = camera1;
            GraphicsDevice.Viewport = camera1.viewport;

            base.Draw(gameTime);

            // Set current drawing camera for player 2
            // and set the viewport to player 2's viewport,
            // then clear and call base.Draw to invoke
            // the Draw method on the ModelManager component
            currentDrawingCamera = camera2;
            GraphicsDevice.Viewport = camera2.viewport;

            base.Draw(gameTime);
        }
    }
}
