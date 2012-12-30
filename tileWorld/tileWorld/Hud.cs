using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace tileWorld
{
    class Hud
    {
        private Texture2D curser;

        private Texture2D smallPlayerInfoBG;
        private Rectangle smallPlayerInfoBG_R;
        private Texture2D largePlayerInfoBG;
        private Rectangle largePlayerInfoBG_R;
        private Texture2D debugBG;
        private Rectangle debugBG_R;

        private bool displayBigPlayerInfo = false;
        private bool hoverSmallPlayerInfo = false;
       // public bool debugMode = false;

        SpriteFont smallFont;
        

        GraphicsDevice graphicsDevice;
        
        Player player;
        World world;
        NPC_Manager npcManager;

        Vector2 curserPos;
        Color curserColor;

        public Hud(ContentManager Content, GraphicsDevice graphicsDevice, Player player, World world, NPC_Manager npcManager)
        {
            this.graphicsDevice = graphicsDevice;
            this.player = player;
            this.world = world;
            this.npcManager = npcManager;

            smallFont = Content.Load<SpriteFont>(@"Fonts/Font-PF Ronda Seven");

            debugBG = Content.Load<Texture2D>(@"debugRec");
            debugBG_R = new Rectangle(10, 10, 300, 150);

            curser = Content.Load<Texture2D>("HUD/curser");

            smallPlayerInfoBG = Content.Load<Texture2D>("HUD/smallPlayerInfoBG");
            smallPlayerInfoBG_R = new Rectangle(5, graphicsDevice.Viewport.Height - smallPlayerInfoBG.Height - 5, smallPlayerInfoBG.Width, smallPlayerInfoBG.Height);

            largePlayerInfoBG = Content.Load<Texture2D>("HUD/largePlayerInfoBG");
            largePlayerInfoBG_R = new Rectangle(5, graphicsDevice.Viewport.Height - smallPlayerInfoBG.Height - 10 - largePlayerInfoBG.Height, largePlayerInfoBG.Width, largePlayerInfoBG.Height);
        }

        public void Update(InputHandler input)
        {
            
            curserPos = input.mousePos();
            Vector2 curserWorldPos = curserPos + Camara.Location;
            hoverSmallPlayerInfo = false;
            
            NPC npc = npcManager.getNPCatPos(curserWorldPos);
            if (npc != null)
                curserColor = Color.OrangeRed;
            else
                curserColor = Color.BlueViolet;
            
            //hover on SmallPlayerInfo
            if (smallPlayerInfoBG_R.Contains((int)input.mousePos().X, (int)input.mousePos().Y))
            {
                hoverSmallPlayerInfo = true;
                if (input.mouseLeftClick()) //click on small player info
                    displayBigPlayerInfo = !displayBigPlayerInfo;
            }


            //keyboard shortcuts
            if (input.keyBoardKeyPress(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                Game.debugMode = !Game.debugMode;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(smallPlayerInfoBG, smallPlayerInfoBG_R, Color.White);
            if (hoverSmallPlayerInfo)
                spriteBatch.DrawString(smallFont, " "+ player.playerHP(), new Vector2(smallPlayerInfoBG_R.Location.X+10,smallPlayerInfoBG_R.Location.Y), Color.White);
            if(displayBigPlayerInfo)
                spriteBatch.Draw(largePlayerInfoBG, largePlayerInfoBG_R, Color.White);
            if (Game.debugMode)
            {
                spriteBatch.Draw(debugBG, debugBG_R, Color.DarkBlue);

                spriteBatch.DrawString(smallFont, "Camera Pixel Pos X , Y : " + Camara.Location.X + " , " + Camara.Location.Y, new Vector2(30, 30), Color.White);
                spriteBatch.DrawString(smallFont, "Camera Tile Pos X , Y : " + (int)Camara.Location.X / world.TileWidth + " , " + (int)Camara.Location.Y / world.TileHeight, new Vector2(30, 45), Color.White);

                spriteBatch.DrawString(smallFont, "Player Pixel Pos X , Y : " + (int)player.Position.X + " , " + (int)player.Position.Y, new Vector2(30, 75), Color.White);
                spriteBatch.DrawString(smallFont, "Player Tile Pos : " + (int)(player.Position.X / world.TileWidth) + " , " + (int)(player.Position.Y / world.TileHeight), new Vector2(30, 85), Color.White);
                spriteBatch.DrawString(smallFont, "Player Standing Tile ID : " + world.getCellFromPixelPos(player.Position).TileID, new Vector2(30, 95), Color.White);
                spriteBatch.DrawString(smallFont, "Player Chunk ID : " + world.getCellFromPixelPos(player.Position).chunkID, new Vector2(30, 105), Color.White);

                spriteBatch.DrawString(smallFont, "NPC Count : " + npcManager.npcCount(), new Vector2(30, 125), Color.White);

            }
            spriteBatch.Draw(curser, curserPos, curserColor);


        }
    }
}
