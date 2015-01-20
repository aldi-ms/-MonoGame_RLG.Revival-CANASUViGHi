#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
#endregion

namespace CanasUvighi
{
    public class GameData
    {
        // Points to the default location of the save files.
        public const string TERRAIN_FILE = @"../../../sav/terrain.cusav";
        public const string UNIT_FILE = @"../../../sav/unit.cusav";
        public const string MAPLIST_FILE = @"../../../maps/maplist.culist";
        private FileManager fm;

        private string cipherKey;

        // Default encoding to use for files.
        private static readonly Encoding ENCODING = Encoding.UTF8;

        private List<string> mapPaths;
        
        // Loaded data/object lists
        private List<Terrain> terrainList;
        private List<Unit> npcList;
        private List<Map> mapList;
        private Unit playerCharacter;

        #region Constructors
        /// <summary>
        /// Create a new Data object to load modified/saved data values
        /// for common game objects / elements.
        /// </summary>
        public GameData(string pcName)
        {
            this.fm = new FileManager(pcName, false);

            // loaded in this.terrainList
            LoadTerrainList();

            // if save is present load its values
            // else throw ex
            if (fm.SaveExists)
            {
                // to this.mapList
                LoadMaps(SaveFolder.Modified);

                // to this.npcList
                LoadNPCList(SaveFolder.Modified);

                // to this.playerCharacter
                LoadPC();
            }
            else
                throw new ArgumentException("Character folder doesn't exist!", pcName);
        }

        public GameData(Unit playerCharacter)
        {
            // to this.terrainList
            LoadTerrainList();

            this.playerCharacter = playerCharacter;

            this.fm = new FileManager(playerCharacter.Name, true);

            // to this.mapList
            LoadMaps(SaveFolder.Default);

            // to this.npcList
            LoadNPCList(SaveFolder.Default);

        }
        #endregion

        #region Properties
        /// <summary>
        /// Return a list of all loaded Terrain objects.
        /// Position in list can be used as Terrain ID.
        /// </summary>
        public List<Terrain> TerrainDB
        {
            get { return this.terrainList; }
        }

        /// <summary>
        /// Return a list of all loaded Unit objects.
        /// Position in list can be used as Unit ID.
        /// </summary>
        public List<Unit> NPCList
        {
            get { return this.npcList; }

            set 
            {
                foreach (Unit unit in value)
                {
                    if (unit.IsPlayerControl)
                        throw new ArgumentException("No player control units in the NPC list!");
                }

                this.npcList = value;
            }
        }

        public Unit PlayerCharacter
        {
            get { return this.playerCharacter; }

            set
            {
                if (value.IsPlayerControl)
                    this.playerCharacter = value;
                else
                    throw new ArgumentException("Unit set to PlayerCharacter should be in player control!");
            }
        }

        public List<Map> MapList
        {
            get { return this.mapList; }
            set { this.mapList = value; }
        }
        #endregion

        /// <summary>
        /// Save Terrain, NPC, Map Lists as well as the player character.
        /// </summary>
        public void SaveGameData()
        {
            SaveTerrainList(FileManager.TerrainFile);
            SaveNPCList(SaveFolder.Modified);
            SaveMapList(SaveFolder.Modified);
            SavePC();
        }

        #region Map save/load
        /// <summary>
        /// Save the MapList to the folder specified.
        /// </summary>
        /// <param name="folder">Folder we want to save in.</param>
        public void SaveMapList(SaveFolder folder)
        {
            string path = "";

            switch (folder)
            {
                case SaveFolder.Default:
                    path = FileManager.DefaultMapsFolder;
                    break;

                case SaveFolder.Modified:
                    path = fm.ModifiedMapsFolder;
                    break;
            }

            foreach (Map map in this.mapList)
            {
                SaveMap(map, path);
            }
        }

