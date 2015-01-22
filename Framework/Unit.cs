using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace CanasUvighi
{
    public class Unit
    {
        private bool
            isPlayerControl = false,
            hasSpawned = false;
        private int
            id,
            x = -1,
            y = -1,
            speed,
            energy,
            mapID;
        private string
            name,
            visual;
        private Color color;
        private GameData gameData;

        #region Constructors
        /* *
        /// <summary>
        /// Create a new Unit with the specified variables. (Map)
        /// </summary>
        /// <param name="id">The ID of the Unit by DB.</param>
        /// <param name="name">Name of the Unit.</param>
        /// <param name="visual">Visual string representation of the Unit.</param>
        /// <param name="color">Color to use for Unit.</param>
        /// <param name="unitMap">The Map on which the Unit exists on.</param>
        /// <param name="x">X-axis of the Unit in the specified map.</param>
        /// <param name="y">Y-axis of the Unit in the specified map.</param>
        public Unit(int id, string name, string visual, Color color, Map unitMap, int speed, int x, int y)
        {
            this.id = id;
            this.name = name;
            this.visual = visual;
            this.color = color;
            this.unitMap = unitMap;
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.energy = 0;
        }
        * */

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
        public Unit(GameData gameData, int id, string name, string visual, Color color, int mapID, int speed, int x, int y)
        {
            this.gameData = gameData;
            this.id = id;
            this.name = name;
            this.visual = visual;
            this.color = color;
            this.mapID = mapID;
            this.speed = speed;

            this.x = x;
            this.y = y;

            this.energy = 0;
        }
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
            set { this.mapID = value; }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Map UnitMap
        {
            get { return gameData.MapList[mapID]; }
            /* *
            set 
            {
                this.hasSpawned = false;
                this.unitMap = value;
            }
            * */
        }

        public int X
        {
            get { return this.x; }
        }

        public int Y
        {
            get { return this.y; }
        }

        public int Speed
        {
            get { return this.speed; }
            set { }
        }

        public int Energy
        {
            get { return this.energy; }
            set { this.energy = value; }
        }

        public bool IsPlayerControl
        {
            get { return this.isPlayerControl; }
            set { this.isPlayerControl = value; }
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

                default:
                    break;

            }
            #endregion

            // Check if new coordinates are valid / is the move legal
            if (UnitMap.CheckTile(this.x + dX, this.y + dY))
            {
                // Remove unit from old position
                UnitMap.RemoveUnit(this.x, this.y);

                // Set new unit coordinates
                this.x += dX;
                this.y += dY;

                // Set unit to the new position
                UnitMap.SetUnit(this.x, this.y, this.id);
            }
                // Move was illegal, return false
            else 
                return false;

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
                if (UnitMap.CheckTile(this.x, this.y))
                {
                    UnitMap.SetUnit(this.x, this.y, this.ID);
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
                    this.x = x;
                    this.y = y;

                    UnitMap.SetUnit(this.x, this.y, this.ID);
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
        
        public JSONUnit ToJSONUnit()
        {
            JSONUnit jsonUnit = new JSONUnit();
            jsonUnit.isPlayerControl = this.isPlayerControl;
            jsonUnit.hasSpawned = this.hasSpawned;
            jsonUnit.id = this.id;
            jsonUnit.x = this.x;
            jsonUnit.y = this.y;
            jsonUnit.speed = this.speed;
            jsonUnit.energy = this.energy;
            jsonUnit.name = this.name;
            jsonUnit.visual = this.visual;
            jsonUnit.color = this.color;
            jsonUnit.mapID = this.mapID;

            return jsonUnit;
        }
    }
}
