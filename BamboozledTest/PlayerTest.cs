using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bamboozled;

namespace BamboozledTest
{
    [TestClass]
    public class PlayerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        /// <summary>
        ///A test for Player Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Bamboozled.exe")]
        public void PlayerConstructorTest()
        {
            Player_Accessor target = new Player_Accessor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
