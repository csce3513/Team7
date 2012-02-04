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


namespace PhoneAsteroids
{

    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }



        public Camera(Game game)
            : base(game)
        {
            // Build camera view matrix
            view = Matrix.CreateLookAt(new Vector3(0, 0, 200),
                Vector3.Zero, Vector3.Up);


            // Build camera projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                1.6666f,
                1, 3000);

        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }



    }
}