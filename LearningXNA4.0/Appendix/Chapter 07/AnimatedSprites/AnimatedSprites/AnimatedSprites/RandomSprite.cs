using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class RandomSprite : Sprite
    {
        SpriteManager spriteManager;

        //Random variable to determine when to change directions
        int minChangeTime = 500;
        int maxChangeTime = 1000;
        int changeDirectionTimer;
        Random rnd;

        public RandomSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, string collisionCueName,
            SpriteManager spriteManager, Random rnd)
            : base(textureImage, position, frameSize, collisionOffset,
            currentFrame, sheetSize, speed, collisionCueName)
        {
            this.spriteManager = spriteManager;
            this.rnd = rnd;
            ResetTimer();
        }

        public RandomSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame,
            string collisionCueName, SpriteManager spriteManager,
            Random rnd)
            : base(textureImage, position, frameSize, collisionOffset,
            currentFrame, sheetSize, speed, millisecondsPerFrame,
            collisionCueName)
        {
            this.spriteManager = spriteManager;
            this.rnd = rnd;
            ResetTimer();
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Move forward
            position += speed;
            Vector2 player = spriteManager.GetPlayerPosition();

            //Is it time to change directions?
            changeDirectionTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (changeDirectionTimer < 0)
            {
                //Pick a new random direction
                float Length = speed.Length();
                speed = new Vector2((float)rnd.NextDouble() - .5f,
                   (float)rnd.NextDouble() - .5f);
                speed.Normalize();
                speed *= Length;

                ResetTimer();
            }

            base.Update(gameTime, clientBounds);
        }

        private void ResetTimer()
        {
            changeDirectionTimer = rnd.Next(
                minChangeTime, maxChangeTime);
        }

    }
}