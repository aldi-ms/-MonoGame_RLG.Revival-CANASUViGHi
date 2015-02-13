using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace CanasUvighi
{
    public class Unit : IActor, IComparable<Unit>
    {
        private bool
            isPlayerControl = false,
            hasSpawned = false;
        private int
            id,
            speed,
            energy,
            mapID;
        private string
            name,
            visual;
        private Point position;
        private Color color;
        private GameData gameData;

        #region Constructors
        /// <summary>
        /// Create a new Unit with the specified variables. (MapID)
        /// </summary>
        /// <param name="id">The ID of the Unit by DB.</param>
        /// <param name="name">Name of the Unit.</param>
        /// <param name="visual">Visual string representation of the Unit.</param>
        /// <param name="color">Color to use for Unit.</param>
        /// <param name="mapID">ID of the Map on which the Unit exists.</param>
        /// <param name="x">X-axis of the Unit in the specified map.</param>
        /// <param name="y">Y-axis of the Unit in the specified map.</param>
        [JsonConstructor]
        public Unit(GameData gameData, int id, string name, string visual, Color color, int mapID, int speed, int x, int y)
        {
            this.gameData = gameData;
            this.id = id;
            this.name = name;
            this.visual = visual;
            this.color = color;
            this.mapID = mapID;
            this.speed = speed;
            this.position = new Point(x, y);

            this.energy = 0;
        }

        public Unit(GameData gameData, Unit unit)
            :this(
            gameData, 
            unit.id,
            unit.name,
            unit.visual,
            unit.color,
            unit.mapID,
            unit.speed,
            unit.position.X,
            unit.position.Y)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// Unit ID by GameData.
        /// </summary>
        public int ID
        {
            get { return this.id; }
        }

        public int MapID
        {
            get { return this.mapID; }

            set
            { 
                // Check if the parsed value is an existing Map!
                if (this.gameData.MapList[value] != null)
                {
                    this.mapID = value;
                }
            }
        }

        public string Visual
        {
            get { return this.visual; }
        }

        public string Name
        {
            get { return this.name; }
        }
        
        public int X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }

        public int Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        public int Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        public bool IsPlayerControl
        {
            get { return this.isPlayerControl; }
            set { this.isPlayerControl = value; }
        }

        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        [JsonIgnore]
        public int Energy
        {
            get { return this.energy; }
            set { this.energy = value; }
        }

        [JsonIgnore]
        public Map UnitMap
        {
            get { return gameData.MapList[mapID]; }
        }
        #endregion

        public bool Move(CardinalDirection dir)
        {
            #region Get delta coordinates
            int dX = 0;
            int dY = 0;

            switch (dir)
            {
                case CardinalDirection.North:
                    dX = -1;
                    break;

                case CardinalDirection.South:
                    dX = 1;
                    break;

                case CardinalDirection.West:
                    dY = -1;
                    break;

                case CardinalDirection.East:
                    dY = 1;
                    break;

                case CardinalDirection.NorthWest:
                    dX = -1;
                    dY = -1;
                    break;

                case CardinalDirection.NorthEast:
                    dX = -1;
                    dY = 1;
                    break;

                case CardinalDirection.SouthEast:
                    dX = 1;
                    dY = 1;
                    break;

                case CardinalDirection.SouthWest:
                    dX = 1;
                    dY = -1;
                    break;
            }
            #endregion

            // Check if new coordinates are valid / is the move legal
            if (UnitMap.CheckTile(this.X + dX, this.Y + dY))
            {
                // Remove unit from old position
                UnitMap.RemoveUnit(this.X, this.Y);

                // Set new unit coordinates
                this.X += dX;
                this.Y += dY;

                // Set unit to the new position
                UnitMap.SetUnit(this.X, this.Y, this.id);
            }
            // Move was illegal, return false
            else
            {
                return false;
            }

            // Subract energy cost for the move
            this.energy -= 100;

            // Move was successful, return true
            return true;
        }

        /// <summary>
        /// Spawns the unit on its map and coordinates.
        /// </summary>
        public bool Spawn()
        {
            if (!this.hasSpawned)
            {
                if (UnitMap.CheckTile(this.X, this.Y))
                {
                    UnitMap.SetUnit(this.X, this.Y, this.ID);
                    this.hasSpawned = true;
                    
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Spawn the unit on specified coordinates, if valid.
        /// </summary>
        /// <param name="x">X-axis of the map tile.</param>
        /// <param name="y">X-axis of the map tile.</param>
        /// <returns>True if the Unit was spawned successfully. 
        /// False otherwise (eg. when the unit has already been spawned).</returns>
        public bool Spawn(int x, int y)
        {
            // Unit not-spawned - proceed.
            if (!this.hasSpawned)
            {
                if (UnitMap.CheckTile(x, y))
                {
                    // Set unit coordinates.
                    this.X = x;
                    this.Y = y;

                    this.UnitMap.SetUnit(this.X, this.Y, this.ID);
                    this.hasSpawned = true;

                    // We have spawned the unit successfully
                    // -> return true
                    return true;
                }
            }
            // The unit has already been spawned
            // -> return false
            return false;
        }

        public int CompareTo(Unit unit)
        {
            int result = unit.energy - this.energy;
            return result;
        }
    }
}
