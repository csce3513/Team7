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

namespace Catch
{
    // Represents different states of the game
    public enum GameState
    {
        SignIn, FindSession,
        CreateSession, Start, InGame, GameOver
    }

    // Represents different types of network messages
    public enum MessageType
    {
        StartGame, EndGame, RestartGame,
        RejoinLobby, UpdatePlayerPos
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Fonts
        SpriteFont scoreFont;

        // Current game state
        GameState currentGameState = GameState.SignIn;

        // Audio variables
        SoundEffectInstance trackInstance;

        // Sprite speeds
        Vector2 chasingSpeed = new Vector2(4, 4);
        Vector2 chasedSpeed = new Vector2(6, 6);

        // Network stuff
        NetworkSession networkSession;
        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

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
            Components.Add(new GamerServicesComponent(this));

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

            // Load fonts
            scoreFont = Content.Load<SpriteFont>(@"fonts\ScoreFont");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Only run the Update code if the game is currently active.
            // This prevents the game from progressing while
            // gamer services windows are open.
            if (this.IsActive)
            {
                // Run different methods based on game state
                switch (currentGameState)
                {
                    case GameState.SignIn:
                        Update_SignIn();
                        break;
                    case GameState.FindSession:
                        Update_FindSession();
                        break;
                    case GameState.CreateSession:
                        Update_CreateSession();
                        break;
                    case GameState.Start:
                        Update_Start(gameTime);
                        break;
                    case GameState.InGame:
                        Update_InGame(gameTime);
                        break;
                    case GameState.GameOver:
                        Update_GameOver(gameTime);
                        break;
                }
            }

            // Update the network session and pump network messages
            if (networkSession != null)
                networkSession.Update();

            
            base.Update(gameTime);
        }


        protected void Update_SignIn()
        {
            // If no local gamers are signed in, show sign-in screen
            if (Gamer.SignedInGamers.Count < 1)
            {
                Guide.ShowSignIn(1, false);
            }
            else
            {
                // Local gamer signed in, move to find sessions
                currentGameState = GameState.FindSession;
            }
        }

        private void Update_FindSession()
        {
            // Find sesssions of the current game
            AvailableNetworkSessionCollection sessions =
                NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
            if (sessions.Count == 0)
            {
                // If no sessions exist, move to the CreateSession game state
                currentGameState = GameState.CreateSession;
            }
            else
            {
                // If a session does exist, join it, wire up events,
                // and move to the Start game state
                networkSession = NetworkSession.Join(sessions[0]);
                WireUpEvents();
                currentGameState = GameState.Start;
            }

        }

        protected void WireUpEvents()
        {
            // Wire up events for gamers joining and leaving
            networkSession.GamerJoined += GamerJoined;
            networkSession.GamerLeft += GamerLeft;
        }

