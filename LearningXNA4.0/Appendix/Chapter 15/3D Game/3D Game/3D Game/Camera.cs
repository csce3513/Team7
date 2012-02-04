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
        public Vector3 cameraUp;

        // Mouse stuff
        MouseState prevMouseState;

        // Max yaw/pitch variables
        float totalYaw = MathHelper.PiOver4 / 2;
        float currentYaw = 0;
        float totalPitch = MathHelper.PiOver4 / 2;
        float currentPitch = 0;

        public Vector3 GetCameraDirection
        {
            get { return cameraDirection; }
        }

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
            // Yaw rotation
            float yawAngle = (-MathHelper.PiOver4 / 150) *
                    (Mouse.GetState().X - prevMouseState.X);

            if (Math.Abs(currentYaw + yawAngle) < totalYaw)
            {
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(cameraUp, yawAngle));
                currentYaw += yawAngle;
            }

            // Pitch rotation
            float pitchAngle = (MathHelper.PiOver4 / 150) *
                (Mouse.GetState().Y - prevMouseState.Y);

            if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
            {
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(
                        Vector3.Cross(cameraUp, cameraDirection),
                    pitchAngle));

                currentPitch += pitchAngle;
            }

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