#region Using Statements
using System;
using System.IO;
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
#endregion

namespace Bamboozled
{
    public class Menu : GameScreen
    {
        #region Fields
        private Scrolling scrolling1, scrolling2;
        private Level level;
        protected KeyboardState keyboardState;
        ContentManager content;
        float pauseAlpha;
        SpriteFont gameFont;
        int timeSinceLastFlip;
        bool flip;
        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public Menu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            timeSinceLastFlip = 0;
            flip = false;
        }

        public override void LoadContent()
        {
            
            
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            level = new Level(content, keyboardState,ScreenManager);
            gameFont = content.Load<SpriteFont>("Fonts\\GameFont");
            scrolling1 = new Scrolling(content.Load<Texture2D>(@"Images\Pink Forest"), new Rectangle(0, 0, 1024, 576));
            scrolling2 = new Scrolling(content.Load<Texture2D>(@"Images\Pink Forest"), new Rectangle(1024, 0, 1024, 576));

            level.LoadContent();

            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            string new_level = "C:/temp/level.txt";
            //writeLevel(new_level,blocks);
            content.Unload();
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            

            if (IsActive)
            {
                keyboardState = Keyboard.GetState();
                level.keyboardState = keyboardState;
                
                if (level.player.isMoving && level.player.position.X >= ScreenManager.GraphicsDevice.Viewport.Width / 2)
                {
                    if (scrolling1.rectangle.X + scrolling1.texture.Width <= 0)
                        scrolling1.rectangle.X = scrolling2.rectangle.X + scrolling2.texture.Width;
                    if (scrolling2.rectangle.X + scrolling2.texture.Width <= 0)
                        scrolling2.rectangle.X = scrolling1.rectangle.X + scrolling1.texture.Width;
                    scrolling1.Update();
                    scrolling2.Update();
                    // SCROLL THE LEVEL HERE 
                    level.scroll();
                }

                if (level.player.isGameOver)
                {
                    ScreenManager.AddScreen(new GameOver(), ControllingPlayer);
                }

                if (level.isWinning)
                {
                    ScreenManager.AddScreen(new Win(), ControllingPlayer);
                }
                    
                base.Update(gameTime, otherScreenHasFocus, false);
                level.Update(gameTime, otherScreenHasFocus, false);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

        }

        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            
            level.Draw(gameTime, spriteBatch);
            scrolling1.Draw(spriteBatch);
            scrolling2.Draw(spriteBatch);

            if (level.isTripping)
            {
                scrolling1.texture = content.Load<Texture2D>(@"Images/pink_forest_inverted");
                scrolling2.texture = content.Load<Texture2D>(@"Images/pink_forest_inverted");
            }
            
            spriteBatch.End();

            

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

        }

        private bool isComma(char a)
        {
            if (a == ',')
                return true;
            else
                return false;
        }
        #endregion
    }
}
