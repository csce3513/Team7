using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bamboozled
{
    class Backgrounds
    {
        public Texture2D texture;
        public Rectangle rectangle;

        //public Texture2D Texture
        //{
        //    get { return texture; }
        //    set { texture = value; }
        //}

        //public Rectangle Rectangle
        //{
        //    get { return rectangle; }
        //    set { rectangle = value; }
        //}

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }

    class Scrolling : Backgrounds
    {
        public Scrolling(Texture2D texture, Rectangle rectangle)
        {
            this.texture = texture;
            this.rectangle = rectangle;
        }

        public void Update()
        {
            rectangle.X -= 1;
        }
        public void scrollRight()
        {
            rectangle.X += 1;
        }
    }
}
