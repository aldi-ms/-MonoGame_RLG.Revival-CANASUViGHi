using System;

namespace CanasUvighi
{
    public class Map
    {
        private string name;
        private FlatArray<Tile> tiles;
        private GameData dbData;
        private Random RNG = new Random();

        public Map(string name, FlatArray<Tile> tiles, GameData dbData, bool randomTerrain = false)
        {
            this.name = name;
            this.tiles = tiles;
            this.dbData = dbData;

            this.PopulateMap(0, randomTerrain);
        }

        #region Properties
        public string Name
        {
            get { return this.name; }
        }

        public FlatArray<Tile> Tiles
        {
            get { return this.tiles; }
            set { this.tiles = value; }
        }

        public GameData DBData
        {
            get { return this.dbData; }
            //set { this.dbData = value; }
        }

        public int Height
        {
            get { return this.tiles.Height; }
        }

        public int Width
        {
            get { return this.tiles.Width; }
        }
        #endregion

        /// <summary>
        /// Populate the whole map with specified terrain.
        /// </summary>
        /// <param name="terrain">Fill the map with the Terrain.</param>
        private void PopulateMap(int terrainID = 0, bool randomize = false)
        {
            for (int row = 0; row < this.tiles.Height; row++)
            {
                for (int col = 0; col < this.tiles.Width; col++)
                {
                    if (randomize)
                    {
                        // 0 to 6 == 70% (7 points in) for grass terrain (==0)
                        if (RNG.Next(0, 10) > 6)
                            terrainID = 1;
                        else
                            terrainID = 0;
                    }

                    this.tiles[row, col] = new Tile(terrainID, 0, 0, 0);
                }
            }
        }

        public string GetTileVisual(int x, int y)
        {
            return this.tiles[x, y].GetTileVisual(this.dbData);
        }

        public Terrain GetTerrain(int x, int y)
        {
            return this.dbData.TerrainDB[this.tiles[x, y].Terrain];
        }

        #region Alternate Layer control
        /// <summary>
        /// Sets the Unit id of the specified tile.
        /// </summary>
        /// <param name="x">X axis of the tile array.</param>
        /// <param name="y">Y axis of the tile array.</param>
        /// <param name="unitID">The ID of the unit (0 is none).</param>
        public void SetUnit(int x, int y, int unitID)
        {
            this.tiles[x, y].Unit = unitID;
        }

        /// <summary>
        /// Remove the unit from the specified coordinates.
        /// </summary>
        /// <param name="x">X axis of the unit.</param>
        /// <param name="y">Y axis of the unit.</param>
        public void RemoveUnit(int x, int y)
        {
            SetUnit(x, y, 0);
        }
        #endregion
    }
}
