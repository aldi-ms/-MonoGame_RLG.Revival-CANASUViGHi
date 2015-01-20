using System;
using System.IO;

namespace CanasUvighi
{
    public class FileManager
    {
        private const string
            mainFolder = "../../../data/",
            dataDefault = "default/",
            mapsFolder = "maps/",
            terrainFile = "terrain.cu",
            npcUnitFile = "npc_unit.cu";

        private string
            charName,
            pcFile;

        /// <summary>
        /// Creates new FileManager to keep all folder/file path strings.
        /// </summary>
        /// <param name="pcName">Player Character name is used as save folder name.</param>
        /// <param name="createFolders">False if we are loading a previously saved game.
        /// True for new game to create folder structure.</param>
        public FileManager(string pcName, bool createFolders)
        {
            this.charName = pcName;
            this.pcFile = pcName + ".cu";

            if (createFolders)
                CreateFolderStructure();
        }
        
        #region Default (static) data folders & files
        /// <summary>
        /// Get the game terrain file. (static through games file)
        /// </summary>
        public static string TerrainFile
        {
            get { return DefaultDataFolder + terrainFile; }
        }

        /// <summary>
        /// Path to the folder containing the default game files.
        /// </summary>
        public static string DefaultDataFolder
        {
            get { return mainFolder + dataDefault; }
        }

        /// <summary>
        /// Path to the folder containing the default map files.
        /// </summary>
        public static string DefaultMapsFolder
        {
            get { return DefaultDataFolder + mapsFolder; }
        }
        
        /// <summary>
        /// Path + file containing the default Non-Player Characters.
        /// </summary>
        public static string DefaultNPCFile
        {
            get { return DefaultDataFolder + npcUnitFile; }
        }

        /// <summary>
        /// Get all maps in the default map folder. (def: "default/maps/")
        /// (Non-Recursive folders)
        /// </summary>
        public static string[] ListOfDefaultMaps
        {
            get
            {
                return Directory.GetFiles(
                    DefaultMapsFolder,
                    "*.cumap",
                    SearchOption.TopDirectoryOnly
                    );
            }
        }
        #endregion

        #region Current player character (modified) folders & files
        /// <summary>
        /// Path to Player Character save folder.
        /// </summary>
        public string PlayerCharacterFolder
        {
            get { return mainFolder + charName + "/"; }
        }

        /// <summary>
        /// Path to Player Character modified/saved maps folder.
        /// </summary>
        public string ModifiedMapsFolder
        {
            get { return PlayerCharacterFolder + mapsFolder; }
        }

        /// <summary>
        /// Path + file containing this Player Character.
        /// </summary>
        public string PCFile
        {
            get { return PlayerCharacterFolder + this.pcFile; }
        }

        /// <summary>
        /// Path + file containing the modified Non-Player Characters.
        /// </summary>
        public string ModifiedNPCFile
        {
            get { return PlayerCharacterFolder + npcUnitFile; }
        }
        
        /// <summary>
        /// Get all maps in the modified map folder (def: "%char_name%/maps").
        /// (Non-Recursive folders)
        /// </summary>
        public string[] ListOfModifiedMaps
        {
            get
            {
                return Directory.GetFiles(
                    ModifiedMapsFolder, 
                    "*.cumap",
                    SearchOption.TopDirectoryOnly
                    );
            }
        }

        public bool SaveExists
        {
            get
            {
                return Directory.Exists(PlayerCharacterFolder);
            }
        }
        #endregion

        public void DeleteCharacterFolders()
        {
            if (Directory.Exists(PlayerCharacterFolder))
            {
                Directory.Delete(PlayerCharacterFolder, true);
            }
        }

        private void CreateFolderStructure()
        {
            // Default dir should exist.
            if (!Directory.Exists(DefaultDataFolder))
            {
                Directory.CreateDirectory(DefaultDataFolder);
                Directory.CreateDirectory(DefaultMapsFolder);
            }

            // Create character directories
            Directory.CreateDirectory(PlayerCharacterFolder);
            Directory.CreateDirectory(ModifiedMapsFolder);
        }
    }

}
