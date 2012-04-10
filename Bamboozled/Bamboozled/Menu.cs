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
        /*GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Gameplay gameplay;
         */

        #region Fields
        private Scrolling scrolling1, scrolling2;

        private Player player;
        private static Texture2D background; // Temporary fix
        //private Platform one_block;
        protected KeyboardState keyboardState;
        List<Platform> blocks;
        ContentManager content;
        float pauseAlpha;
        SpriteFont gameFont;
        Platform[,] one_level;

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public Menu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

        public override void LoadContent()
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("Fonts\\GameFont");
            //background = content.Load<Texture2D>(@"Images\forest04");
            scrolling1 = new Scrolling(content.Load<Texture2D>(@"Images\Pink Forest"), new Rectangle(0, 0, 1024, 576));
            scrolling2 = new Scrolling(content.Load<Texture2D>(@"Images\Pink Forest"), new Rectangle(1024, 0, 1024, 576));
            player = new Player(content, new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height - 88));


            // Textures for each type of box. Can be used to build a level
            Texture2D left_edge = content.Load<Texture2D>(@"Images\dirt_left_edge");
            Texture2D right_edge = content.Load<Texture2D>(@"Images\dirt_right_edge");
            Texture2D left_center = content.Load<Texture2D>(@"Images\dirt_left_center");
            Texture2D right_center = content.Load<Texture2D>(@"Images\dirt_right_center");
            Texture2D bottom_center = content.Load<Texture2D>(@"Images\dirt_bottom_center");
            Texture2D bottom_left = content.Load<Texture2D>(@"Images\dirt_bottom_left");
            Texture2D bottom_right = content.Load<Texture2D>(@"Images\dirt_bottom_right");


            // Code to generate a simple level with 5 long islands, each at a higher point.(and several small islands in between)
            Vector2 initial = new Vector2(300, 500);
            blocks = new List<Platform>();
            initial.X += 50; 
            for (int i = 0; i < 2; i++)
            {
                blocks.Add(new Platform(new Vector2(initial.X-50, initial.Y), left_edge));
                blocks.Add(new Platform(new Vector2(initial.X-50, initial.Y+50), bottom_left));
                for (int j = 0; j < 200; j++)
                {
                    blocks.Add(new Platform(initial, left_center));
                    blocks.Add(new Platform(new Vector2(initial.X + 50, initial.Y), right_center));
                    blocks.Add(new Platform(new Vector2(initial.X, initial.Y + 50), bottom_center));
                    blocks.Add(new Platform(new Vector2(initial.X + 50, initial.Y + 50), bottom_center));
                    if(j%10==0) // Every 20th block, make a floating island
                    {
                        // Left side
                        blocks.Add(new Platform(new Vector2(initial.X,initial.Y-150),bottom_left));
                        blocks.Add(new Platform(new Vector2(initial.X, initial.Y - 200), left_edge));

                        // Center
                        blocks.Add(new Platform(new Vector2(initial.X+50, initial.Y - 200), left_center));
                        blocks.Add(new Platform(new Vector2(initial.X+50, initial.Y - 150), bottom_center));
                        blocks.Add(new Platform(new Vector2(initial.X + 100, initial.Y - 200), right_center));
                        blocks.Add(new Platform(new Vector2(initial.X + 100, initial.Y - 150), bottom_center));

                        // Right side
                        blocks.Add(new Platform(new Vector2(initial.X + 150, initial.Y - 150), bottom_right));
                        blocks.Add(new Platform(new Vector2(initial.X + 150, initial.Y - 200), right_edge));
                    }
                    initial.X += 100;
                }
                blocks.Add(new Platform(new Vector2(initial.X, initial.Y), right_edge));
                blocks.Add(new Platform(new Vector2(initial.X, initial.Y + 50), bottom_right));
                initial.Y -= 100;
                initial.X += 300;

            }

            initial = new Vector2(initial.X,initial.Y+200);
            // One more long island
            for (int i = 0; i < 4; i++)
            {
                blocks.Add(new Platform(new Vector2(initial.X - 50, initial.Y), left_edge));
                blocks.Add(new Platform(new Vector2(initial.X - 50, initial.Y + 50), bottom_left));
                for (int j = 0; j < 200; j++)
                {
                    blocks.Add(new Platform(initial, left_center));
                    blocks.Add(new Platform(new Vector2(initial.X + 50, initial.Y), right_center));
                    blocks.Add(new Platform(new Vector2(initial.X, initial.Y + 50), bottom_center));
                    blocks.Add(new Platform(new Vector2(initial.X + 50, initial.Y + 50), bottom_center));
                    initial.X += 100;
                }
            }
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }
        #endregion

        #region Update and Draw
        /*  protected override void Update(GameTime gameTime)
        {

        }
       */

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
                player.Update(gameTime, keyboardState);


                if (player.isMoving && player.position.X >= ScreenManager.GraphicsDevice.Viewport.Width / 2)
                {
                    if (scrolling1.rectangle.X + scrolling1.texture.Width <= 0)
                        scrolling1.rectangle.X = scrolling2.rectangle.X + scrolling2.texture.Width;
                    if (scrolling2.rectangle.X + scrolling2.texture.Width <= 0)
                        scrolling2.rectangle.X = scrolling1.rectangle.X + scrolling1.texture.Width;
                    scrolling1.Update();
                    scrolling2.Update();
                    foreach (Platform box in blocks) // Scroll each block as well
                        box.Scroll();
                }
                //if (player.isMoving && player.position.X <= 100) // Scrolling Left Quick Patch, still very broken.
                //{
                //    if (scrolling1.rectangle.X + scrolling1.texture.Width <= 0)
                //        scrolling1.rectangle.X = scrolling2.rectangle.X + scrolling2.texture.Width;
                //    if (scrolling2.rectangle.X + scrolling2.texture.Width <= 0)
                //        scrolling2.rectangle.X = scrolling1.rectangle.X + scrolling1.texture.Width;
                //    scrolling1.scrollRight();
                //    scrolling2.scrollRight();
                //    foreach (Platform box in blocks) // Scroll each block as well
                //        box.ScrollRight();
                    
                //}
                foreach (Platform one_block in blocks) // Is the player colliding with any boxes on the screen?
                {
                    if (player.collisionRect.Intersects(one_block.collisionRect))
                    {
                        if (player.collisionRect.Bottom > one_block.collisionRect.Top && player.collisionRect.Bottom < one_block.collisionRect.Bottom - (one_block.collisionRect.Height / 2))
                        {
                            player.position = new Vector2(player.position.X, one_block.collisionRect.Top - player.frameSize.Y);
                            player.velocity = Vector2.Zero;
                            player.setAccel(Vector2.Zero);
                            player.isJumping = false;
                            player.isOnPlatform = true;
                            continue; // The continues mean that if he lands on the block, it won't even check for the left/right cases
                        }
                        if (player.collisionRect.Top < one_block.collisionRect.Bottom && player.collisionRect.Top > one_block.collisionRect.Top - (one_block.collisionRect.Height / 2))
                        {
                            player.position = new Vector2(player.position.X, one_block.collisionRect.Bottom);
                            player.velocity = Vector2.Zero;
                            player.setAccel(Vector2.Zero);
                            player.isJumping = false;
                            player.isOnPlatform = true;
                            continue;
                        }
                        if (player.collisionRect.Right > one_block.collisionRect.Left && player.collisionRect.Right < one_block.collisionRect.Right - (one_block.collisionRect.Width / 2))
                        {
                            player.position = new Vector2(one_block.collisionRect.Left - player.frameSize.X, player.position.Y);
                        }
                        if (player.collisionRect.Left < one_block.collisionRect.Right && player.collisionRect.Left > one_block.collisionRect.Right - (one_block.collisionRect.Width / 2))
                        {
                            player.position = new Vector2(one_block.collisionRect.Right, player.position.Y);
                        }


                    }
                }
                base.Update(gameTime, otherScreenHasFocus, false);
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
            else
            {
            }
        }

        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            scrolling1.Draw(spriteBatch);
            scrolling2.Draw(spriteBatch);

            player.Draw(spriteBatch);

            foreach (Platform single in blocks)
                single.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

        }

        #endregion
    }
}
