using System;

namespace CanasUvighi
{
    /// <summary>
    /// A tile is a basic game object keeping all we need to keep during a game for a single tile.
    /// The game world/map is composed of tiles.
    /// </summary>
    public class Tile : IFovCell
    {
        private TileLayers layers;

        #region Constructors
        /// <summary>
        /// Create a Tile with the specifit objects contained.
        /// Objects are parsed through ID (int).
        /// </summary>
        /// <param name="data">The Data element for all our loaded tables.</param>
        /// <param name="terrain">Terrain ID. Here 0 value means grass.</param>
        /// <param name="gameObj">GameObject ID. 0 value means no gameObj.</param>
        /// <param name="container">Item Bag ID. 0 value means no gameObj.</param>
        /// <param name="unit">Unit ID. 0 value means no gameObj.</param>
        public Tile(int terrain, int gameObj, int container, int unit)
        {
            this.layers = new TileLayers(terrain, gameObj, container, unit);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Used by the FOV engine. Do not set manually.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Needed of the IFovCell interface, for the FOV engine.
        /// </summary>
        public bool IsTransparent
        {
            get { return true; }
        }

        private TileLayers Layers
        {
            get { return this.layers; }
            set { this.layers = value; }
        }

        // Create properties for each layer, returning int
        public int Unit
        {
            get { return this.layers.Unit; }
            set { this.layers.Unit = value; }
        }

        public int Container
        {
            get { return this.layers.Container; }
            set { this.layers.Container = value; }
        }

        public int GameObj
        {
            get { return this.layers.GameObj; }
            set { this.layers.GameObj = value; }
        }

        public int Terrain
        {
            get { return this.layers.Terrain; }
            set { this.layers.Terrain = value; }
        }
        #endregion

        /// <summary>
        /// Get the highest priority element contained in the tile.(Unit > Container > GameObj > Terrain)
        /// </summary>
        /// <param name="dbData"></param>
        /// <returns>A string of the highest priority element.</returns>
        public string GetTileVisual(GameData dbData)
        {
            if (this.layers.Unit != 0)
            {
                /* logic to get the specified Unit 
                 * visual str from the DB (by ID).*/
                return "@";
            }
            else if (this.layers.Container != 0)
            {
                /* logic to get the specified Container 
                 * visual str from the DB (by ID).*/
                return "o";
            }
            else if (this.layers.GameObj != 0)
            {
                /* logic to get the specified GameObj 
                 * visual str from the DB (by ID).*/
                return "$";
            }

            // if there is nothing else in this Tile 
            // just show the terrain
            int n = this.layers.Terrain;

            return dbData.TerrainDB[n].Visual;
        }

    }
}