using Microsoft.Xna.Framework;

namespace CanasUvighi
{
    /// <summary>
    /// Temporary class for loading JSON Terrain.
    /// </summary>
    public class JSONTerrain
    {
        public int id;
        public string
            name,
            visual;
        public bool
            isTransparent,
            isBlocked;

        /// <summary>
        /// Convert to actual Terrain object.
        /// </summary>
        /// <returns></returns>
        public Terrain ToTerrain()
        {
            return new Terrain(id, name, visual, isTransparent, isBlocked);
        }
    }

    public class JSONUnit
    {
        public bool
            isPlayerControl = false,
            hasSpawned = false;
        public int
            id,
            x,
            y,
            speed,
            energy;
        public string 
            name,
            visual;
        public Color color;
        public Map map;

        public Unit ToUnit()
        {
            return new Unit(id, name, visual, color, map, speed, x, y);
        }
    }
}