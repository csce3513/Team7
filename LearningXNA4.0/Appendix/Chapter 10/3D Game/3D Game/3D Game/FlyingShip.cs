using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _3D_Game
{
    class FlyingShip : BasicModel
    {

        Matrix rotation = Matrix.CreateRotationY(MathHelper.Pi);
        Matrix translation = Matrix.Identity;

        float fMaxDistance = -400;
        Vector3 Direction = new Vector3(0, 0, -1);

        public FlyingShip(Model m)
            : base(m)
        {
        }

        public override void Update()
        {
            //if the object has traveled past the max distance
            //or in front of the origin, reverse direction
            //and rotate ship 180 degrees
            if (translation.Translation.Z < fMaxDistance ||
                translation.Translation.Z > 0)
            {
                Direction.Z *= -1;
                rotation *= Matrix.CreateRotationY(MathHelper.Pi);
            }

            translation *= Matrix.CreateTranslation(Direction);
        }

        public override Matrix GetWorld()
        {
            return world * rotation * translation;
        }
    }
}
