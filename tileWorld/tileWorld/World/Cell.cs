using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace tileWorld
{
    [Serializable]
    public class Cell
    {   
        public int TileID;
        public int TileEntityID = 0;
        public int TileTreeID = 0;


        public bool Collision = false;
        public bool Liquid = false;
        public bool Hurt = false;
        public int OccupiedCount = 0;
        public Vector2 tilePosition;
        public Vector2 pixelPosition;
        public Vector2 pixelPositionCenter;
        public string chunkID = "NA";
        public Color color = Color.White;
        public Color debugColor = Color.White;
        public int cost = 0;

        public int randomNumber;

        public Cell(int tileID)
        {
            TileID = tileID;
            TileEntityID = -1;
        }

        public Cell(int tileID, bool collision, bool liquid, bool hurt)
        {
            
            TileID = tileID;
            Collision= collision;
            Liquid =  liquid;
            Hurt = hurt;
            TileEntityID = -1;
        }

        public void enterCell()
        {
            OccupiedCount += 100;
        }
        public void exitCell()
        {
            OccupiedCount -= 100;
        }

        public void enterCellintoPath()
        {
            OccupiedCount += 10;
            debugColor = Color.RosyBrown;
        }
        public void exitCellintoPath()
        {
            OccupiedCount -= 10;
            debugColor = Color.White;
        }

   }
}
