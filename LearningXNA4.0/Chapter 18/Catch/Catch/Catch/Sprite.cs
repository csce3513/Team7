using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catch
{
    abstract class Sprite
    {
        // Stuff needed to draw the sprite
        public Texture2D textureImage { get; set; }
        protected Point frameSize;
        Point currentFrame;
        public Point sheetSize { get; set; }
        protected float scale = 1;
        protected float originalScale = 1;

        // Speed stuff
        public Vector2 originalSpeed { get; set; }

        // Collision data
        int collisionOffset;
        
        // Framerate stuff
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;

        // Movement data
        public Vector2 speed { get; set; }
        protected Vector2 position;

        // Sound stuff
        public string collisionCueName { get; private set; }

        // Abstract definition of direction property
        public abstract Vector2 direction
        {
            get;
        }

        // Get current position of the sprite
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        

        // Get/set score
        public int scoreValue { get; protected set; }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            string collisionCueName, int scoreValue)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, defaultMillisecondsPerFrame, collisionCueName,
            scoreValue)
        {
        }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, string collisionCueName, int scoreValue)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            originalSpeed = speed;
            this.collisionCueName = collisionCueName;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.scoreValue = scoreValue;
        }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            string collisionCueName, int scoreValue, float scale)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, defaultMillisecondsPerFrame, collisionCueName,
            scoreValue)
        {
            this.scale = scale;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
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

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                scale, SpriteEffects.None, 0);
        }

        // Gets the collision rect based on position, framesize and collision offset
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)(position.X + (collisionOffset * scale)),
                    (int)(position.Y + (collisionOffset * scale)),
                    (int)((frameSize.X - (collisionOffset * 2)) * scale),
                    (int)((frameSize.Y - (collisionOffset * 2)) * scale));
            }
        }

        // Detect if this sprite is off the screen and irrelevant
        public bool IsOutOfBounds(Rectangle clientRect)
        {
            if (position.X < -frameSize.X ||
                position.X > clientRect.Width ||
                position.Y < -frameSize.Y ||
                position.Y > clientRect.Height)
            {
                return true;
            }

            return false;
        }

        public void ModifyScale(float modifier)
        {
            scale *= modifier;
        }
        
        public void ResetScale()
        {
            scale = originalScale;
        }
        
        public void ModifySpeed(float modifier)
        {
            speed *= modifier;
        }
    
        public void ResetSpeed()
        {
            speed = originalSpeed;
        }
    }
}
