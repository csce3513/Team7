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
    public class Player
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
        private bool isJumping;
        private float maxJump;

        public Player(ContentManager content, Vector2 position)
        {
            textureImage = content.Load<Texture2D>(@"Images/temp_char");
            frameSize = new Point(44, 88);
            currentFrame = new Point(0, 0);
            sheetSize = new Point(7, 1);
            this.position = position;
            speed = new Vector2(5, 5);
        }


        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            this.keyboardState = keyboardState;
            this.Movement(); // <---- Why are these functions seperate if the only time they're used, they're called right next to eachother?
            //this.Velocity(); // <----

            position += velocity;

            if (isJumping)
            {
                // Loop through jump animation
            }
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

        // Checks for key presses, moves player accordingly.
        private void Movement()
        {
            isMoving = false;
            Vector2 inputDirection = Vector2.Zero;

            if (!keysOppositeDirection(keyboardState))   // Ensures no movement or animation if opposing keys occur
            {
                if (keysLeftDirection(keyboardState))   // Left
                {
                    isMoving = true;
                    directionOfMovement = SpriteEffects.FlipHorizontally;
                    inputDirection.X -= 1;
                }
                if (keysRightDirection(keyboardState))  // Right
                {
                    isMoving = true;
                    directionOfMovement = SpriteEffects.None;
                    inputDirection.X += 1;
                }
                if (keysUpDirection(keyboardState))     // Jump (WIP)
                {
                    // Player should only jump if 
                    // 1.) Its a new key press
                    // 2.) He's on top of either the ground or a platform
                    // 3.) 
                    inputDirection.Y = -1;
                }
            }

            velocity = speed * inputDirection;    
        }

        //private void jump()
        //{
        //    isJumping = true;
        //    speed.Y = -500;
        //}

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

        #region KeyHandlers
        // Returns true if any two keys are being held that, false otherwise
        public bool keysOppositeDirection(KeyboardState currentKeys)
        {
            return ((currentKeys.IsKeyDown(Keys.Left) && currentKeys.IsKeyDown(Keys.Right)) ||
                    (currentKeys.IsKeyDown(Keys.Left) && currentKeys.IsKeyDown(Keys.D)) ||
                    (currentKeys.IsKeyDown(Keys.Right) && currentKeys.IsKeyDown(Keys.A)) ||
                    (currentKeys.IsKeyDown(Keys.A) && currentKeys.IsKeyDown(Keys.D))
                   );

        }


        // Returns true if either Left or A is held
        public bool keysLeftDirection(KeyboardState currentKeys)
        {
            return (currentKeys.IsKeyDown(Keys.Left) || currentKeys.IsKeyDown(Keys.A));
        }

        // Returns true if either Right or D is held
        public bool keysRightDirection(KeyboardState currentKeys)
        {
            return (currentKeys.IsKeyDown(Keys.Right) || currentKeys.IsKeyDown(Keys.D));
        }

        // Returns true if either Right or D is held
        public bool keysUpDirection(KeyboardState currentKeys)
        {
            return (currentKeys.IsKeyDown(Keys.Up) || currentKeys.IsKeyDown(Keys.W) || currentKeys.IsKeyDown(Keys.Space));
        }
        #endregion
    }
}
