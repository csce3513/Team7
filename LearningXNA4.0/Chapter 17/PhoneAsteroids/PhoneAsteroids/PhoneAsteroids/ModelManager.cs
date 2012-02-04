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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : DrawableGameComponent
    {
        // List of models
        List<AsteroidModel> asteroidList = new List<AsteroidModel>();
        List<ShotModel> shotList = new List<ShotModel>();
        PlayerModel player;

        // Spawning Asteroid stuff
        int timeSinceLastSpawn = 0;
        int nextSpawnTime = 0;
        int maxSpawnTime = 4000;
        int minSpawnTime = 2000;

        // Sounds
        SoundEffect trackSound;
        SoundEffect collisionSound;
        SoundEffectInstance trackSoundInstance;


        public ModelManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            SetNextSpawnTime();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Initialize player
            player = new PlayerModel(Game.Content.Load<Model>(@"models\p1_wedge"));

            // Load audio
            collisionSound = Game.Content.Load<SoundEffect>(@"audio\collision");
            trackSound = Game.Content.Load<SoundEffect>(@"audio\track");
            trackSoundInstance = trackSound.CreateInstance();
            trackSoundInstance.IsLooped = true;
            trackSoundInstance.Play();

            // Load all models
            Game.Content.Load<Model>(@"models\asteroid1");
            Game.Content.Load<Model>(@"models\asteroid2");
            Game.Content.Load<Model>(@"models\ammo");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Time to spawn?
            timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastSpawn > nextSpawnTime)
            {
                if (((Game1)Game).random.Next(2) == 0)
                    asteroidList.Add(new AsteroidModel(Game.Content.Load<Model>(@"models\asteroid1"), ((Game1)Game).random, new Vector3(1, 0, 0), AsteroidModel.AsteroidSize.LARGE));
                else
                    asteroidList.Add(new AsteroidModel(Game.Content.Load<Model>(@"models\asteroid2"), ((Game1)Game).random, new Vector3(1, 0, 0), AsteroidModel.AsteroidSize.LARGE));
                SetNextSpawnTime();
            }


            // Update player
            player.Update();

            // Loop through all shots and call Update
            for (int i = 0; i < shotList.Count; ++i)
            {
                shotList[i].Update();
                if (shotList[i].translationVector.X < -300)
                {
                    shotList.RemoveAt(i);
                    --i;
                }
            }

            // Loop through all models and call Update
            for (int i = 0; i < asteroidList.Count; ++i)
            {
                asteroidList[i].Update();

                if (Collision(player, asteroidList[i]))
                {
                    // Game over
                    Game.Exit();
                    --i;
                    break;
                }

                // Check for asteroid out of bounds
                if (Math.Abs(asteroidList[i].position.X) > 300 ||
                    Math.Abs(asteroidList[i].position.Y) > 300)
                {
                    asteroidList.RemoveAt(i);
                    --i;
                    break;
                }

                // Check for shots hitting asteroids
                for (int j = 0; j < shotList.Count; ++j)
                {
                    if (Collision(shotList[j], asteroidList[i]))
                    {
                        SpawnCollisionAsteroids(asteroidList[i].position, asteroidList[i].size);
                        collisionSound.Play();
                        shotList.RemoveAt(j);
                        asteroidList.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }


            base.Update(gameTime);
        }

        private void SpawnCollisionAsteroids(Vector3 position, AsteroidModel.AsteroidSize asteroidSize)
        {
            if (asteroidSize != AsteroidModel.AsteroidSize.SMALL)
            {
                // Spawn 2-4 smaller asteroids if collision was with a large or medium asteroid
                int numberToSpawn = ((Game1)Game).random.Next(2, 5);
                for (int i = 0; i <= numberToSpawn; ++i)
                {
                    if (((Game1)Game).random.Next(2) == 0)
                        asteroidList.Add(new AsteroidModel(Game.Content.Load<Model>(@"models\asteroid1"),
                            ((Game1)Game).random,
                            new Vector3(
                                ((float)((Game1)Game).random.NextDouble() * 2) - 1,
                                ((float)((Game1)Game).random.NextDouble() * 2) - 1, 0),
                            asteroidSize - 1, position));
                    else
                        asteroidList.Add(new AsteroidModel(Game.Content.Load<Model>(@"models\asteroid2"),
                            ((Game1)Game).random,
                            new Vector3(
                                ((float)((Game1)Game).random.NextDouble() * 2) - 1,
                                ((float)((Game1)Game).random.NextDouble() * 2) - 1, 0),
                            asteroidSize - 1, position));
                }
            }
        }

        private bool Collision(BasicModel playerModel, BasicModel asteroidModel)
        {
            // Check two models for collision using their bounding spheres
            foreach (ModelMesh playerMesh in playerModel.model.Meshes)
            {
                foreach (ModelMesh asteroidMesh in asteroidModel.model.Meshes)
                {
                    if (asteroidMesh.BoundingSphere.Transform(asteroidModel.GetWorldForBoundingSphere()).
                        Intersects(playerMesh.BoundingSphere.Transform(playerModel.GetWorldForBoundingSphere())))
                        return true;
                }
            }
            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw player
            player.Draw(((Game1)Game).camera);

            // Loop through and draw each asteroid
            foreach (AsteroidModel asteroid in asteroidList)
            {
                asteroid.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each shot
            foreach (ShotModel shot in shotList)
            {
                shot.Draw(((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }

        public void SetNextSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).random.Next(minSpawnTime, maxSpawnTime);
            timeSinceLastSpawn = 0;
        }

        internal void FireShot()
        {
            shotList.Add(new ShotModel(Game.Content.Load<Model>(@"models\ammo"),
                ((Game1)Game).random, player.GetTranslationVector()));

        }

    }
}
