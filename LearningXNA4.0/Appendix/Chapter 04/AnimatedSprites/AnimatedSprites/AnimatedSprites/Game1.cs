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

namespace AnimatedSprites
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Rings variables
        Texture2D ringsTexture;
        Point ringsFrameSize = new Point(75, 75);
        Point ringsCurrentFrame = new Point(0, 0);
        Point ringsSheetSize = new Point(6, 8);
        int ringsTimeSinceLastFrame = 0;
        int ringsMillisecondsPerFrame = 50;


        //Skull variables
        Texture2D skullTexture;
        Point skullFrameSize = new Point(75, 75);
        Point skullCurrentFrame = new Point(0, 0);
        Point skullSheetSize = new Point(6, 8);
        int skullTimeSinceLastFrame = 0;
        const int skullMillisecondsPerFrame = 50;

        //Plus variables
        Texture2D plusTexture;
        Point plusFrameSize = new Point(75, 75);
        Point plusCurrentFrame = new Point(0, 0);
        Point plusSheetSize = new Point(6, 4);
        int plusTimeSinceLastFrame = 0;
        const int plusMillisecondsPerFrame = 50;

        //Rings movement
        Vector2 ringsPosition = Vector2.Zero;
        const float ringsSpeed = 6;
        MouseState prevMouseState;

        //Skull position
        Vector2 skullPosition = new Vector2(100, 100);
        Vector2 skullSpeed = new Vector2(4, 2);

        //Plus position
        Vector2 plusPosition = new Vector2(200, 200);
        Vector2 plusSpeed = new Vector2(2, 5);

        //Collision detection variables
        int ringsCollisionRectOffset = 10;
        int skullCollisionRectOffset = 10;
        int plusCollisionRectOffset = 10;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ringsTexture = Content.Load<Texture2D>(@"images \threerings");
            skullTexture = Content.Load<Texture2D>(@"images\skullball");
            plusTexture = Content.Load<Texture2D>(@"images\plus");
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

            //Update time since last frame and only
            //change animation if framerate expired
            ringsTimeSinceLastFrame +=
                gameTime.ElapsedGameTime.Milliseconds;
            if (ringsTimeSinceLastFrame > ringsMillisecondsPerFrame)
            {
                ringsTimeSinceLastFrame -= ringsMillisecondsPerFrame;

                ++ringsCurrentFrame.X;
                if (ringsCurrentFrame.X >= ringsSheetSize.X)
                {
                    ringsCurrentFrame.X = 0;
                    ++ringsCurrentFrame.Y;
                    if (ringsCurrentFrame.Y >= ringsSheetSize.Y)
                        ringsCurrentFrame.Y = 0;
                }
            }

            //Then do the same to update the skull animation
            skullTimeSinceLastFrame +=
                gameTime.ElapsedGameTime.Milliseconds;
            if (skullTimeSinceLastFrame > skullMillisecondsPerFrame)
            {
                skullTimeSinceLastFrame -= skullMillisecondsPerFrame;

                ++skullCurrentFrame.X;
                if (skullCurrentFrame.X >= skullSheetSize.X)
                {
                    skullCurrentFrame.X = 0;
                    ++skullCurrentFrame.Y;
                    if (skullCurrentFrame.Y >= skullSheetSize.Y)
                        skullCurrentFrame.Y = 0;
                }
            }

            //Then do the same to update the plus animation
            plusTimeSinceLastFrame +=
                gameTime.ElapsedGameTime.Milliseconds;
            if (plusTimeSinceLastFrame > plusMillisecondsPerFrame)
            {
                plusTimeSinceLastFrame -= plusMillisecondsPerFrame;

                ++plusCurrentFrame.X;
                if (plusCurrentFrame.X >= plusSheetSize.X)
                {
                    plusCurrentFrame.X = 0;
                    ++plusCurrentFrame.Y;
                    if (plusCurrentFrame.Y >= plusSheetSize.Y)
                        plusCurrentFrame.Y = 0;
                }
            }

            //Move position of rings based on keyboard input
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                ringsPosition.X -= ringsSpeed;
            if (keyboardState.IsKeyDown(Keys.Right))
                ringsPosition.X += ringsSpeed;
            if (keyboardState.IsKeyDown(Keys.Up))
                ringsPosition.Y -= ringsSpeed;
            if (keyboardState.IsKeyDown(Keys.Down))
                ringsPosition.Y += ringsSpeed;

            //Move the skull
            skullPosition += skullSpeed;
            if (skullPosition.X >
                Window.ClientBounds.Width - skullFrameSize.X ||
                skullPosition.X < 0)
                skullSpeed.X *= -1;
            if (skullPosition.Y >
                Window.ClientBounds.Height - skullFrameSize.Y ||
                skullPosition.Y < 0)
                skullSpeed.Y *= -1;

            //Move the plus
            plusPosition += plusSpeed;
            if (plusPosition.X >
                Window.ClientBounds.Width - plusFrameSize.X ||
                plusPosition.X < 0)
                plusSpeed.X *= -1;
            if (plusPosition.Y >
                Window.ClientBounds.Height - plusFrameSize.Y ||
                plusPosition.Y < 0)
                plusSpeed.Y *= -1;

            //Move rings based on mouse movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.X != prevMouseState.X ||
            mouseState.Y != prevMouseState.Y)
                ringsPosition = new Vector2(mouseState.X, mouseState.Y);
            prevMouseState = mouseState;

            //Move rings based on gamepad input
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            if (gamepadState.Buttons.A == ButtonState.Pressed)
            {
                //A is pressed, double speed and vibrate
                ringsPosition.X +=
                    ringsSpeed * 2 * gamepadState.ThumbSticks.Left.X;
                ringsPosition.Y -=
                    ringsSpeed * 2 * gamepadState.ThumbSticks.Left.Y;
                GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
            }
            else
            {
                //A is not pressed, normal speed and stop vibration
                ringsPosition.X += ringsSpeed * gamepadState.ThumbSticks.Left.X;
                ringsPosition.Y -= ringsSpeed * gamepadState.ThumbSticks.Left.Y;
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }

            //Adjust position of rings to keep it in the game window
            if (ringsPosition.X < 0)
                ringsPosition.X = 0;
            if (ringsPosition.Y < 0)
                ringsPosition.Y = 0;
            if (ringsPosition.X >
                Window.ClientBounds.Width - ringsFrameSize.X)
                ringsPosition.X =
                    Window.ClientBounds.Width - ringsFrameSize.X;
            if (ringsPosition.Y >
                Window.ClientBounds.Height - ringsFrameSize.Y)
                ringsPosition.Y =
                    Window.ClientBounds.Height - ringsFrameSize.Y;

            //If objects collide, exit the game
            if (Collide())
                Exit();

            base.Update(gameTime);
        }

        protected bool Collide()
        {
            Rectangle ringsRect = new Rectangle(
                (int)ringsPosition.X + ringsCollisionRectOffset,
                (int)ringsPosition.Y + ringsCollisionRectOffset,
                ringsFrameSize.X - (ringsCollisionRectOffset * 2),
                ringsFrameSize.Y - (ringsCollisionRectOffset * 2));

            Rectangle skullRect = new Rectangle(
                (int)skullPosition.X + skullCollisionRectOffset,
                (int)skullPosition.Y + skullCollisionRectOffset,
                skullFrameSize.X - (skullCollisionRectOffset * 2),
                skullFrameSize.Y - (skullCollisionRectOffset * 2));

            Rectangle plusRect = new Rectangle(
                (int)plusPosition.X + plusCollisionRectOffset,
                (int)plusPosition.Y + plusCollisionRectOffset,
                plusFrameSize.X - (plusCollisionRectOffset * 2),
                plusFrameSize.Y - (plusCollisionRectOffset * 2));

            return ringsRect.Intersects(skullRect) ||
                ringsRect.Intersects(plusRect);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);


            //Draw the rings
            spriteBatch.Draw(ringsTexture, ringsPosition,
                new Rectangle(ringsCurrentFrame.X * ringsFrameSize.X,
                    ringsCurrentFrame.Y * ringsFrameSize.Y,
                    ringsFrameSize.X,
                    ringsFrameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None, 0);

            //Draw the skull
            spriteBatch.Draw(skullTexture, skullPosition,
                new Rectangle(skullCurrentFrame.X * skullFrameSize.X,
                    skullCurrentFrame.Y * skullFrameSize.Y,
                    skullFrameSize.X,
                    skullFrameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None, 0);

            //Draw the plus
            spriteBatch.Draw(plusTexture, plusPosition,
                new Rectangle(plusCurrentFrame.X * plusFrameSize.X,
                    plusCurrentFrame.Y * plusFrameSize.Y,
                    plusFrameSize.X,
                    plusFrameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}