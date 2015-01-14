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
            x,
            y,
            speed,
            energy;
        private string name;
        private string visual;
        private Color color;
        private Map map;

        #region Constructors
        /// <summary>
        /// Create a new Unit with the specified variables.
        /// </summary>
        /// <param name="id">The ID of the unit by DB.</param>
        /// <param name="name">Name of the unit.</param>
        /// <param name="visual">Visual string of the unit.</param>
        /// <param name="color">Color of the unit.</param>
        /// <param name="map">The Map on which the unit exists on.</param>
        /// <param name="x">X axis of the unit in the specified map.</param>
        /// <param name="y">Y axis of the unit in the specified map.</param>
        public Unit(int id, string name, string visual, Color color, Map map, int speed, int x, int y)
        {
            this.id = id;
            this.name = name;
            this.visual = visual;
            this.color = color;
            this.map = map;
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.energy = 0;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The unit ID by DB.
        /// </summary>
        public int ID
        {
            get { return this.id; }
        }

        public Map Map
        {
            get { return this.map; }

            set 
            {
                this.hasSpawned = false;
                this.map = value;
            }
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

            // check if new coordinates are valid / is the move legal
            if (ValidateMove(this.x + dX, this.y + dY))
            {
                // remove unit from old position
                this.map.RemoveUnit(this.x, this.y);

                // set new unit coordinates
                this.x += dX;
                this.y += dY;

                // set unit to the new position
                this.map.SetUnit(this.x, this.y, this.id);
            }
                // move was illegal, return false
            else 
                return false;

            // subract energy cost for the move
            this.energy -= 100;

            // move was made, return true
            return true;
        }

        /// <summary>
        /// Spawns the unit on its X and Y. Use once per map.
        /// </summary>
        public void Spawn()
        {
            if (!this.hasSpawned)
            {
                if (ValidateMove(this.x, this.y))
                {
                    this.map.SetUnit(this.x, this.y, this.ID);
                    this.hasSpawned = true;
                }
            }
        }

        public void MakePlayerControl()
        {
            this.isPlayerControl = true;
        }

        /// <summary>
        /// Check for situations that make the requested move invalid/illegal.
        /// </summary>
        /// <param name="x">X axis of the requested move.</param>
        /// <param name="y">Y axis of the requested move.</param>
        /// <returns>True for valid, False for invalid/illegal.</returns>
        private bool ValidateMove(int x, int y)
        {
            if (x < 0 || x >= map.Height ||
                y < 0 || y >= map.Width)
            {
                return false;
            }

            if (this.map.GetTerrain(x, y).IsBlocked)
            {
                return false;
            }

            return true;
        }
    }
}
