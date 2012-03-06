using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bamboozled
{
    public static class LevelManager
    {
        #region Declarations
        private static ContentManager Content;
        private static Player player;
        private static int currentLevel;
        private static Vector2 respawnLocation;

        private static Texture2D background;
        //private static KeyboardState keyboardState;

        //private static List<Enemy> enemies = new List<Enemy>(); // For when the Enemy class is built
        #endregion

        #region Properties
        public static int CurrentLevel
        {
            get { return currentLevel; }
        }

        public static Vector2 RespawnLocation
        {
            get { return respawnLocation; }
            set { respawnLocation = value; }
        }
        #endregion

        #region Initialization
        public static void Initialize(ContentManager content, Player gamePlayer)
        {
            Content = content;
            player = gamePlayer;
            background = Content.Load<Texture2D>(@"Images\forest04");
            //keyboardState = keys;
        }
        #endregion

        #region Helper Methods
        // private static void checkCurrentCellCode()
        #endregion

        #region Public Methods
        public static void LoadLevel(int levelNumber)
        {

        }
        // public static void ReloadLevel()

        public static void Update(GameTime gameTime, KeyboardState keyboardState)
        {

            // Gravity (currently only effects player. Needs to be reworked to effect player and all enemies)
            Vector2 tempPos = player.getPos();
            Vector2 tempAccel = player.getAccel();
            if (tempPos.Y < 576 - 109) // Temporary way to detect bottom of screen, needs to be replaced with more modular code
            {
                tempAccel.Y += 1;
                player.setAccel(tempAccel);
            }
            else
            {
                player.setAccel(Vector2.Zero);
            }

            
            player.Update(gameTime,keyboardState);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1024, 576), Color.White);
            player.Draw(spriteBatch);
        }
        #endregion

    }
}
