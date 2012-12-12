using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace tileWorld
{
    class TileSet
    {
        public Texture2D TileSetTexture;
        public int TileWidth;
        public int TileHeight;

        public TileSet(ContentManager content, int tileWidth, int tileHeight, string tileSetTexture)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TileSetTexture = content.Load<Texture2D>(tileSetTexture);
        }




       public  Rectangle getSourceRectangle(int ID)
        {
            int rectangleX = ID % (TileSetTexture.Width / TileWidth);
            int rectangleY = ID / (TileSetTexture.Width / TileWidth);

            return new Rectangle(rectangleX * TileWidth, rectangleY * TileHeight, TileWidth, TileHeight);
        }

        public void draw(SpriteBatch spriteBatch, Vector2 position, Vector2 origin, int tileID, Color color)
        {
            spriteBatch.Draw(TileSetTexture,
                    new Rectangle((int)position.X, (int)position.Y, TileWidth, TileHeight),
                    getSourceRectangle(tileID),
                    color,0f, origin,SpriteEffects.None,0);
        }

    }
}