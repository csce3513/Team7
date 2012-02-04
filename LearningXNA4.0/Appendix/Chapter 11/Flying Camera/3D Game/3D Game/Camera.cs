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

        // Camera vectors
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        // Speed
        float speed = 3;

        // Mouse stuff
        MouseState prevMouseState;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();


            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 3000);
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Remove the Y component of the camera direction
            Vector3 movementDirection = cameraDirection;
            movementDirection.Y = 0;

            //Normalize the vector to ensure constant speed
            movementDirection.Normalize();

            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += movementDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= movementDirection * speed;

            // Move side to side
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;

            // Yaw rotation
            cameraDirection = Vector3.Transform(cameraDirection,
                Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                (Mouse.GetState().X - prevMouseState.X)));

            // Roll rotation
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                cameraUp = Vector3.Transform(cameraUp,
                    Matrix.CreateFromAxisAngle(cameraDirection,
                    MathHelper.PiOver4 / 45));
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                cameraUp = Vector3.Transform(cameraUp,
                    Matrix.CreateFromAxisAngle(cameraDirection,
                    -MathHelper.PiOver4 / 45));
            }

            // Pitch rotation
            cameraDirection = Vector3.Transform(cameraDirection,
                Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                (MathHelper.PiOver4 / 100) *
                (Mouse.GetState().Y - prevMouseState.Y)));

            cameraUp = Vector3.Transform(cameraUp,
                Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                (MathHelper.PiOver4 / 100) *
                (Mouse.GetState().Y - prevMouseState.Y)));

            // Reset prevMouseState
            prevMouseState = Mouse.GetState();

            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }
    }
}