        /// <summary>
        /// Load all maps from a folder to the MapList.
        /// </summary>
        /// <param name="folder">Folder from which we want to load.</param>
        public void LoadMaps(SaveFolder folder)
        {
            string[] mapsPathList = null;

            switch (folder)
            {
                case SaveFolder.Default:
                    mapsPathList = FileManager.ListOfDefaultMaps;
                    break;

                case SaveFolder.Modified:
                    mapsPathList = fm.ListOfModifiedMaps;
                    break;
            }

            foreach (string mapFile in mapsPathList)
            {
                using (var sReader = new StreamReader(mapFile, ENCODING))
                {
                    this.mapList.Add(LoadMap(mapFile));
                }
            }
        }

        /// <summary>
        /// Save the Map (with all objects inside it) to the give path + file.
        /// </summary>
        /// <param name="map">Map object we want to save.</param>
        /// <param name="filePath">File path to the save directory.
        /// (file name is auto-generated, do not supply it)</param>
        private void SaveMap(Map map, string filePath)
        {
            StringBuilder saveString = new StringBuilder();
            
            // Iterate over all Map Tiles
            for (int x = 0; x < map.Tiles.X; x++)
            {
                for (int y = 0; y < map.Tiles.Y; y++)
                {
                    saveString.Append(
                        JsonConvert.SerializeObject(map.Tiles[x, y])
                        );
                    // Insert new line
                    saveString.Append('\n');
                }
            }

            string fileName = string.Format("{0}_{1}[{2}x{3}].cumap", map.ID, map.Name, map.Height, map.Width);

            // Save to file, overwrite old
            using (var sWriter = new StreamWriter(filePath + fileName, false, ENCODING))
                sWriter.Write(saveString);
                //sWriter.Write(StringCipher.Encrypt(saveString.ToString(), cipherKey));
        }

        /// <summary>
        /// Load Map from a file.
        /// </summary>
        /// <param name="mapFile">The file to read.</param>
        /// <returns>The loaded Map object.</returns>
        private Map LoadMap(string mapFile)
        {
            #region Read file name info
            // ex. map file name: "../../../maps/0_TEST-MAP[22x36].cumap"
            string[] split = mapFile.Split('/');

            // Last element is file name. ex: "0_TEST-MAP[22x36].cumap"
            string fileName = split[split.Length - 1];

            // Split by '_', first element is the Map ID, 
            // rest is map name, size and file extension
            split = fileName.Split('_');
            int mapID = int.Parse(split[0]);

            // To get "TEST-MAP", "22x36", ".cumap"
            // we split by '[' and ']'
            split = split[1].Split('[', ']');
            string mapName = split[0];

            // Split 22x36 to get each Map dimension/size
            split = split[1].Split('x');
            int height = int.Parse(split[0]);
            int width = int.Parse(split[1]);
            #endregion

            string[] tileStrings;

            using (var sReader = new StreamReader(mapFile, ENCODING))
            {
                tileStrings = sReader.ReadToEnd().Split(
                    new char[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries
                    );
            }

            int counter = 0;
            FlatArray<Tile> tiles = new FlatArray<Tile>(height, width);

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    tiles[x, y] = JsonConvert.DeserializeObject<Tile>(tileStrings[counter]);
                    counter++;
                }
            }

            return new Map(mapID, mapName, tiles, this);
        }
        #endregion

        #region Unit save/load
        /// <summary>
        /// Save the Player Character.
        /// </summary>
        public void SavePC()
        {
            SaveUnitList(
                new List<Unit>() { this.playerCharacter },
                fm.PCFile
                );
        }

        /// <summary>
        /// Load the Player Character Unit to this.playerCharacter.
        /// </summary>
        public void LoadPC()
        {
            LoadUnitList(fm.PCFile);
        }

        /// <summary>
        /// Save the NPC List to the folder specified.
        /// </summary>
        /// <param name="folder">Folder we want to save in.</param>
        public void SaveNPCList(SaveFolder folder)
        {
            // check for PC ?
            switch (folder)
            {
                case SaveFolder.Default:
                    SaveUnitList(this.npcList, FileManager.DefaultNPCFile);
                    break;

                case SaveFolder.Modified:
                    SaveUnitList(this.npcList, fm.ModifiedNPCFile);
                    break;
            }
        }

