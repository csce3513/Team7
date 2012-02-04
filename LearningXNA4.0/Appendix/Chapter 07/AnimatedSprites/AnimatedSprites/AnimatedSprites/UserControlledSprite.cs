using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnimatedSprites
{
    class UserControlledSprite: Sprite
    {
        // Movement stuff
        MouseState prevMouseState;

        // Get direction of sprite based on player input and speed
        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                // If player pressed arrow keys, move the sprite
                if (Keyboard.GetState(  ).IsKeyDown(Keys.Left))
                    inputDirection.X -= 1;
                if (Keyboard.GetState(  ).IsKeyDown(Keys.Right))
                    inputDirection.X += 1;
                if (Keyboard.GetState(  ).IsKeyDown(Keys.Up))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState(  ).IsKeyDown(Keys.Down))
                    inputDirection.Y += 1;

                // If player pressed the gamepad thumbstick, move the sprite
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if(gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if(gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y -= gamepadState.ThumbSticks.Left.Y;

                return inputDirection * speed;
            }
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, null)
        {
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, null)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite based on direction
            position += direction;

            // If player moved the mouse, move the sprite
            MouseState currMouseState = Mouse.GetState();
            if (currMouseState.X != prevMouseState.X ||
                currMouseState.Y != prevMouseState.Y)
            {
                position = new Vector2(currMouseState.X, currMouseState.Y);
            }
            prevMouseState = currMouseState;

            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;

            base.Update(gameTime, clientBounds);
        }
    }
}
