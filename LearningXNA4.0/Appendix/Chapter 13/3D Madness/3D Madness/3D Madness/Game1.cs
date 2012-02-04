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
        Effect normalEffect;
        Effect blurEffect;
        Effect negativeEffect;
        Effect grayscaleEffect;

        // Movement and rotation stuff
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        // Texture info
        Texture2D texture;

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

            //Load effect
            normalEffect = Content.Load<Effect>(@"effects\Red");
            grayscaleEffect = Content.Load<Effect>(@"effects\Grayscale");
            negativeEffect = Content.Load<Effect>(@"effects\Negative");
            blurEffect = Content.Load<Effect>(@"effects\Blur");

            // Load texture
            texture = Content.Load<Texture2D>(@"Textures\trees");
           
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
                0, 
                0);

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

            //Draw front
            DrawVerts(normalEffect, 0, 2);
            //Draw back
            DrawVerts(blurEffect, 4, 2);
            //Draw left
            DrawVerts(grayscaleEffect, 8, 2);
            //Draw right
            DrawVerts(negativeEffect, 12, 2);
            //Draw top
            DrawVerts(blurEffect, 16, 2);
            //Draw bottom
            DrawVerts(grayscaleEffect, 20, 2);

            base.Draw(gameTime);
        }

        protected void DrawVerts(Effect effect, int start, int end)
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            Matrix world = worldRotation * worldTranslation;
            effect.Parameters["xWorldViewProjection"].SetValue(
                world * camera.view * camera.projection);
            effect.Parameters["xColoredTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, start, end);
            }

        }
    }
}
