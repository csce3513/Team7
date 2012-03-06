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

namespace Tile_Engine
{
    public static class TileMap
    {
        #region Declarations
        public const int TileWidth = 48;
        public const int TileHeight = 48;
        public const int MapWidth = 160;
        public const int MapHeight = 12;
        public const int MapLayers = 3;
        private const int skyTile = 2;

        static private MapSquare[,] mapCells =
    new MapSquare[MapWidth, MapHeight];
        static private Texture2D tileSheet;
        #endregion

        static public void Initialize(Texture2D tileTexture)
        {
            tileSheet = tileTexture;

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int z = 0; z < MapLayers; z++)
                    {
                        mapCells[x, y] = new MapSquare(skyTile, 0, 0, "", true);
                    }
                }
            }
        }

        public static int TilesPerRow
        {
            get { return tileSheet.Width / TileWidth; }
        }

        public static Rectangle TileSourceRectangle(int tileIndex)
        {
            return new Rectangle(
                (tileIndex % TilesPerRow) * TileWidth,
                (tileIndex / TilesPerRow) * TileHeight,
                TileWidth,
                TileHeight);
        }

        }
    


}
