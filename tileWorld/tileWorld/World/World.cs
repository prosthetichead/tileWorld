using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace tileWorld
{
    class World
    {
        string WorldName;
        int range = 3;
        int TileWidth;
        int TileHeight;
        int ChunkWidth;
        int ChunkHeight;
        IDictionary<string, Chunk> chunkDictonary = new Dictionary<string, Chunk>();


        private int currentChunkX;
        private int currentChunkY;

        private Vector2 PlayerPos;

        TileSet GroundTiles;
        TileSet TileEntites;
        SpriteFont fontTiny;
        ContentManager Content;
        
        public bool debug = false;

        
        public World(ContentManager content, string worldName, int chunkWidth, int chunkHeight, int tileWidth, int tileHeight, Vector2 playerPos)
        {
            
            Content = content;
            fontTiny = Content.Load<SpriteFont>(@"Fonts/Font-PF Arma Five");
            GroundTiles = new TileSet(content, tileWidth, tileHeight, "tileSets/groundTiles");
            TileEntites = new TileSet(content, tileWidth, tileHeight, "tileSets/TileEntites");
            //ItemTiles = new tileSet(content, tileWidth, tileHeight, "tileSets/itemTiles");

            WorldName = worldName;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            ChunkWidth = chunkWidth;
            ChunkHeight = chunkHeight;

            AddNewChunks((int)playerPos.X, (int)playerPos.Y);
        }

        public Vector2 getCenterOfTile(Vector2 position)
        {
            Vector2 returnVector2 = new Vector2((position.X / TileWidth), (position.Y/ TileHeight));
            return returnVector2;
        }


        public Chunk getChunk(int tileNumberX, int tileNumberY)
        {
            //chunks root position used for the chunk key
            int chunkX = getChunkX(tileNumberX);
            int chunkY = getChunkY(tileNumberY);
            //chunk key used as dictonary Key
            string chunkKey = chunkX + "," + chunkY;


            //is chunk in dictonary?
            if (chunkDictonary.ContainsKey(chunkKey)) //yes, get chunk
            {
                return chunkDictonary[chunkKey];
            }
            else //no? Return null
            {
                return null;// AddNewChunks(tileNumberX, tileNumberY);
            }
        }

        public Cell[] getCellArray(Cell cell)
        {

            Cell[] CellArray = new Cell[8];
            int X = (int)cell.tilePosition.X;
            int Y = (int)cell.tilePosition.Y;

            CellArray[0] = getCell(X + 1, Y);
            CellArray[1] = getCell(X - 1, Y);
            CellArray[2] = getCell(X, Y + 1);
            CellArray[3] = getCell(X, Y - 1);
            CellArray[4] = getCell(X + 1, Y + 1);
            CellArray[5] = getCell(X - 1, Y - 1);
            CellArray[6] = getCell(X + 1, Y - 1);
            CellArray[7] = getCell(X - 1, Y + 1);

            return CellArray;

        }


        public Cell getCellFromPixelPos(Vector2 position)
        {
            return getCell((int)(position.X / TileWidth), (int)(position.Y / TileWidth));
        }
        public Cell getCell(int X, int Y)
        {        
            int chunkX = getChunkX(X);
            int chunkY = getChunkY(Y);
            string chunkKey = chunkX + "," + chunkY;
            
            //position in the chunk
            int chunkPosX;
            chunkPosX = X % ChunkWidth;
            if (chunkPosX < 0)
                chunkPosX = (ChunkWidth) + chunkPosX;
            
            int chunkPosY;
            chunkPosY = Y % ChunkHeight;
            if (chunkPosY < 0)
                chunkPosY = (ChunkHeight) + chunkPosY;

            if (chunkDictonary.ContainsKey(chunkKey))
                return chunkDictonary[chunkKey].getBackgroundCell(chunkPosX, chunkPosY);
            else
                return new Cell(0); //return a 0 error cell
        }


        private void AddNewChunks(int x, int y)
        {

            int XStart = x - (ChunkWidth * range);
            int XEnd = x + (ChunkWidth * range);
            int YStart = y - (ChunkHeight * range);
            int YEnd = y + (ChunkHeight * range);


            int chunkX;
            int chunkY;
            string chunkKey;

           for (int i = 0; i < chunkDictonary.Count; i++)
           {
            chunkDictonary.ElementAt(i).Value.markedForDelete = true;
           }

           for (int X = XStart; X <= XEnd; X =X + ChunkWidth)
           {
             for (int Y = YStart; Y <= YEnd; Y = Y + ChunkHeight)
            {
                    chunkX = getChunkX(X);
                    chunkY = getChunkY(Y);
                    chunkKey = chunkX + "," + chunkY;



                    if (chunkDictonary.ContainsKey(chunkKey))
                    {
                        //update last used time chunk is already in the dictonary but is still needed
                        chunkDictonary[chunkKey].UpdateLastUsed(); //also unmarks the chunk for delete.
                    }
                    else
                    {
                        //create the chunk add the chunk to the dictonary
                        chunkDictonary.Add(chunkKey, new Chunk(ChunkWidth, ChunkHeight, chunkX, chunkY, WorldName));
                        chunkDictonary[chunkKey].initialize();
                    }
                }
            }   
                       
        }
        private void CleanUpChunks()
        {
            
            KeyValuePair<string, Chunk>[] ChunkDicArray = chunkDictonary.ToArray();
            for (int i = 0; i < ChunkDicArray.Count(); i++)
            {
                if (ChunkDicArray[i].Value.markedForDelete)
                {
                    //write any NPC currently inside the chunk to the NPCData list inside the chunk
                    //remove the NPC from the world
                    ChunkDicArray[i].Value.writeChunkToHDD();
                    chunkDictonary.Remove(ChunkDicArray[i].Key);
                }
            }
        }
        
        public Boolean testIsNewChunk(int x, int y)
        {
            x = x / TileWidth;
            y = y / TileHeight;

            int chunkX = getChunkX(x);
            int chunkY = getChunkY(y);
            
            if ((currentChunkX == chunkX) & (currentChunkY == chunkY))
            {
                //this is the same chunk still no need to update chunk dictonary!
                return false; 
            }
            else //we are in a new chunk
            {
                //Thread addChunksThread = new Thread(() => AddNewChunks(x, y));
                AddNewChunks(x, y);
                
                Thread thread = new Thread(CleanUpChunks);
                thread.Name = "Clean Up Chunks";
                thread.Start();
                currentChunkX = chunkX; //update current chunk root pos
                currentChunkY = chunkY;
                return true; 
            }  
        }


        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            PlayerPos = playerPos;
            // Check is the player aproching a new chunk? if yes start generating new chunk data
            // update every NPC
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            Vector2 firstSquare = new Vector2(Camara.Location.X / GroundTiles.TileWidth, Camara.Location.Y / GroundTiles.TileHeight);
            Vector2 squareOffset = new Vector2(Camara.Location.X % GroundTiles.TileWidth, Camara.Location.Y % GroundTiles.TileHeight);
            Vector2 origin = Vector2.Zero;
            int pixelPosX;
            int pixelPosY;
            int tilePosX;
            int tilePosY;
            Cell cell;
           
            
            for (int y = -10; y < 50; y++)
            {
                for (int x = -10; x < 50; x++)
                {

                    pixelPosX = (x * GroundTiles.TileWidth) - (int)squareOffset.X;
                    pixelPosY = (y * GroundTiles.TileHeight) - (int)squareOffset.Y;
                    tilePosX = x + (int)firstSquare.X;
                    tilePosY = y + (int)firstSquare.Y;
                    
                    
                   
                    
                    if ((cell = getCell(tilePosX, tilePosY)) == null)
                    {
                        cell = new Cell(0);
                    }

                    //Draw Cell
                    GroundTiles.draw(spriteBatch, new Vector2(pixelPosX, pixelPosY), origin, cell.TileID, cell.color);
                    if (cell.TileEntityID > 0)
                    {
                        TileEntites.draw(spriteBatch, new Vector2(pixelPosX, pixelPosY), origin, cell.TileEntityID, cell.color);
                    }
                    //spriteBatch.Draw(GroundTiles.TileSetTexture,
                    //        new Rectangle(pixelPosX, pixelPosY, GroundTiles.TileWidth, GroundTiles.TileHeight),
                    //        GroundTiles.getSourceRectangle(cell.TileID),
                    //        Color.White,0f, origin,SpriteEffects.None,0);


                    if (debug)
                    {
                        int Xpos = tilePosX - (int)(PlayerPos.X / TileWidth);
                        int Ypos = tilePosY- (int)(PlayerPos.Y/TileHeight);
                        if ((Math.Abs(tilePosX - (int)(PlayerPos.X / TileWidth)) < 5) & (Math.Abs(tilePosY - (int)(PlayerPos.Y / TileHeight)) < 5))
                        {

                            spriteBatch.DrawString(fontTiny, cell.tilePosition.X + ", " + cell.tilePosition.Y, new Vector2(pixelPosX + 1, pixelPosY), Color.White);
                            spriteBatch.DrawString(fontTiny, cell.chunkID, new Vector2(pixelPosX + 1, pixelPosY + 10), Color.White);
                        }
                    }       
                }// END X ForLoop
            } // END Y ForLoop
        
        }


        #region Get ChunkPos
        //finds the chuck pos using x y cords
        private int getChunkX(int x)
        {
            int test;
            int chunkX;
            test = x % ChunkWidth;
            if (test < 0)
                chunkX = (x - ChunkWidth) / ChunkWidth;
            else
                chunkX = x / ChunkWidth;


            return chunkX;
        }
        private int getChunkY(int y)
        {
            int test;
            int chunkY;
            test = y % ChunkHeight;
            if (test < 0)
                chunkY = (y - ChunkHeight) / ChunkHeight;
            else
                chunkY = y / ChunkHeight;

            return chunkY;
        }
        #endregion
    
    }
}