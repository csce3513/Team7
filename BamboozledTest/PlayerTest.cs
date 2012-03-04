using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bamboozled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BamboozledTest
{
    [TestClass]
    public class PlayerTest
    {
        [TestMethod()]
        public void keysLeftDirectionTest()
        {
            //Vector2 temp = Vector2.Zero;
            //Player tempPlayer = new Player(Game.Content,temp);
            //KeyboardState keyA = new KeyboardState(Keys.A);         // Creates a manual KeyState, tricks the function into thinking the A key is pressed
            //KeyboardState keyLeft = new KeyboardState(Keys.Left);
            //Assert.IsTrue(tempPlayer.keysLeftDirection(keyA));      // Should be true, because A key is meant to Handle Left direction
            //Assert.IsTrue(tempPlayer.keysLeftDirection(keyLeft));  
        }
    }

}
