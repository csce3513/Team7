using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;

namespace PhoneAsteroids
{
    class PlayerModel : BasicModel
    {
        Vector3 translationVector = Vector3.Zero;

        // Scale
        float scale = .025f;

        // Accelerometer stuff
        Accelerometer accelerometer;
        Vector3 accelerometerData = new Vector3();
        int speed = 5;

        public PlayerModel(Model m)
            : base(m)
        {
            accelerometer = new Accelerometer();

            // Create an accelerometer event handler
            accelerometer.ReadingChanged +=
                new EventHandler<AccelerometerReadingEventArgs>(AccelerometerDataChanged);

            // Start the accelerometer
            accelerometer.Start();
        }

        public override void Update()
        {
            // Move the ship based on accelerometer input
            translationVector.Y += accelerometerData.X * speed;
            translationVector.X += -accelerometerData.Y * speed;


            base.Update();
        }

        override public Matrix GetWorld()
        {
            return world * Matrix.CreateScale(scale) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(GetTranslationVector());
        }

        override public Matrix GetWorldForBoundingSphere()
        {
            return world * Matrix.CreateScale(scale / 2) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(GetTranslationVector());
        }

        public Vector3 GetTranslationVector()
        {
            // Return current translation vector plus offset
            // to start player at far right of screen
            return new Vector3(50, 0, 0) + translationVector;
        }

        public void AccelerometerDataChanged(object sender, AccelerometerReadingEventArgs e)
        {
            // Store accelerometer data
            accelerometerData.X = (float)e.X;
            accelerometerData.Y = (float)e.Y;
        }
    }
}
