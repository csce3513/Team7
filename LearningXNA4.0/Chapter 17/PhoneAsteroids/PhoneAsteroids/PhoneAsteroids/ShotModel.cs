using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;


namespace PhoneAsteroids
{
    class ShotModel : BasicModel
    {
        // Position and rotation
        public Vector3 translationVector;
        Vector3 rotationAxis = Vector3.Zero;
        Vector3 randomRotation;
        float maxRotationSpeed = MathHelper.Pi;

        // Speed, scale and random
        Vector3 speed = new Vector3(-10, 0, 0);
        Vector3 scale = new Vector3(3, 3, 3);
        Random random;

        // Regular asteroid constructor
        public ShotModel(Model m, Random random, Vector3 initialPosition)
            : base(m)
        {
            this.random = random;

            // Initialize random rotation speed
            randomRotation = new Vector3(
                (float)random.NextDouble() *
                maxRotationSpeed - (maxRotationSpeed / 2),
                (float)random.NextDouble() *
                maxRotationSpeed - (maxRotationSpeed / 2),
                (float)random.NextDouble() *
                maxRotationSpeed - (maxRotationSpeed / 2));

            // Initialize position
            translationVector = new Vector3(initialPosition.X - 25, initialPosition.Y, initialPosition.Z);

        }



        public override void Update()
        {
            rotationAxis += randomRotation;
            translationVector += speed;

            base.Update();
        }

        override public Matrix GetWorld()
        {
            return world * Matrix.CreateScale(scale) *
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationAxis.Y),
                    MathHelper.ToRadians(rotationAxis.X), MathHelper.ToRadians(rotationAxis.Z)) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(translationVector);
        }

        public Matrix GetBoundingSphereWorld()
        {
            return world * Matrix.CreateScale(scale / 4f) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(new Vector3(50, 0, 0)) *
                Matrix.CreateTranslation(translationVector);
        }

        override public Matrix GetWorldForBoundingSphere()
        {
            return world * Matrix.CreateScale(scale / 2) *
                Matrix.CreateTranslation(translationVector);
        }
    }
}
