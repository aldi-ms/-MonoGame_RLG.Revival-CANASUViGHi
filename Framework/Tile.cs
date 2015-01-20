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

        /// <summary>
        /// Create a Tile with the specifit objects contained.
        /// Objects are parsed by ID.
        /// </summary>
        /// <param name="data">The Data element for all our loaded tables.</param>
        /// <param name="terrain">Terrain ID. Zero for open-grass Terrain.</param>
        /// <param name="gameObj">GameObject ID. Zero for no gameObj.</param>
        /// <param name="container">Item Bag ID. Zero for no gameObj.</param>
        /// <param name="unit">Unit ID. Zero for no gameObj.</param>
        public Tile(int terrain, int gameObj, int container, int unit)
        {
            this.layers = new TileLayers(terrain, gameObj, container, unit);
        }

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

        public TileLayers Layers
        {
            get { return this.layers; }
            private set { this.layers = value; }
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
    }
}