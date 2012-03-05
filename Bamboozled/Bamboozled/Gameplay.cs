using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Bamboozled
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Gameplay : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected KeyboardState keyboardState;
        private Player player;
        //private LevelManager level;

        SpriteBatch spriteBatch;

        public Gameplay(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            

            player = new Player(Game.Content, new Vector2(0, Game.Window.ClientBounds.Height - 109)); // This needs to happen in LevelManager now
            LevelManager.Initialize(Game.Content,player); 
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            //player.Update(gameTime, keyboardState); // This needs to happen in LevelManager now
            LevelManager.Update(gameTime, keyboardState);

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            
            //player.Draw(spriteBatch);

            LevelManager.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
