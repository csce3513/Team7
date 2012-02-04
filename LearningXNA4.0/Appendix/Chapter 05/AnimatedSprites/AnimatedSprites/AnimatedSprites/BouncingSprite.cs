using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class BouncingSprite: AutomatedSprite
    {
        public BouncingSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset,
            currentFrame, sheetSize, speed)
        {
        }
        public BouncingSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset,
            currentFrame, sheetSize, speed, millisecondsPerFrame)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;

            //Reverse direction if hit a side
            if (position.X > clientBounds.Width - frameSize.X ||
                position.X < 0)
                speed.X *= -1;
            if (position.Y > clientBounds.Height - frameSize.Y ||
                position.Y < 0)
                speed.Y *= -1;

            base.Update(gameTime, clientBounds);
        }
    }
}