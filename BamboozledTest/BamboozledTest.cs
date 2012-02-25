using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bamboozled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BamboozledTest
{
    [TestClass]
    public class BamboozledTest
    {
        Bamboozled.Bamboozled test = new Bamboozled.Bamboozled();
        [TestMethod]
        public void getKeyboardInputTest()
        {
            KeyboardState initalKeyboardState = Keyboard.GetState(); ;
            KeyboardState bamboozledkeyboardState = test.getKeyboardInput();
            Assert.IsTrue(initalKeyboardState == bamboozledkeyboardState);
        }
    }
}
