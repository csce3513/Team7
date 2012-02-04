using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace PhoneAsteroids
{
    class AsteroidModel : BasicModel
    {
        // Size of asteroids
        public enum AsteroidSize { SMALL, MEDIUM, LARGE }
        public AsteroidSize size;
        Vector3 scale;


        // Rotation stuff
        Vector3 rotationAxis = Vector3.Zero;
        Vector3 randomRotation;
        float maxRotationSpeed = MathHelper.Pi;

        // Misc 
        Random random;
        Vector3 direction;
        public Vector3 position;

        public AsteroidModel(Model m, Random random, Vector3 direction, AsteroidSize size, Vector3 position)
            : this(m, random, direction, size)
        {
            this.position = position;
        }

        public AsteroidModel(Model m, Random random, Vector3 direction, AsteroidSize size)
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

            this.size = size;

            // Initialize random scale
            if (size == AsteroidSize.LARGE)
            {
                scale = new Vector3(((float)random.NextDouble() * .005f) + .035f,
                    ((float)random.NextDouble() * .005f) + .035f,
                    ((float)random.NextDouble() * .005f) + .035f);
            }
            else if (size == AsteroidSize.MEDIUM)
            {
                scale = new Vector3(((float)random.NextDouble() * .005f) + .015f,
                    ((float)random.NextDouble() * .005f) + .015f,
                    ((float)random.NextDouble() * .005f) + .015f);
            }
            else
            {
                scale = new Vector3(((float)random.NextDouble() * .005f) + .005f,
                    ((float)random.NextDouble() * .005f) + .005f,
                    ((float)random.NextDouble() * .005f) + .005f);
            }

            // Initialize random speed
            this.direction = direction * ((float)random.NextDouble() + 1f);

            position = new Vector3(-300, ((float)random.NextDouble() * 100) - 50, 0);

        }



        public override void Update()
        {
            // Update rotation and position
            rotationAxis += randomRotation;
            position += direction;

            base.Update();
        }

        override public Matrix GetWorld()
        {
            return world * Matrix.CreateScale(scale) *
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationAxis.Y),
                MathHelper.ToRadians(rotationAxis.X), MathHelper.ToRadians(rotationAxis.Z)) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(position);
        }

        override public Matrix GetWorldForBoundingSphere()
        {
            // Make the bounding sphere smaller but rotate the same as the world is rotating
            return world * Matrix.CreateScale(scale / 2) *
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationAxis.Y),
                MathHelper.ToRadians(rotationAxis.X), MathHelper.ToRadians(rotationAxis.Z)) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(position);
        }
    }
}
