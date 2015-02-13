using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CanasUvighi
{
    public class Map
    {
        private int id;
        private string name;
        private FlatArray<Tile> mapTiles;
        private GameData gameData;

        #region Constructors
        /// <summary>
        /// Create a new empty Map and fill it with random Terrain/s. (GEN MAP)
        /// </summary>
        /// <param name="id">ID of the Map.</param>
        /// <param name="name">Name of the Map.</param>
        /// <param name="tiles">Flattened 2D array of Tiles, 
        /// containing the actual Map elements / layers.</param>
        /// <param name="gameData">GameData to use.</param>
        /// <param name="terrainIDs">Randomly fill the Map with given 
        /// Terrain IDs.</param>
        public Map(
            int id,
            string name,
            FlatArray<Tile> tiles,
            GameData gameData,
            int[] terrainIDs)
        {
            this.id = id;
            // should check name for invalid characters! [a-Z0-9]?
            this.name = name;
            this.mapTiles = tiles;
            this.gameData = gameData;

            if (terrainIDs != null)
            {
                // Fill Map with random Terrain IDs
                FillMap(terrainIDs);
            }
        }

        /// <summary>
        /// Create a Map with the specified Tiles. (CREATE MAP)
        /// </summary>
        /// <param name="id">ID of the Map.</param>
        /// <param name="tiles">Flattened 2D array of Tiles, 
        /// containing the actual Map elements / layers.</param>
        /// <param name="gameData">GameData to use.</param>
        public Map(int id, string name, FlatArray<Tile> tiles, GameData gameData)
            :this(id, name, tiles, gameData, null)
        { }
        #endregion

        #region Properties
        public string Name
        {
            get { return this.name; }
        }

        public FlatArray<Tile> Tiles
        {
            get { return this.mapTiles; }
            set { this.mapTiles = value; }
        }

        public Tile this[Point index]
        {
            get { return this.mapTiles[index.X, index.Y]; }
            set { this.mapTiles[index.X, index.y] = value; }
        }

        public int Height
        {
            get { return this.mapTiles.X; }
        }

        public int Width
        {
            get { return this.mapTiles.Y; }
        }

        public int ID
        {
            get { return this.id; }
        }
        #endregion

        public void Draw(SpriteBatch spriteBatch, Point center)
        {

        }

        /// <summary>
        /// Get the visual string of the highest priority Tile Layer.
        /// </summary>
        /// <param name="x">X axis of the Tile.</param>
        /// <param name="y">Y axis of the Tile.</param>
        /// <returns>The visual string of the highest priority element.</returns>
        public string TileVisual(Point point)
        {
            TileLayers layer = this[point].Layers;

            // if the Tile is not visible, return whitespace
            if (!this[point].IsVisible)
                return " ";

            if (layer.Unit != 0)
            {
                return gameData.UnitList[layer.Unit - 1].Visual;
            }
            if (layer.Container != 0)
            {
                // todo:implement!
                return "o";
            }
            if (layer.GameObj != 0)
            {
                // todo:implement!
                return "*";
            }
            else
            {
                // if none of the above exist in the Tile, return the Terrain
                return gameData.TerrainList[layer.Terrain].Visual;
            }
        }

        /// <summary>
        /// Get the Terrain object at the specified Tile.
        /// </summary>
        /// <param name="x">X axis of the Tile.</param>
        /// <param name="y">Y axis of the Tile.</param>
        /// <returns>Terrain object.</returns>
        public Terrain GetTerrain(Point point)
        {
            return gameData.TerrainList[this[point].Terrain];
        }

        /// <summary>
        /// Check if the Tile at the given coordinates is valid/free.
        /// </summary>
        /// <param name="x">X axis of the Tile.</param>
        /// <param name="y">Y axis of the Tile.</param>
        /// <returns>True for valid/free, False for invalid/blocked.</returns>
        public bool CheckTile(Point point)
        {
            // Check if the coordinates are inside the Map bounds.
            if (point.X < 0 || point.X >= this.Height ||
                point.Y < 0 || point.Y >= this.Width)
            {
                return false;
            }

            if (this[point].Unit != 0)
                return false;

            // If the Terrain is blocking the given Tile return false,
            // else return true.
            return !GetTerrain(point).IsBlocked;
        }

        #region Layer Control
        /// <summary>
        /// Sets the Unit id of the specified tile.
        /// </summary>
        /// <param name="x">X axis of the tile array.</param>
        /// <param name="y">Y axis of the tile array.</param>
        /// <param name="unitID">The ID of the unit (0 is none).</param>
        public void SetUnit(int x, int y, int unitID)
        {
            this.mapTiles[x, y].Unit = unitID;
        }

        /// <summary>
        /// Remove the unit from the specified coordinates.
        /// </summary>
        /// <param name="x">X axis of the Unit.</param>
        /// <param name="y">Y axis of the Unit.</param>
        public void RemoveUnit(int x, int y)
        {
            SetUnit(x, y, 0);
        }

        /// <summary>
        /// Populate the map with specified terrain/s.
        /// </summary>
        /// <param name="terrainIDs">An array of Terrain IDs to randomly fill the Map with.</param>
        private void FillMap(int[] terrainIDs)
        {
            Random RNG = new Random();

            for (int row = 0; row < this.mapTiles.X; row++)
            {
                for (int col = 0; col < this.mapTiles.Y; col++)
                {
                    // Set the Terrain ID to the first array element
                    int id = terrainIDs[0];

                    // If we have more than one Terrain IDs
                    // choose a random one
                    if (terrainIDs.Length > 1)
                    {
                        id = RNG.Next(0, terrainIDs.Length);

                        // If the tile is blocked 40% (from 6 to 9, inclusive)
                        // chance to get a new random ID.
                        if (gameData.TerrainList[id].IsBlocked)
                        {
                            if (RNG.Next(0, 10) > 5)
                                id = RNG.Next(0, terrainIDs.Length);
                        }
                    }

                    this.mapTiles[row, col] = new Tile(id, 0, 0, 0);
                }
            }
        }
        #endregion
    }
}
