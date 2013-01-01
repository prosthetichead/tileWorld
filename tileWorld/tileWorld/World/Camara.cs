using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tileWorld
{
     static class Camara
    {
         private static int tileHeight = 32; 
         public static Vector2 Location = Vector2.Zero;
         public static int screenResWidth;
         public static int screenResHeight;
         private static Vector2 CenterCamaraLocation = Vector2.Zero;
         //public static int TileHight =32;
         //public static int TileWidth =32;

         public static float zoom = 1f; // Camera Zoom
         public static Matrix transform; // Matrix Transform

    
        public static void setLocation(Vector2 location)
        {
            Location = location;
        }

        public static Vector2 getLocation()
        {
            return Location;
        }


        public static Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            transform =       // Thanks to o KB o for this solution
            Matrix.CreateTranslation(new Vector3(-graphicsDevice.Viewport.Width / 2, -graphicsDevice.Viewport.Height/2, 0)) *
                                         Matrix.CreateRotationZ(0) *
                                         Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return transform;
        }

         public static float calculateDepth(Vector2 pos)
         {
             //int tileNumber = (int)(Y / tileHeight)+1;
             return (pos.Y + pos.X/1000) / 1000 ;

             
         }
    }
}
