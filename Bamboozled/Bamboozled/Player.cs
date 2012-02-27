using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Bamboozled
{
    class Player
    {
        private Texture2D textureImage;
        private Point frameSize;
        private Point currentFrame;
        private Point sheetSize;
        protected Vector2 position;
        protected Vector2 speed;
        protected Vector2 velocity;
        protected int timeSinceLastFrame;
        protected int millisecondsPerFrame = 40;

        private KeyboardState keyboardState;

        protected SpriteEffects directionOfMovement = SpriteEffects.None;
        private bool isMoving;

        public Player(ContentManager content, Vector2 postition)
        {
            textureImage = content.Load<Texture2D>(@"Images/temp_char");
            frameSize = new Point(44, 88);
            currentFrame = new Point(0, 0);
            sheetSize = new Point(7, 1);
            this.position = postition;
            speed = new Vector2(5, 5);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            this.keyboardState = keyboardState;
            this.Movement(); // <---- Why are these functions seperate if the only time they're used, they're called right next to eachother?
            this.Velocity(); // <----

            position += velocity;

            if (isMoving)
            {
                // Update frame if time to do so based on framerate
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    // Increment to next frame
                    timeSinceLastFrame = 0;
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {
                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSize.Y)
                            currentFrame.Y = 0;
                    }
                }
            }
            else
            {
                currentFrame.X = 1;
            }
        }

        private void Movement()
        {
            isMoving = false;

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                isMoving = true;
                directionOfMovement = SpriteEffects.FlipHorizontally;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                isMoving = true;
                directionOfMovement = SpriteEffects.None;
            }
        }

        private void Velocity()
        {
            Vector2 inputDirection = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.Left)||keyboardState.IsKeyDown(Keys.A))
            {
                inputDirection.X -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.Right)||keyboardState.IsKeyDown(Keys.D))
            {
                inputDirection.X += 1;
            }
            velocity = speed * inputDirection;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
                (textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1f, directionOfMovement, 1);
        }
    }
}
