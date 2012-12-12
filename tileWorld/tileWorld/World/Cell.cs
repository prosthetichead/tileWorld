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
        public int TileEntityID;
        public bool Collision = false;
        public bool Liquid = false;
        public bool Hurt = false;
        public bool Occupied = false;
        public Vector2 tilePosition;
        public Vector2 pixelPosition;
        public string chunkID = "NA";
        public Color color = Color.White;

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

        
   }
}