        void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            // Gamer joined. Set the tag for the gamer to a new UserControlledSprite.
            // If the gamer is the host, create a chaser; if not, create a chased.
            if (e.Gamer.IsHost)
            {
                e.Gamer.Tag = CreateChasingSprite();
            }
            else
            {
                e.Gamer.Tag = CreateChasedSprite();
            }
        }

        private UserControlledSprite CreateChasedSprite()
        {
            // Create a new chased sprite
            // using the gears sprite sheet
            return new UserControlledSprite(
                    Content.Load<Texture2D>(@"Images/gears"),
                    new Vector2((Window.ClientBounds.Width / 2) + 150,
                        (Window.ClientBounds.Height / 2) + 150),
                    new Point(100, 100), 10, new Point(0, 0),
                    new Point(6, 8), chasedSpeed, false);
        }

        private UserControlledSprite CreateChasingSprite()
        {
            // Create a new chasing sprite
            // using the dynamite sprite sheet
            return new UserControlledSprite(
                    Content.Load<Texture2D>(@"Images/dynamite"),
                    new Vector2((Window.ClientBounds.Width / 2) - 150,
                        (Window.ClientBounds.Height / 2) - 150),
                    new Point(100, 100), 10, new Point(0, 0),
                    new Point(6, 8), chasingSpeed, true);
        }

        void GamerLeft(object sender, GamerLeftEventArgs e)
        {
            // Dispose of the network session, set it to null.
            // Stop the soundtrack and go
            // back to searching for sessions.
            networkSession.Dispose();
            networkSession = null;

            trackInstance.Stop();

            currentGameState = GameState.FindSession;
        }

        private void Update_CreateSession()
        {
            // Create a new session using SystemLink with a max of 1 local player
            // and a max of 2 total players
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = false;

            // Wire up events and move to the Start game state
            WireUpEvents();
            currentGameState = GameState.Start;
        }

        private void Update_Start(GameTime gameTime)
        {
            // Get local gamer
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];

            // Check for game start key or button press
            // only if there are two players
            if (networkSession.AllGamers.Count == 2)
            {
                // If space bar or Start button is pressed, begin the game
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start ==
                    ButtonState.Pressed)
                {
                    // Send message to other player that we're starting
                    packetWriter.Write((int)MessageType.StartGame);
                    localGamer.SendData(packetWriter, SendDataOptions.Reliable);

                    // Call StartGame
                    StartGame();
                }
            }

            // Process any incoming packets
            ProcessIncomingData(gameTime);
        }

        protected void StartGame()
        {
            // Set game state to InGame
            currentGameState = GameState.InGame;

            // Start the soundtrack audio
            SoundEffect se = Content.Load<SoundEffect>(@"audio\track");
            trackInstance = se.CreateInstance();
            trackInstance.IsLooped = true;
            trackInstance.Play();

            // Play the start sound
            se = Content.Load<SoundEffect>(@"audio\start");
            se.Play();
        }

        protected void ProcessIncomingData(GameTime gameTime)
        {
            // Process incoming data
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];

            // While there are packets to be read...
            while (localGamer.IsDataAvailable)
            {
                // Get the packet
                NetworkGamer sender;
                localGamer.ReceiveData(packetReader, out sender);

                // Ignore the packet if you sent it
                if (!sender.IsLocal)
                {
                    // Read messagetype from start of packet
                    // and call appropriate method
                    MessageType messageType = (MessageType)packetReader.ReadInt32();
                    switch (messageType)
                    {
                        case MessageType.EndGame:
                            EndGame();
                            break;
                        case MessageType.StartGame:
                            StartGame();
                            break;
                        case MessageType.RejoinLobby:
                            RejoinLobby();
                            break;
                        case MessageType.RestartGame:
                            RestartGame();
                            break;
                        case MessageType.UpdatePlayerPos:
                            UpdateRemotePlayer(gameTime);
                            break;
                    }
                }
            }
        }

        protected void EndGame()
        {
            // Play collision sound effect
            // (game ends when players collide)
            SoundEffect se = Content.Load<SoundEffect>(@"audio\boom");
            se.Play();

            // Stop the soundtrack music
            trackInstance.Stop();

            // Move to the game-over state
            currentGameState = GameState.GameOver;
        }

        private void RejoinLobby()
        {
            // Switch dynamite and gears sprites
            // as well as chaser vs. chased
            SwitchPlayersAndReset(false);
            currentGameState = GameState.Start;
        }

        private void RestartGame()
        {
            // Switch dynamite and gears sprites
            // as well as chaser vs. chased
            SwitchPlayersAndReset(true);
            StartGame();
        }

        private void SwitchPlayersAndReset(bool switchPlayers)
        {
            // Only do this if there are two players
            if (networkSession.AllGamers.Count == 2)
            {
                // Are we truly switching players or are we
                // setting the host as the chaser?
                if (switchPlayers)
                {
                    // Switch player sprites
                    if (((UserControlledSprite)networkSession.AllGamers[0].Tag).isChasing)
                    {
                        networkSession.AllGamers[0].Tag = CreateChasedSprite();
                        networkSession.AllGamers[1].Tag = CreateChasingSprite();
                    }
                    else
                    {
                        networkSession.AllGamers[0].Tag = CreateChasingSprite();
                        networkSession.AllGamers[1].Tag = CreateChasedSprite();
                    }
                }
                else
                {
                    // Switch player sprites
                    if (networkSession.AllGamers[0].IsHost)
                    {
                        networkSession.AllGamers[0].Tag = CreateChasingSprite();
                        networkSession.AllGamers[1].Tag = CreateChasedSprite();
                    }
                    else
                    {
                        networkSession.AllGamers[0].Tag = CreateChasedSprite();
                        networkSession.AllGamers[1].Tag = CreateChasingSprite();
                    }
                }

            }
        }

        protected void UpdateRemotePlayer(GameTime gameTime)
        {
            // Get the other (non-local) player
            NetworkGamer theOtherGuy = GetOtherPlayer();

            // Get the UserControlledSprite representing the other player
            UserControlledSprite theOtherSprite = ((UserControlledSprite)theOtherGuy.Tag);

            // Read in the new position of the other player
            Vector2 otherGuyPos = packetReader.ReadVector2();

            // If the sprite is being chased,
            // retrieve and set the score as well
            if (!theOtherSprite.isChasing)
            {
                int score = packetReader.ReadInt32();
                theOtherSprite.score = score;
            }

            // Set the position
            theOtherSprite.Position = otherGuyPos;

            // Update only the frame of the other sprite
            // (no need to update position because you just did!)
            theOtherSprite.Update(gameTime, Window.ClientBounds, false);
        }

        protected NetworkGamer GetOtherPlayer()
        {
            // Search through the list of players and find the
            // one that's remote
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (!gamer.IsLocal)
                {
                    return gamer;
                }
            }

            return null;
        }

        private void Update_InGame(GameTime gameTime)
        {
            // Update the local player
            UpdateLocalPlayer(gameTime);

            // Read any incoming data
            ProcessIncomingData(gameTime);


            // Only host checks for collisions
            if (networkSession.IsHost)
            {
                // Only check for collisions if there are two players
                if (networkSession.AllGamers.Count == 2)
                {
                    UserControlledSprite sprite1 =
                        (UserControlledSprite)networkSession.AllGamers[0].Tag;
                    UserControlledSprite sprite2 =
                        (UserControlledSprite)networkSession.AllGamers[1].Tag;

                    if (sprite1.collisionRect.Intersects(
                        sprite2.collisionRect))
                    {
                        // If the two players intersect, game over.
                        // Send a game-over message to the other player
                        // and call EndGame.
                        packetWriter.Write((int)MessageType.EndGame);
                        networkSession.LocalGamers[0].SendData(packetWriter,
                            SendDataOptions.Reliable);

                        EndGame();
                    }
                }
            }
        }

        protected void UpdateLocalPlayer(GameTime gameTime)
        {
            // Get local player
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];

            // Get the local player's sprite
            UserControlledSprite sprite = (UserControlledSprite)localGamer.Tag;

            // Call the sprite's Update method, which will process user input
            // for movement and update the animation frame
            sprite.Update(gameTime, Window.ClientBounds, true);

            // if this sprite is being chased, increment the score
            // (score is just the num milliseconds that the chased player
            // survived)
            if (!sprite.isChasing)
                sprite.score += gameTime.ElapsedGameTime.Milliseconds;

            // Send message to other player with message tag and
            // new position of sprite
            packetWriter.Write((int)MessageType.UpdatePlayerPos);
            packetWriter.Write(sprite.Position);

            // If this player is being chased, add the score to the message
            if (!sprite.isChasing)
                packetWriter.Write(sprite.score);

            // Send data to other player
            localGamer.SendData(packetWriter, SendDataOptions.InOrder);

        }

        private void Update_GameOver(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadSate = GamePad.GetState(PlayerIndex.One);

            // If player presses Enter or A button, restart game
            if (keyboardState.IsKeyDown(Keys.Enter) ||
                gamePadSate.Buttons.A == ButtonState.Pressed)
            {
                // Send restart game message
                packetWriter.Write((int)MessageType.RestartGame);
                networkSession.LocalGamers[0].SendData(packetWriter,
                    SendDataOptions.Reliable);

                RestartGame();
            }
            // If player presses Escape or B button, rejoin lobby
            if (keyboardState.IsKeyDown(Keys.Escape) ||
                gamePadSate.Buttons.B == ButtonState.Pressed)
            {
                // Send rejoin lobby message
                packetWriter.Write((int)MessageType.RejoinLobby);
                networkSession.LocalGamers[0].SendData(packetWriter,
                    SendDataOptions.Reliable);

                RejoinLobby();
            }

            // Read any incoming messages
            ProcessIncomingData(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Only draw when game is active
            if (this.IsActive)
            {
                // Based on the current game state,
                // call the appropriate method
                switch (currentGameState)
                {
                    case GameState.SignIn:
                    case GameState.FindSession:
                    case GameState.CreateSession:
                        GraphicsDevice.Clear(Color.DarkBlue);
                        break;

                    case GameState.Start:
                        DrawStartScreen();
                        break;


                    case GameState.InGame:
                        DrawInGameScreen(gameTime);
                        break;

                    case GameState.GameOver:
                        DrawGameOverScreen();
                        break;

                }

            }

            base.Draw(gameTime);
        }

        private void DrawStartScreen()
        {
            // Clear screen
            GraphicsDevice.Clear(Color.AliceBlue);

            // Draw text for intro splash screen
            spriteBatch.Begin();

            // Draw instructions
            string text = "The dynamite player chases the gears\n";
            text += networkSession.Host.Gamertag +
                " is the HOST and plays as dynamite first";
            spriteBatch.DrawString(scoreFont, text,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(text).X / 2),
                (Window.ClientBounds.Height / 2)
                - (scoreFont.MeasureString(text).Y / 2)),
                Color.SaddleBrown);

            // If both gamers are there, tell gamers to press space bar or Start to begin
            if (networkSession.AllGamers.Count == 2)
            {
                text = "(Game is ready. Press Spacebar or Start button to begin)";
                spriteBatch.DrawString(scoreFont, text,
                    new Vector2((Window.ClientBounds.Width / 2)
                    - (scoreFont.MeasureString(text).X / 2),
                    (Window.ClientBounds.Height / 2)
                    - (scoreFont.MeasureString(text).Y / 2) + 60),
                    Color.SaddleBrown);
            }
            // If only one player is there, tell gamer you're waiting for players
            else
            {
                text = "(Waiting for players)";
                spriteBatch.DrawString(scoreFont, text,
                    new Vector2((Window.ClientBounds.Width / 2)
                    - (scoreFont.MeasureString(text).X / 2),
                    (Window.ClientBounds.Height / 2) + 60),
                    Color.SaddleBrown);
            }

            // Loop through all gamers and get their gamertags,
            // then draw list of all gamers currently in the game
            text = "\n\nCurrent Player(s):";
            foreach (Gamer gamer in networkSession.AllGamers)
            {
                text += "\n" + gamer.Gamertag;
            }
            spriteBatch.DrawString(scoreFont, text,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(text).X / 2),
                (Window.ClientBounds.Height / 2) + 90),
                Color.SaddleBrown);

            spriteBatch.End();
        }

        private void DrawInGameScreen(GameTime gameTime)
        {
            // Clear device
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            // Loop through all gamers in session
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Pull out the sprite for each gamer and draw it
                UserControlledSprite sprite = ((UserControlledSprite)gamer.Tag);
                sprite.Draw(gameTime, spriteBatch);

                // If the sprite is being chased, draw the score for that sprite
                if (!sprite.isChasing)
                {
                    string text = "Score: " + sprite.score.ToString();
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2(10, 10),
                        Color.SaddleBrown);
                }
            }

            spriteBatch.End();
        }

        private void DrawGameOverScreen()
        {
            // Clear device
            GraphicsDevice.Clear(Color.Navy);

            spriteBatch.Begin();

            // Game over. Find the chased sprite and draw his score.
            string text = "Game Over\n";
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                UserControlledSprite sprite = ((UserControlledSprite)gamer.Tag);
                if (!sprite.isChasing)
                {
                    text += "Score: " + sprite.score.ToString();
                }
            }

            // Give players instructions from here
            text += "\nPress ENTER or A button to switch and play again";
            text += "\nPress ESCAPE or B button to exit to game lobby";

            spriteBatch.DrawString(scoreFont, text,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(text).X / 2),
                (Window.ClientBounds.Height / 2)
                - (scoreFont.MeasureString(text).Y / 2)),
                Color.WhiteSmoke);

            spriteBatch.End();
        }
    }
}
