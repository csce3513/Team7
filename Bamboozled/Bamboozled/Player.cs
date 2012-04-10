﻿using System;
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
        public Point frameSize { get; set; }
        private Point currentFrame;
        private Point sheetSize;
        public Vector2 position;
        protected Vector2 speed;
        protected Vector2 acceleration;
        public Vector2 velocity { get; set; }
        protected int timeSinceLastFrame;
        protected int millisecondsPerFrame = 40;
        private KeyboardState keyboardState;
        public bool isOnPlatform { get; set; } // Quick hack to get jumping on platforms to work

        protected SpriteEffects directionOfMovement = SpriteEffects.None;
        public bool isMoving; //Temporary fix. need gettter and setter
        public bool isJumping { get; set; }
        private float maxJump;

        public Vector2 getPos()
        {
            return position;
        }

        public void setPos(Vector2 temppos)
        {
            position = temppos;
        }

        public Vector2 getAccel()
        {
            return acceleration;
        }

        public void setAccel(Vector2 tempAccel)
        {
            acceleration = tempAccel;
        }


        public Player(ContentManager content, Vector2 position)
        {
            textureImage = content.Load<Texture2D>(@"Images/panda_walking");
            frameSize = new Point(88, 96);
            currentFrame = new Point(0, 0);
            sheetSize = new Point(4, 0);
            this.position = position;
            speed = new Vector2(8, 8);
            acceleration = new Vector2(0, 0);
            maxJump = 15;
            
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            this.keyboardState = keyboardState;
            this.Movement(); // <---- Why are these functions seperate if the only time they're used, they're called right next to eachother?
            //this.Velocity(); // <----

            velocity += acceleration;
            if (position.Y + velocity.Y <= 576 - 109)
            {
                position += velocity;
                if (position.X > 1024 / 2)
                {
                    position.X = 1024 / 2;
                }
                else if (position.X < 0)
                {
                    position.X = 0;
                }
            }
            else
            {
                position.Y = 576 - 109;
            }


            if (isJumping)
            {
                // Loop through jumping animation
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

            Vector2 tempPos = this.getPos();
            Vector2 tempAccel = this.getAccel();
            if (tempPos.Y < 576 - 109) // Temporary way to detect bottom of screen, needs to be replaced with more modular code
            {
                if(tempAccel.Y + 1 < 20)
                    tempAccel.Y += 1;
                this.setAccel(tempAccel);
            }
            else
            {
                this.setAccel(Vector2.Zero);
            }

        }

        // Checks for key presses, moves player accordingly.
        private void Movement()
        {
            isMoving = false;
            isJumping = false;
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
                if (position.Y >= 576 - 109 || isOnPlatform==true) // If player isn't on platform(WIP) or ground
                {
                    if (keysUpDirection(keyboardState))     // If up key is pressed
                    {
                        jump();
                        isOnPlatform = false;
                        return;
                    }
                }
            }

            velocity = speed * inputDirection;    
        }

        private void jump()
        {
            if (position.Y >= maxJump)
            {
                isJumping = true;
                acceleration.Y += -20;
            }
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
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
               (int)(position.X),
                (int)(position.Y),
                (int)(frameSize.X),
                (int)(frameSize.Y));
            }
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
