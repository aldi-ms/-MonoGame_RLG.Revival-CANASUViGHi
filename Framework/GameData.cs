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
        private static readonly Encoding ENCODING = Encoding.UTF8;
        private List<Terrain> terrainDB;
        private List<Unit> unitDB;

        /// <summary>
        /// Create a new Data object to hold data values for common game elements.
        /// </summary>
        public GameData()
        {
            this.terrainDB = LoadTerrain();
        }

        #region Properties
        /// <summary>
        /// Return a list of all terrains that exist in the loaded DB. Positions in list can be used as their "id".
        /// </summary>
        public List<Terrain> TerrainDB
        {
            get { return this.terrainDB; }
        }

        public List<Unit> UnitDB
        {
            get { return this.unitDB; }
        }

        public void SaveToFiles()
        {
            this.SaveTerrainList();
        }
        #endregion

        private void SaveMap(FlatArray<Tile> gameWorld)
        {
            for (int row = 0; row < gameWorld.Width; row++)
            {
                for (int col = 0; col < gameWorld.Height; col++)
                {
                    // . . .
                }
            }
        }
        
        /// <summary>
        /// Save the TerrainList.
        /// </summary>
        /// <param name="terrainFile">Path string to the location where the save file will be created.
        /// Default is @"../../sav/terrain.sav". Overwrites old file.</param>
        private void SaveTerrainList(string terrainFile = TERRAIN_FILE)
        {
            StringBuilder saveString = new StringBuilder();

            foreach (var terrain in this.terrainDB)
            {
                saveString.Append(
                    JsonConvert.SerializeObject(terrain.ToJSONTerrain())
                    );
                saveString.Append('\n');
            }

            // Save JSON terrain objects to file, overwrite with false!
            using (var sWriter = new StreamWriter(terrainFile, false, ENCODING))
            {
                sWriter.Write(saveString);
            }
        }

        /// <summary>
        /// Load the TerrainList from a previously saved file. Called at constructor time.
        /// </summary>
        /// <param name="terrainFile">The save file to read from. Default is @"../../../sav/terrain.sav".</param>
        /// <returns>Returns the TerrainList.</returns>
        private List<Terrain> LoadTerrain(string terrainFile = TERRAIN_FILE)
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
    }
}
