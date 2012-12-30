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


namespace tileWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        KeyboardState lastKeyState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;
        World gameWorld;
        NPC_Manager npcManager;
        Hud hud;
        InputHandler input;
        
        bool isplayerInNewChunk = false;
        public static bool debugMode = false;

        int ChunkSizeWidth = 64; //size of chunks in tiles
        int ChunkSizeHeight = 64; //size of chunks in tiles
        int TileSizeWidth = 32; //size of each tile in pixels
        int TileSizeHeight = 32; //size of each tile in pixels

        int screenResWidth = 1280;
        int screenResHeight = 720;

       

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferWidth = screenResWidth;
            graphics.PreferredBackBufferHeight = screenResHeight;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            Vector2 playerStartPos = new Vector2(43, 2);

            Camara.screenResWidth = screenResWidth;
            Camara.screenResHeight = screenResHeight;
            Camara.zoom = 1f;


            input = new InputHandler(this);
            //groundTiles = new tileSet(TileSizeWidth, TileSizeHeight, Content.Load<Texture2D>(@"tileSets/groundTiles"));
            gameWorld = new World(Content, "TheWorld", ChunkSizeWidth, ChunkSizeHeight, TileSizeWidth, TileSizeHeight, playerStartPos);
            player = new Player(Content, gameWorld);
            player.Position = playerStartPos;// gameWorld.playerStartTile();

            npcManager = new NPC_Manager(Content, gameWorld);

            hud = new Hud(Content, GraphicsDevice, player, gameWorld, npcManager);

            Camara.Location.X = (player.Position.X) - screenResWidth / 2;
            Camara.Location.Y = (player.Position.Y) - screenResHeight / 2;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
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
            input.Update(gameTime);
            KeyboardState keyState = Keyboard.GetState();
         
            if (input.keyBoardKeyHold(Keys.Q) & Game.debugMode)
            {
                Vector2 NPCPositon = new Vector2((int)(input.mousePos().X + Camara.Location.X), (int)(input.mousePos().Y + Camara.Location.Y));
                npcManager.GenNPC_atPos(NPCPositon);
            }
            if (input.keyBoardKeyPress(Keys.OemOpenBrackets))
            {
                Camara.zoom += .01f;
            }
            if (input.keyBoardKeyPress(Keys.OemCloseBrackets))
            {
                Camara.zoom -= .01f;
            }

            hud.Update(input); 
            player.Update(gameTime, npcManager, input);

            gameWorld.Update(gameTime, player.Position);
            npcManager.update(gameTime, player);

            MessageSystem.Instance.Update(gameTime);

            Camara.Location.X = (int)(player.Position.X) - GraphicsDevice.Viewport.Width / 2;
            Camara.Location.Y = (int)(player.Position.Y) - GraphicsDevice.Viewport.Height / 2;

            //check if in a new chunk
            //if yes add new cunks to world and clean up old ones
            isplayerInNewChunk = gameWorld.testIsNewChunk((int)player.Position.X, (int)player.Position.Y);

            lastKeyState = keyState; //save state to use to compare next update.
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camara.get_transformation(GraphicsDevice));
            
                gameWorld.Draw(spriteBatch);
                player.Draw(spriteBatch);
                npcManager.Draw(spriteBatch);
                MessageSystem.Instance.Draw(spriteBatch);
            
            spriteBatch.End();

            
            
            spriteBatch.Begin(); // DONT ZOOM!

                hud.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
