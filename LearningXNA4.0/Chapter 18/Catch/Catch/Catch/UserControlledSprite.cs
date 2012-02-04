using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Catch
{
    class UserControlledSprite: Sprite
    {
        public int score { get; set; }

        public bool isChasing { get; set; }

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
            Vector2 speed, bool isChasing)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, null, 0)
        {
            score = 0;
            this.isChasing = isChasing;
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, bool isChasing)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, null, 0)
        {
            score = 0;
            this.isChasing = isChasing;
        }

        public void Update(GameTime gameTime,
            Rectangle clientBounds, bool moveSprite)
        {
            if (moveSprite)
            {
                // Move the sprite according to the direction property
                position += direction;

                // If the sprite is off the screen, put it back in play
                if (position.X < 0)
                    position.X = 0;
                if (position.Y < 0)
                    position.Y = 0;
                if (position.X > clientBounds.Width - frameSize.X)
                    position.X = clientBounds.Width - frameSize.X;
                if (position.Y > clientBounds.Height - frameSize.Y)
                    position.Y = clientBounds.Height - frameSize.Y;
            }

            base.Update(gameTime, clientBounds);
        }
    }
}
