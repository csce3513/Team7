using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Bamboozled
{
    public class Player
    {
        private Texture2D textureImage;
        public Point frameSize { get; set; }
        private Point currentFrame;
        private Point sheetSizeWalking;
        private Point sheetSizeJumping;
        public Vector2 position;
        public Vector2 speed { get; set; }
        public Vector2 acceleration;
        public Vector2 velocity { get; set; }
        protected int timeSinceLastFrame;
        protected int millisecondsPerFrame = 70;
        private KeyboardState keyboardState;
        public bool isOnPlatform { get; set; } // Quick hack to get jumping on platforms to work
        public int lives { get; set; }
        public bool isGameOver { get; set; }
        protected SpriteEffects directionOfMovement = SpriteEffects.None;
        public bool isMoving; //Temporary fix. need gettter and setter
        public bool isJumping { get; set; }
        private float maxJump;
        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect jump_sound;
        ContentManager local_content;
        public bool jetpackActive;
        private Texture2D textureImageJetpack;
        public Point frameSizeJetpack { get; set; }
        private Point currentFrameJetpack;
        private Point sheetSizeJetpack;
        public Vector2 positionJetpack;
        private Vector2 jetpackOffset;
        public bool isAnimated;
        public int jetpackFuel { get; set; }
        protected int timeOfJetpack;
        private bool died = false;
        private int defualtJetpackTime = 5000;

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
            local_content = content;
            textureImage = content.Load<Texture2D>(@"Images/panda_walking_jumping");
            frameSize = new Point(88, 95);
            currentFrame = new Point(0, 0);
            sheetSizeWalking = new Point(4, 0);
            sheetSizeJumping = new Point(10, 1);
            this.position = position;
            speed = new Vector2(8, 8);
            acceleration = new Vector2(0, 0);
            maxJump = 15;
            lives = 8;
            soundEngine = content.Load<SoundEffect>("Audio\\Waves\\explosion");
            soundEngineInstance = soundEngine.CreateInstance();
            jump_sound = content.Load<SoundEffect>("Audio\\Waves\\jump");
            textureImageJetpack = content.Load<Texture2D>(@"Images/jetpack");
            frameSizeJetpack = new Point(32, 63);
            currentFrameJetpack = new Point(2, 2);
            jetpackOffset = new Vector2(-4, 25);
            positionJetpack = this.position + jetpackOffset;
            jetpackFuel = defualtJetpackTime;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            this.keyboardState = keyboardState;
            Movement();

            if (died)
            {
                died = false;
                jetpackFuel = defualtJetpackTime;
            }

            velocity += acceleration;
            position += velocity;
            if (jetpackActive && jetpackFuel >= 1)
            {
                position += new Vector2(0, -5);
                jetpackFuel -= gameTime.ElapsedGameTime.Milliseconds;
            }
            if (position.X > 1024 / 2)
                position.X = 1024 / 2;
            else if (position.X < 0)
                position.X = 0;
            positionJetpack = position + jetpackOffset;
            animate(gameTime);
            Vector2 tempPos = this.getPos();
            Vector2 tempAccel = this.getAccel();
            // THIS IS WHERE IT IS
            if (tempAccel.Y + 1 < 20 && (!jetpackActive || jetpackFuel <=0)) 
                tempAccel.Y += 1;
            else
                tempAccel.Y = 0;
            this.setAccel(tempAccel);
            isJumping = true;
            if (position.Y <= -251)
                position.Y = -250;
        }
        private void Jetpack(GameTime gameTime)
        {
            if (jetpackActive)
            {
                jetpackFuel -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void set_tripping()
        {
            textureImage = local_content.Load<Texture2D>(@"Images/panda_tripping");
        }

        public void set_regular()
        {
            textureImage = local_content.Load<Texture2D>(@"Images/panda_walking_jumping");
        }

        private void animate(GameTime gameTime)
        {

            if (isAnimated)
            {
                //Update frame if time to do so based on framerate
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    // Increment to next frame
                    timeSinceLastFrame = 0;
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSizeWalking.X)
                    {
                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSizeWalking.Y)
                            currentFrame.Y = 0;
                    }
                }
            }
            else
            {
                currentFrame.X = 1;
            }
            if (jetpackActive && jetpackFuel >= 1)
                currentFrameJetpack = new Point(2, 1);
            else
                currentFrameJetpack = new Point(2, 2);
        }

        public void kill()
        {
            isGameOver = false;
            if (lives > 1)
                lives--;
            else
                isGameOver = true;
            died = true;
        }
            

        // Checks for key presses, moves player accordingly.
        private void Movement()
        {
            isMoving = false;
            isJumping = false;
            jetpackActive = false;
            isAnimated = false;
            Vector2 inputDirection = Vector2.Zero;

            if (!keysOppositeDirection(keyboardState))   // Ensures no movement or animation if opposing keys occur
            {
                if (keysLeftDirection(keyboardState))   // Left
                {
                    isMoving = true;
                    isAnimated = true;
                    directionOfMovement = SpriteEffects.FlipHorizontally;
                    jetpackOffset = new Vector2(60, 25);
                    inputDirection.X -= 1;
                }
                if (keysRightDirection(keyboardState))  // Right
                {
                    isMoving = true;
                    isAnimated = true;
                    directionOfMovement = SpriteEffects.None;
                    jetpackOffset = new Vector2(-4, 25);
                    inputDirection.X += 1;
                }
                if (keysSpace(keyboardState))
                {
                    isAnimated = false;
                    jetpackActive = true;
                }
                if (isOnPlatform==true) // If player isn't on platform(WIP) or ground
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

        public void jump()
        {
            jump_sound.Play(.2f, 0f, 0f);
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
            spriteBatch.Draw(textureImageJetpack,
                positionJetpack,
                new Rectangle(currentFrameJetpack.X * frameSizeJetpack.X,
                    currentFrameJetpack.Y * frameSizeJetpack.Y,
                    frameSizeJetpack.X, frameSizeJetpack.Y),
                    Color.White, 0, Vector2.Zero,
                1f, directionOfMovement, 1);
        }
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                (int)(position.X - 10),
                (int)(position.Y),
                (int)(frameSize.X - 10),
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
            return (currentKeys.IsKeyDown(Keys.Up) || currentKeys.IsKeyDown(Keys.W));
        }
        public bool keysSpace(KeyboardState currentKeys)
        {
            return (currentKeys.IsKeyDown(Keys.Space));
        }
        #endregion
    }
}
