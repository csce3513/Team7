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

namespace _3D_Madness
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game camera
        Camera camera;

        // Vertex data
        VertexPositionTexture[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;

        // Movement and rotation stuff
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        // Texture info
        List<Texture2D> textureList = new List<Texture2D>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize camera
            camera = new Camera(this, new Vector3(0, 0, 5),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //initialize vertices
            verts = new VertexPositionTexture[24];
            //FRONT
            verts[0] = new VertexPositionTexture(
                new Vector3(-1, 1, 1), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(
                new Vector3(1, 1, 1), new Vector2(1, 0));
            verts[2] = new VertexPositionTexture(
                new Vector3(-1, -1, 1), new Vector2(0, 1));
            verts[3] = new VertexPositionTexture(
                new Vector3(1, -1, 1), new Vector2(1, 1));

            //BACK
            verts[4] = new VertexPositionTexture(
                new Vector3(1, 1, -1), new Vector2(0, 0));
            verts[5] = new VertexPositionTexture(
                new Vector3(-1, 1, -1), new Vector2(1, 0));
            verts[6] = new VertexPositionTexture(
                new Vector3(1, -1, -1), new Vector2(0, 1));
            verts[7] = new VertexPositionTexture(
                new Vector3(-1, -1, -1), new Vector2(1, 1));

            //LEFT
            verts[8] = new VertexPositionTexture(
                new Vector3(-1, 1, -1), new Vector2(0, 0));
            verts[9] = new VertexPositionTexture(
                new Vector3(-1, 1, 1), new Vector2(1, 0));
            verts[10] = new VertexPositionTexture(
                new Vector3(-1, -1, -1), new Vector2(0, 1));
            verts[11] = new VertexPositionTexture(
                new Vector3(-1, -1, 1), new Vector2(1, 1));

            //RIGHT
            verts[12] = new VertexPositionTexture(
                new Vector3(1, 1, 1), new Vector2(0, 0));
            verts[13] = new VertexPositionTexture(
                new Vector3(1, 1, -1), new Vector2(1, 0));
            verts[14] = new VertexPositionTexture(
                new Vector3(1, -1, 1), new Vector2(0, 1));
            verts[15] = new VertexPositionTexture(
                new Vector3(1, -1, -1), new Vector2(1, 1));

            //TOP
            verts[16] = new VertexPositionTexture(
                new Vector3(-1, 1, -1), new Vector2(0, 0));
            verts[17] = new VertexPositionTexture(
                new Vector3(1, 1, -1), new Vector2(1, 0));
            verts[18] = new VertexPositionTexture(
                new Vector3(-1, 1, 1), new Vector2(0, 1));
            verts[19] = new VertexPositionTexture(
                new Vector3(1, 1, 1), new Vector2(1, 1));

            //BOTTOM
            verts[20] = new VertexPositionTexture(
                new Vector3(-1, -1, 1), new Vector2(0, 0));
            verts[21] = new VertexPositionTexture(
                new Vector3(1, -1, 1), new Vector2(1, 0));
            verts[22] = new VertexPositionTexture(
                new Vector3(-1, -1, -1), new Vector2(0, 1));
            verts[23] = new VertexPositionTexture(
                new Vector3(1, -1, -1), new Vector2(1, 1));


            // Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            // Initialize the BasicEffect
            effect = new BasicEffect(GraphicsDevice);

            //load all textures
            textureList.Add(Content.Load<Texture2D>(@"Textures\Trees"));
            textureList.Add(Content.Load<Texture2D>(@"Textures\t1"));
            textureList.Add(Content.Load<Texture2D>(@"Textures\t2"));
            textureList.Add(Content.Load<Texture2D>(@"Textures\t3"));
            textureList.Add(Content.Load<Texture2D>(@"Textures\t4"));
            textureList.Add(Content.Load<Texture2D>(@"Textures\t5"));
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Translation
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                worldTranslation *= Matrix.CreateTranslation(-.01f, 0, 0);
            if (keyboardState.IsKeyDown(Keys.Right))
                worldTranslation *= Matrix.CreateTranslation(.01f, 0, 0);

            // Rotation
            worldRotation *= Matrix.CreateFromYawPitchRoll(
                MathHelper.PiOver4 / 60,
                MathHelper.PiOver4 / 360,
                MathHelper.PiOver4 / 180);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            

            // Set the vertex buffer on the GraphicsDevice
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //Set object and camera info
            effect.World = worldRotation * worldTranslation * worldRotation;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.TextureEnabled = true;

            // Draw front
            effect.Texture = textureList[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 0, 2);

            }

            //draw back 
            effect.Texture = textureList[1];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 4, 2);

            }


            //Draw left
            effect.Texture = textureList[2];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 8, 2);

            }

            //draw right
            effect.Texture = textureList[3];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 12, 2);

            }

            //draw top
            effect.Texture = textureList[4];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 16, 2);

            }

            //draw bottom
            effect.Texture = textureList[5];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 20, 2);

            }


            base.Draw(gameTime);
        }
    }
}
