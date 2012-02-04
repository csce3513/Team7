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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace _3D_Game
{

    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Viewport viewport { get; set; }

        // Vectors for the view matrix
        Vector3 cameraPosition;
        Vector3 cameraDirection;
        Vector3 cameraUp;

        // Speed
        float speed = 3;

        public Camera(Game game, Vector3 pos, Vector3 target,
            Vector3 up, Viewport viewport)
            : base(game)
        {
            // Create view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)viewport.Width /
                (float)viewport.Height,
                1, 3000);

            this.viewport = viewport;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            CreateLookAt();


            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }

        public void MoveForwardBackward(bool forward)
        {
            // Move forward/backward
            if (forward)
                cameraPosition += cameraDirection * speed;
            else
                cameraPosition -= cameraDirection * speed;
        }

        public void MoveStrafeLeftRight(bool left)
        {

            // Strafe
            if (left)
            {
                cameraPosition +=
                   Vector3.Cross(cameraUp, cameraDirection) * speed;
            }
            else
            {
                cameraPosition -=
                    Vector3.Cross(cameraUp, cameraDirection) * speed;
            }
        }
    }
}