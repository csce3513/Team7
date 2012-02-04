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


namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : DrawableGameComponent
    {
        // List of models
        List<BasicModel> models = new List<BasicModel>();

        // Spawn variables
        Vector3 maxSpawnLocation = new Vector3(100, 100, -3000);
        int nextSpawnTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.Pi / 40;

        // Enemy count
        int enemiesThisLevel = 0;

        // Misses variables
        int missedThisLevel = 0;

        // Current level
        int currentLevel = 0;

        // List of LevelInfo objects
        List<LevelInfo> levelInfoList = new List<LevelInfo>(  );

        // Shot stuff
        List<BasicModel> shots = new List<BasicModel>(  );
        float shotMinZ = -3000;

        public ModelManager(Game game)
            : base(game)
        {
            // Initialize game levels
            levelInfoList.Add(new LevelInfo(1000, 3000, 20, 2, 6, 10));
            levelInfoList.Add(new LevelInfo(900, 2800, 22, 2, 6, 9));
            levelInfoList.Add(new LevelInfo(800, 2600, 24, 2, 6, 8));
            levelInfoList.Add(new LevelInfo(700, 2400, 26, 3, 7, 7));
            levelInfoList.Add(new LevelInfo(600, 2200, 28, 3, 7, 6));
            levelInfoList.Add(new LevelInfo(500, 2000, 30, 3, 7, 5));
            levelInfoList.Add(new LevelInfo(400, 1800, 32, 4, 7, 4));
            levelInfoList.Add(new LevelInfo(300, 1600, 34, 4, 8, 3));
            levelInfoList.Add(new LevelInfo(200, 1400, 36, 5, 8, 2));
            levelInfoList.Add(new LevelInfo(100, 1200, 38, 5, 9, 1));
            levelInfoList.Add(new LevelInfo(50, 1000, 40, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 800, 42, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 600, 44, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(25, 400, 46, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(0, 200, 48, 18, 20, 0));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Set initial spawn time
            SetNextSpawnTime();

            base.Initialize();
        }

        protected override void LoadContent()
        {


            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            // Update models
            UpdateModels();

            // Update shots
            UpdateShots();

            base.Update(gameTime);
        }

        protected void UpdateModels()
        {
            // Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i)
            {
                // Update each model
                models[i].Update();

                // Remove models that are out of bounds
                if (models[i].GetWorld().Translation.Z >
                    ((Game1)Game).camera.cameraPosition.Z + 100)
                {
                    models.RemoveAt(i);
                    --i;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each shot
            foreach (BasicModel bm in shots)
            {
                bm.Draw(((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }

        private void SetNextSpawnTime()
        {
            // Reset the variables to indicate the next enemy spawn time
            nextSpawnTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }

        protected void CheckToSpawnEnemy(GameTime gameTime)
        {
            // Time to spawn a new enemy?
            if (enemiesThisLevel <
                levelInfoList[currentLevel].numberEnemies)
            {
                timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawn > nextSpawnTime)
                {
                    SpawnEnemy();
                }
            }
        }

        private void SpawnEnemy()
        {
            // Generate random position with random X and random Y
            // between -maxX and maxX and -maxY and maxY. Z is always
            // the same for all ships.
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X),
                ((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.Y, (int)maxSpawnLocation.Y),
                maxSpawnLocation.Z);

            // Direction will always be (0, 0, Z), where
            // Z is a random value between minSpeed and maxSpeed
            Vector3 direction = new Vector3(0, 0,
                ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpeed,
                levelInfoList[currentLevel].maxSpeed));

            // Get a random roll rotation between -maxRollAngle and maxRollAngle
            float rollRotation = (float)((Game1)Game).rnd.NextDouble() *
                    maxRollAngle - (maxRollAngle / 2);

            // Add model to the list
            models.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\spaceship"),
                position, direction, 0, 0, rollRotation));

            // Increment # of enemies this level and set next spawn time
            ++enemiesThisLevel;
            SetNextSpawnTime();
        }

        public void AddShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\ammo"),
                position, direction, 0, 0, 0));
        }

        protected void UpdateShots()
        {
            // Loop through shots
            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot
                shots[i].Update();

                // If shot is out of bounds, remove it from game
                if (shots[i].GetWorld().Translation.Z < shotMinZ)
                {
                    shots.RemoveAt(i);
                    --i;
                }
                else
                {
                    // If shot is still in play, check for collisions
                    for (int j = 0; j < models.Count; ++j)
                    {
                        if (shots[i].CollidesWith(models[j].model,
                            models[j].GetWorld()))
                        {
                            // Collision! Remove the ship and the shot.
                            models.RemoveAt(j);
                            shots.RemoveAt(i);
                            --i;
                            ((Game1)Game).PlayCue("Explosions");
                            break;
                        }
                    }
                }
            }
        }
    }
}
