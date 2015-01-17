using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace CanasUvighi
{
    public class GameData
    {
        // Points to the default location of a terrain save file.
        private const string TERRAIN_FILE = @"../../../sav/terrain.sav";
        private const string UNIT_FILE = @"../../../sav/unit.sav";

        // Default encoding to use for files.
        private static readonly Encoding ENCODING = Encoding.UTF8;

        // Loaded object lists (DBs)
        private List<Terrain> terrainDB;
        private List<Unit> unitDB;

        /// <summary>
        /// Create a new Data object to hold data values for common game elements.
        /// </summary>
        public GameData()
        {
            this.terrainDB = LoadTerrain(TERRAIN_FILE);
        }

        #region Properties
        /// <summary>
        /// Return a list of all terrains that exist in the loaded DB. Position in list can be used as their "id".
        /// </summary>
        public List<Terrain> TerrainDB
        {
            get { return this.terrainDB; }
        }

        public List<Unit> UnitDB
        {
            get { return this.unitDB; }
            set { this.unitDB = value; }
        }
        #endregion

        /// <summary>
        /// Call to save all changes on previously loaded object lists (DBs).
        /// </summary>
        public void Save()
        {
            SaveTerrainList(TERRAIN_FILE);
            SaveUnitList(UNIT_FILE);
        }

        public void SaveMap(Map map)
        {
            // . . .
        }

        #region Unit
        /// <summary>
        /// Save the UnitList.  Overwrites old file!
        /// </summary>
        /// <param name="unitFile">Path string to the location
        /// where the save file will be created.</param>
        private void SaveUnitList(string unitFile) 
        {
            StringBuilder saveString = new StringBuilder();

            foreach (Unit unit in unitDB) 
            {
                saveString.Append(
                    JsonConvert.SerializeObject(unit.ToJSONUnit())
                    );
                saveString.Append('\n');
            }

            using (var sWriter = new StreamWriter(unitFile, false, ENCODING))
            {
                sWriter.Write(saveString);
            }
        }

        private List<Unit> LoadUnitList(string unitFile)
        {
            // . . .
            return new List<Unit>();
        }
        #endregion

        #region Terrain
        /// <summary>
        /// Save the TerrainList. Overwrites old file!
        /// </summary>
        /// <param name="terrainFile">String path to the location where the
        /// save file will be created.</param>
        private void SaveTerrainList(string terrainFile)
        {
            StringBuilder saveString = new StringBuilder();

            foreach (Terrain terrain in terrainDB)
            {
                saveString.Append(
                    JsonConvert.SerializeObject(terrain.ToJSONTerrain())
                    );
                saveString.Append('\n');
            }

            // Save JSON terrain objects to file, overwrites with false!
            using (var sWriter = new StreamWriter(terrainFile, false, ENCODING))
            {
                sWriter.Write(saveString);
            }
        }

        /// <summary>
        /// Load the TerrainList from a previously saved file. Called at constructor time.
        /// </summary>
        /// <param name="terrainFile">The save file to read from.</param>
        /// <returns>Returns the TerrainList.</returns>
        private List<Terrain> LoadTerrain(string terrainFile)
        {
            List<Terrain> loadedTerrains = new List<Terrain>();

            using (var sReader = new StreamReader(terrainFile, ENCODING))
            {
                string[] jsonObjects = sReader.ReadToEnd().Split('\n');

                foreach (string obj in jsonObjects) 
                {
                    // Skip empty strings
                    if (obj == "") continue;

                    JSONTerrain ter =  JsonConvert.DeserializeObject<JSONTerrain>(obj);
                    loadedTerrains.Add(ter.ToTerrain());
                }
            }

            return loadedTerrains;
        }
        #endregion
    }
}