        /// <summary>
        /// Load NPC List from the folder specified.
        /// </summary>
        /// <param name="folder">Folder from which we want to load.</param>
        public void LoadNPCList(SaveFolder folder) 
        {            
            switch (folder)
            {
                case SaveFolder.Default:
                    this.npcList = LoadUnitList(FileManager.DefaultNPCFile);
                    break;

                case SaveFolder.Modified:
                    this.npcList = LoadUnitList(fm.ModifiedNPCFile);
                    break;
            }
        }

        /// <summary>
        /// Save the UnitList to the given path + file. If old save file
        /// exists in the same folder it is overwritten!
        /// </summary>
        /// <param name="unitList">The Unit List we want to save.</param>
        /// <param name="npcFile">Path string to the location,
        /// where the save file will be created.</param>
        private void SaveUnitList(List<Unit> unitList, string npcFile) 
        {
            StringBuilder saveString = new StringBuilder();

            foreach (Unit unit in unitList) 
            {
                saveString.Append(
                    JsonConvert.SerializeObject(unit.ToJSONUnit())
                    );
                // Insert new line
                saveString.Append('\n');
            }

            // Save to file, overwrite old
            using (var sWriter = new StreamWriter(npcFile, false, ENCODING))
                sWriter.Write(saveString);
                //sWriter.Write(StringCipher.Encrypt(saveString.ToString(), "PassWord_01"));
        }

        /// <summary>
        /// Load a Unit List from the specified file.
        /// </summary>
        /// <param name="npcFile">Path + file to load.</param>
        /// <returns>List of Units.</returns>
        private List<Unit> LoadUnitList(string npcFile)
        {
            string[] unitStrings;

            using (var sReader = new StreamReader(npcFile, ENCODING))
            {
                unitStrings = sReader.ReadToEnd().Split(
                    new char[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries
                    );
            }

            List<Unit> loadedUnits = new List<Unit>();
            JSONUnit tempUnit = null;

            foreach (string unitStr in unitStrings)
            {
                tempUnit = JsonConvert.DeserializeObject<JSONUnit>(unitStr);
                loadedUnits.Add(tempUnit.//ToUnit());
            }

            return loadedUnits;
        }
        #endregion

        #region Terrain
        /// <summary>
        /// Append(!) a terrain to the list.
        /// </summary>
        /// <param name="terrain">Terrain we want to add.</param>
        public void AddTerrain(Terrain terrain)
        {
            this.terrainList.Add(terrain);
        }

        /// <summary>
        /// Load the Terrain List from file.
        /// </summary>
        public void LoadTerrainList()
        {
            this.terrainList.Clear();

            using (var sReader = new StreamReader(FileManager.TerrainFile, ENCODING))
            {
                string[] jsonStrings = sReader.ReadToEnd().Split(
                    new char[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries
                    );

                foreach (string jsonStr in jsonStrings) 
                {
                    JSONTerrain ter =  JsonConvert.DeserializeObject<JSONTerrain>(jsonStr);
                    this.terrainList.Add(ter.ToTerrain());
                }
            }
        }

        /// <summary>
        /// Save the TerrainList. Overwrites old file!
        /// </summary>
        /// <param name="terrainFile">String path to the location where the
        /// save file will be created.</param>
        private void SaveTerrainList(string terrainFile)
        {
            StringBuilder saveString = new StringBuilder();

            foreach (Terrain terrain in this.terrainList)
            {
                saveString.Append(
                    JsonConvert.SerializeObject(terrain.ToJSONTerrain())
                    );
                // Insert new line
                saveString.Append('\n');
            }

            // Save JSON terrain objects to file, overwrite with false!
            using (var sWriter = new StreamWriter(terrainFile, false, ENCODING))
                sWriter.Write(saveString);
        }
        #endregion
    }
}
