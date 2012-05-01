using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Bamboozled.ScreenManagement;
using Bamboozled.Screens;

namespace Bamboozled
{
    class Platform
    {
        public Rectangle Rect { get; set; }
        public Texture2D block { get; set; }
        public Vector2 location;

        public Platform(Vector2 initial_point,Texture2D picture)
        {
            location = initial_point;
            block = picture;
        }
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                (int)(location.X),
                (int)(location.Y),
                (int)(50),
                (int)(50));
            }
        }

        public void Scroll()
        {
            location.X -= 8;
        }

        public void ScrollRight()
        {
            location.X += 8;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
                (block,
                location,
                null,
                Color.White,
                0, 
                Vector2.Zero,
                .09f,
                SpriteEffects.None, 
                1);
        }
    }
}