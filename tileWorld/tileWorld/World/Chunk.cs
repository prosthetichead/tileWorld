using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace tileWorld
{
    [Serializable]
    public struct ChunkData
    {
        public DateTime TimeLastUsed; // last time the chunk was needed
        public DateTime TimeCreated;
        public int seed;
        public string WorldName;
        public int ChunkRootPosX;
        public int ChunkRootPosY;
        public int ChunkTilesWidth;
        public int ChunkTilesHeight;
        public Cell[,] ChunkBackGroundLayer;
         //public Object[,] ChunkObjects;
        public List<NPCData> npcDataList; // list of NPC DATA for saving.
        
    }

    class Chunk
    {

        private ChunkData chunkData;
        public bool markedForDelete;
        private ContentManager Content;
     
        public Chunk(ContentManager content, int chunkTilesWidth, int chunkTilesHeight, int chunkRootPosX, int chunkRootPosY, string worldName)
        {
            Content = content;
            chunkData.WorldName = worldName;
            chunkData.seed = 14;
            Noise2.SetSeed(chunkData.seed);

            chunkData.TimeLastUsed = DateTime.Now;
            chunkData.TimeCreated = DateTime.Now;

            markedForDelete = false;

            chunkData.ChunkTilesWidth = chunkTilesWidth;
            chunkData.ChunkTilesHeight = chunkTilesHeight;
            chunkData.ChunkRootPosX = chunkRootPosX;
            chunkData.ChunkRootPosY = chunkRootPosY;
            
            chunkData.ChunkBackGroundLayer = new Cell[chunkData.ChunkTilesWidth, chunkData.ChunkTilesHeight];
        }

        public void initialize()
        {
            
            if (File.Exists(chunkData.ChunkRootPosX + "," + chunkData.ChunkRootPosY + ".bin"))
            {
                readChunkFromHDD();
            }
            else
            {
                createBackgroundLayer();
            }
        }


        #region createBAckgroundLayer

        private float getNoise(float factor, int x, int y, int z)
        {
            return (float)Noise2.Noise(2 * x * factor, 2 * y * factor, z * factor) + (float)Noise2.Noise(4 * x * factor, 4 * y * factor, z * factor) + (float)Noise2.Noise(8 * x * factor, 8 * y * factor, z * factor);
                       
        }

        /// <summary>
        /// Reads in chunk file from text file or if file dosnt exists creates a new chunk
        /// creates the main background layer this layer must be created before others as it determains were things will spawn and be placed
        /// </summary>
        /// <returns></returns>
        private void createBackgroundLayer()
        {
            Random r = new Random(14);
            string chunkFileName = chunkData.WorldName + "\\" + chunkData.ChunkRootPosX + "," + chunkData.ChunkRootPosY + ".chunk";

            
                float noise;
                float leftNoise;
                float rightNoise;
                float topNoise;
                float bottomNoise;
                float leftTopNoise;
                float rightTopNoise;
                float leftBottomNoise;
                float rightBottomNoise;

                float sand = .03f;
                float water = 0f;
                float grass = 2; //it shouldnt EVER return more then 1, but it does..

                float factor =.001f;
            
                int itX = 0;
                int itY = 0;
                for (int y = chunkData.ChunkTilesHeight * chunkData.ChunkRootPosY; y < (chunkData.ChunkTilesHeight * chunkData.ChunkRootPosY) + chunkData.ChunkTilesHeight; y++)
                {
                    for (int x = chunkData.ChunkTilesWidth * chunkData.ChunkRootPosX; x < ((chunkData.ChunkTilesWidth * chunkData.ChunkRootPosX) + chunkData.ChunkTilesWidth); x++)
                    {
                        noise = getNoise(factor, x, y, 100);
                        leftNoise = getNoise(factor, x-1, y, 100);
                        rightNoise = getNoise(factor, x+1, y, 100);
                        topNoise = getNoise(factor, x, y-1, 100);
                        bottomNoise = getNoise(factor, x, y+1, 100);
                        leftTopNoise = getNoise(factor, x - 1, y - 1, 100);
                        rightTopNoise = getNoise(factor, x+1, y-1, 100);
                        leftBottomNoise = getNoise(factor, x-1, y+1, 100);
                        rightBottomNoise = getNoise(factor, x+1, y+1, 100);
                        
                        if (noise <= water)//Water Cell
                        {
                            chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(5, false,true, false); //is a liquid
                        }
                        else if (noise <= sand)//Sand Cell
                        {
                            chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(2, false, false, false); //Normal Sand!


                            if (leftNoise <= water) //left tile is water, place a left water to right sand tile
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(80, false, false, false);
                            if (rightNoise <= water) //right tile is water, place a right water to left sand tile
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(82, false, false, false);
                            if (topNoise <= water) //top tile water, place a top water to bottom sand tile
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(61, false, false, false);
                            if (bottomNoise <= water) //bottom tile water, place a bottom water to top sand tile
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(101, false, false, false);

                            //clean up for Top Left
                            if ((leftNoise >= water) & (topNoise >= water) & (leftTopNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(42, false, false, false);
                            if ((leftNoise <= water) & (topNoise <= water) & (leftTopNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(60, false, false, false);

                            //clean up for Top Right
                            if ((rightNoise >= water) & (topNoise >= water) & (rightTopNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(41, false, false, false);
                            if ((rightNoise <= water) & (topNoise <= water) & (rightTopNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(62, false, false, false);

                            //clean up for Bottom Left
                            if ((leftNoise >= water) & (bottomNoise >= water) & (leftBottomNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(22, false, false, false);
                            if ((leftNoise <= water) & (bottomNoise <= water) & (leftBottomNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(100, false, false, false);
                            //clean up for Bottom right
                            if ((rightNoise >= water) & (bottomNoise >= water) & (rightBottomNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(21, false, false, false);
                            if ((rightNoise <= water) & (bottomNoise <= water) & (rightBottomNoise <= water))
                                chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(102, false, false, false);


                      }
                      else if (noise <= grass)//Grass cell
                      {
                          chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(1, false, false, false);
                          

                          if (leftNoise >= water & leftNoise <= sand) //left tile is sand
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(85, false, false, false);
                          if (rightNoise >= water & rightNoise <= sand) //right tile is sand
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(83, false, false, false);
                          if (topNoise >= water & topNoise <= sand) //top tile sand
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(104, false, false, false);
                          if (bottomNoise >= water & bottomNoise <= sand) //top tile sand
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(64, false, false, false);


                          // top left cleanup
                          if ((leftNoise >= water & leftNoise <= sand) & (topNoise >= water & topNoise <= sand) & (leftTopNoise >= water & leftTopNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(24, false, false, false);
                          if ((leftNoise >= sand) & (topNoise >= sand) & (leftTopNoise >= water & leftTopNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(105, false, false, false);

                          //Top Right Cleanup
                          if ((rightNoise >= water & rightNoise <= sand) & (topNoise >= water & topNoise <= sand) & (rightTopNoise >= water & rightTopNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(25, false, false, false);
                          if ((rightNoise >= sand) & (topNoise >= sand) & (rightTopNoise >= water & rightTopNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(103, false, false, false);

                          //Bottom Left Cleanup
                          if ((leftNoise >= water & leftNoise <= sand) & (bottomNoise >= water & bottomNoise <= sand) & (leftBottomNoise >= water & leftBottomNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(44, false, false, false);
                          if ((leftNoise >= sand) & (bottomNoise >= sand) & (leftBottomNoise >= water & leftBottomNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(65, false, false, false);

                          //Bottom Right Cleanup
                          if ((rightNoise >= water & rightNoise <= sand) & (bottomNoise >= water & bottomNoise <= sand) & (rightBottomNoise >= water & rightBottomNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(45, false, false, false);
                          if ((rightNoise >= sand) & (bottomNoise >= sand) & (rightBottomNoise >= water & rightBottomNoise <= sand))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(63, false, false, false);




                          if ((leftNoise <= water) & (topNoise <= water) & (leftTopNoise <= water))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(48, false, false, false);
                          if ((leftNoise >= water) & (topNoise >= water) & (leftTopNoise <= water))
                              chunkData.ChunkBackGroundLayer[itX, itY] = new Cell(44, false, false, false);
                      }

                        
                        //System.Console.WriteLine(ChunkRootPosX + ", " + ChunkRootPosY + " " + x + ", " + y);
                        chunkData.ChunkBackGroundLayer[itX, itY].tilePosition = new Vector2(x, y);
                        chunkData.ChunkBackGroundLayer[itX, itY].chunkID = chunkData.ChunkRootPosX + ", " + chunkData.ChunkRootPosY;


                        if (r.Next(1, 100) < 20 & !chunkData.ChunkBackGroundLayer[itX, itY].Liquid)
                        {
                            chunkData.ChunkBackGroundLayer[itX, itY].TileEntityID = 2;
                            chunkData.ChunkBackGroundLayer[itX, itY].Collision = true;
                        }
                        ++itX;
                    }
                    itX = 0;
                    ++itY;
                }
            }
        #endregion

        private void createRocks()
        {
            // Rocks can only be on grass

        }

        public Cell getBackgroundCell(int x, int y)
        {
            Cell cell;
            if (x >= chunkData.ChunkTilesWidth || y >= chunkData.ChunkTilesHeight)
                cell = new Cell(0);
            else
                cell = chunkData.ChunkBackGroundLayer[x, y];
             return cell;
        }

        public void setBackgroundCell(int x, int y, Cell newCell)
        {
            chunkData.ChunkBackGroundLayer[x, y] = newCell;
        }

        public void UpdateLastUsed()
        {
            chunkData.TimeLastUsed = DateTime.Now;
            markedForDelete = false;
        }

        public DateTime getLastUsed()
        {
            return chunkData.TimeLastUsed;
        }

        public void writeChunkToHDD()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("World\\" + chunkData.ChunkRootPosX + ","+chunkData.ChunkRootPosY+ ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, chunkData);
            stream.Close();
        }
        private void readChunkFromHDD()
            {
            //Open the file written above and read values from it.
            Stream stream = File.Open("World\\" + chunkData.ChunkRootPosX + "," + chunkData.ChunkRootPosY + ".bin", FileMode.Open);
            IFormatter formatter = new BinaryFormatter();
            chunkData = (ChunkData)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}

