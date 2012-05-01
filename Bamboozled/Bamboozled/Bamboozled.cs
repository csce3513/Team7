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
using Bamboozled.ScreenManagement;
using Bamboozled.Screens;

namespace Bamboozled
{
    public class Bamboozled : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;
        Gameplay gameplay;
        public static ContentManager global_content;

        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        public Bamboozled()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            global_content = Content;

            graphics.PreferredBackBufferHeight = 576;
            graphics.PreferredBackBufferWidth = 1024;

            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screen.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void Initialize()
        {
            gameplay = new Gameplay(this);
            //Components.Add(gameplay);
            //gameplay.Enabled = false;
            //gameplay.Visible = false;
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
