namespace CanasUvighi
{
    /// <summary>
    /// Class keeping save the number / order of layers in a tile.
    /// </summary>
    public class TileLayers
    {
        // Commented are the number priority of the layer (higher number is higher priority)
        private int
            terrain,    //0
            gameObj,    //1
            container,        //2
            unit;       //3

        #region Constructor
        /// <summary>
        /// Create a Tile object with specified element IDs.
        /// </summary>
        /// <param name="terrain">Terrain object ID.</param>
        /// <param name="gameObj">GameObject ID.</param>
        /// <param name="bag">Bag ID.</param>
        /// <param name="unit">Unit ID.</param>
        public TileLayers(int terrain, int gameObj, int bag, int unit)
        {
            this.terrain = terrain;
            this.gameObj = gameObj;
            this.container = bag;
            this.unit = unit;
        }

        /// <summary>
        /// Create a default empty Tile object.
        /// </summary>
        public TileLayers()
            : this(0, 0, 0, 0) { }
        #endregion

        #region Properties
        public int GameObj
        {
            get { return this.gameObj; }
            set { this.gameObj = value; }
        }

        public int Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }

        public int Terrain
        {
            get { return this.terrain; }
            set { this.terrain = value; }
        }

        public int Container
        {
            get { return this.container; }
            set { this.container = value; }
        }
        #endregion
    }
}
