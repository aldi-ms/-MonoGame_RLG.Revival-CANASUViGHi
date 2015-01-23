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
        // Points to the location of the save files.
        private FileManager fm;

        // Default encoding to use for files.
        private static readonly Encoding ENCODING = Encoding.UTF8;
                
        // Loaded data/object lists
        private List<Terrain> terrainList;
        private List<Unit> unitList;
        private List<Map> mapList;

        #region Constructors
        /// <summary>
        /// Create a new Data object to load modified/saved data values
        /// for common game objects / elements.
        /// </summary>
        public GameData(string playerName)
        {
            // to this.terrainList
            LoadTerrainList();
            
            this.fm = new FileManager(playerName);

            SaveFolder sFolder = SaveFolder.Default;

            // if the save is present load its values
            // else load default
            if (fm.SaveExists)
            {
                sFolder = SaveFolder.Modified;
            }

            mapList = new List<Map>();
            // to this.mapList
            //LoadMaps(sFolder);

            // to this.npcList
            LoadUnitList(sFolder);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Return a list of all loaded Terrain objects.
        /// Position in list can be used as Terrain ID.
        /// </summary>
        public List<Terrain> TerrainList
        {
            get { return this.terrainList; }
        }

        /// <summary>
        /// Return a list of all loaded Unit objects.
        /// Position in list can be used as Unit ID.
        /// </summary>
        public List<Unit> UnitList
        {
            get { return this.unitList; }

            set
            { this.unitList = value; }
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
            SaveUnitList(SaveFolder.Modified);
            SaveMapList(SaveFolder.Modified);
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

            this.mapList = new List<Map>();

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
        /// Save the Unit List to the folder specified.
        /// </summary>
        /// <param name="folder">Folder we want to save in.</param>
        public void SaveUnitList(SaveFolder folder)
        {
            // check for PC ?
            switch (folder)
            {
                case SaveFolder.Default:
                    SaveUnitList(this.unitList, FileManager.DefaultUnitFile);
                    break;

                case SaveFolder.Modified:
                    SaveUnitList(this.unitList, fm.ModifiedUnitFile);
                    break;
            }
        }

        /// <summary>
        /// Load Unit List file from the folder specified.
        /// </summary>
        /// <param name="folder">Folder from which we want to load.</param>
        public void LoadUnitList(SaveFolder folder) 
        {            
            switch (folder)
            {
                case SaveFolder.Default:
                    if (File.Exists(FileManager.DefaultUnitFile))
                    {
                        this.unitList = LoadUnitList(FileManager.DefaultUnitFile);
                    }
                    break;

                case SaveFolder.Modified:
                    if (File.Exists(fm.ModifiedUnitFile))
                    {
                        this.unitList = LoadUnitList(fm.ModifiedUnitFile);
                    }
                    break;
            }

            if (this.unitList == null)
                this.unitList = new List<Unit>();
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
                    JsonConvert.SerializeObject(unit)
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
        /// <param name="unitFile">Path + file to load.</param>
        /// <returns>List of Units.</returns>
        private List<Unit> LoadUnitList(string unitFile)
        {
            string[] unitStrings;

            using (var sReader = new StreamReader(unitFile, ENCODING))
            {
                unitStrings = sReader.ReadToEnd().Split(
                    new char[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries
                    );
            }

            List<Unit> loadedUnits = new List<Unit>();
            //JSONUnit tempUnit = null;

            foreach (string unitStr in unitStrings)
            {
                //tempUnit = JsonConvert.DeserializeObject<JSONUnit>(unitStr);
                loadedUnits.Add(new Unit(this, JsonConvert.DeserializeObject<Unit>(unitStr)));
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
            this.terrainList = new List<Terrain>();

            using (var sReader = new StreamReader(FileManager.TerrainFile, ENCODING))
            {
                string[] jsonStrings = sReader.ReadToEnd().Split(
                    new char[] { '\n' },
                    StringSplitOptions.RemoveEmptyEntries
                    );

                foreach (string jsonStr in jsonStrings) 
                {
                    this.terrainList.Add(
                        JsonConvert.DeserializeObject<Terrain>(jsonStr)
                        );
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
                    JsonConvert.SerializeObject(terrain)
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
