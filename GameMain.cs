#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace CanasUvighi
{
    /// <summary>
    /// This is the main type for the game.
    /// </summary>
    public class GameMain : Game
    {
        #region Fields
        // Game window resolution
        // 1024 x 640 is 16:10 aspect ratio
        // (close to golden ratio)
        private const int
            TILE_SIZE = 32,
            SCREEN_WIDTH = 1024,
            SCREEN_HEIGHT = 640,

            // minimum energy needed for taking a turn
            TURN_COST_MIN = 100;

        private ulong turns;

        private bool
            inGame = false,
            inMainMenu = false,
            inOverWriteMenu = false,
            waitPlayerAction = false,
            secondaryMenuChoice = false;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont
            instruction22,
            K8KurrierFixed20,
            specialElite22,
            consolas12;

        // gameBox is the rectangle containing
        // the actual game field/board
        private Rectangle gameBox;
        private List<Rectangle> windowBorders;

        // Loaded text-file based predefined game objects
        private GameData gameData;

        // The previous (old) and the current (new)
        // keyboard states to check for key presses
        private KeyboardState 
            oldKBState,
            newKBState;

        // Units Player Character
        private Unit PC;

        // the main menu
        private Menu 
            mainMenu, 
            overwriteMenu;

        // colors used for the game
        private Color
            borderColor = new Color(3, 54, 73),
            fontColor = new Color(205, 179, 128),
            inActiveFontColor = new Color(232, 221, 203),
            backgroundColor = new Color(3, 101, 100);

        private FieldOfView<Tile> fieldOfView;
        private Point 
            fovSource,
            viewBoxTiles;
        #endregion

        #region Constructor, Init, (Un)LoadContent
        public GameMain()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
            
            this.IsMouseVisible = true;
            this.fovSource = new Point();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Font Load
            /* *
             * Old Fonts (implement font loading
             * & more fonts for UI extensibility/configurability)
            GOSTTypeA30 = Content.Load<SpriteFont>("GOST_type_A30");
            GOSTTypeA60 = Content.Load<SpriteFont>("GOST_type_A60");
            square10 = Content.Load<SpriteFont>("square10");
            GOSTTypeA30 = Content.Load<SpriteFont>("GOST_type_A30");
            consolas16 = Content.Load<SpriteFont>("Consolas16");
            square18 = Content.Load<SpriteFont>("Square18");
            pericles18 = Content.Load<SpriteFont>("pericles18");
            captureIt42 = Content.Load<SpriteFont>("capture_it42");
            sketchFlow50 = Content.Load<SpriteFont>("SketchFlow_Print50");
            GOSTTypeA50 = Content.Load<SpriteFont>("GOST_type_A50");
            GtekNova60 = Content.Load<SpriteFont>("GtekNova60");
             * */

            consolas12 = Content.Load<SpriteFont>("Consolas12");                // For debug purposes.
            instruction22 = Content.Load<SpriteFont>("Instruction22");           // Headlines. No lowcap letters.
            K8KurrierFixed20 = Content.Load<SpriteFont>("K8KurierFixed20");    // Great Courier. Use for map/visual characters.
            specialElite22 = Content.Load<SpriteFont>("SpecialElite22");        // Objectives, text, explanations, etc.
            #endregion

            // Create and configure a Menu, which will be used as the Game's Main Menu
            mainMenu = UI.MainMenu(
                spriteBatch, 
                instruction22, 
                specialElite22,
                new Vector2(50f, 50f),
                fontColor, 
                inActiveFontColor);

            // Indicates that we are in the game menu.
            inMainMenu = true;

            // Initialize border rectangles,
            // other hard-coded values (viewBoxTiles)
            SetupPredefined();
            
            // get a state for the oldState of keyboard
            oldKBState = Keyboard.GetState();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            // Get the current Keyboard state
            newKBState = Keyboard.GetState();

            #region In Menu
            if (inMainMenu)
            {
                // Down (arrows, numpad and vi-keys)
                if (CheckKeys(Keys.Down, Keys.J, Keys.NumPad2))
                {
                    mainMenu.Next();
                }

                // Up (arrows, numpad and vi-keys)
                if (CheckKeys(Keys.Up, Keys.K, Keys.NumPad8))
                {
                    mainMenu.Previous();
                }

                // Enter || Space to choose
                if (CheckKeys(Keys.Enter, Keys.Space))
                {
                    inMainMenu = false; 

                    switch (mainMenu.Choose())
                    {
                        case 0:
                            NewGame(); 
                            break;

                        case 1:
                            LoadGame();
                            break;

                        case 3:
                            Quit(); 
                            break;

                        default:
                            inMainMenu = true; 
                            break;
                    }
                }
            }

            if (inOverWriteMenu)
            {
                // Down (arrows, numpad and vi-keys)
                if (CheckKeys(Keys.Down, Keys.J, Keys.NumPad2))
                {
                    overwriteMenu.Next();
                }

                // Up (arrows, numpad and vi-keys)
                if (CheckKeys(Keys.Up, Keys.K, Keys.NumPad8))
                {
                    overwriteMenu.Previous();
                }

                // Boolean secondaryMenuChoice is needed for skipping the 
                // keypress from the previous menu (main menu), otherwise
                // it is caught and choise is made instantly.
                if (CheckKeys(Keys.Enter, Keys.Space) && secondaryMenuChoice)
                {
                    inOverWriteMenu = false;

                    switch (overwriteMenu.Choose())
                    {
                        case 0:
                            secondaryMenuChoice = false;
                            NewGame(true);
                            break;

                        case 1:
                            secondaryMenuChoice = false;
                            LoadGame();
                            break;

                        default:
                            inOverWriteMenu = true;
                            break;
                    }
                }

                // After skipping the first iteration
                // set bool to true so we can get the
                // real keypress.
                secondaryMenuChoice = true;
            }
            #endregion

            #region In Game
            if (inGame)
            {
                // Spawn test NPC unit
                if (CheckKeys(Keys.Q))
                {
                    Unit npc = new Unit(
                        gameData,
                        gameData.UnitList.Count + 1, 
                        "npc_test",
                        "q",
                        Color.Blue,
                        PC.MapID,
                        10, 8, 8);
                    gameData.UnitList.Add(npc);
                    npc.Spawn();
                }

                // If the player is not taking a turn - add speed 
                // to all actors' energy
                if (!waitPlayerAction)
                {
                    foreach (IActor actor in gameData.UnitList)
                    {
                        actor.Energy += actor.Speed;
                    }
                }

                // Sorts the actors by Energy
                gameData.UnitList.Sort();

                // Give control to actors with Energy at least
                // the base turn cost
                foreach (IActor actor in gameData.UnitList)
                {
                    if (actor.Energy >= TURN_COST_MIN)
                    {
                        if (actor.IsPlayerControl)
                        {
                            waitPlayerAction = true;

                            // Actor / Unit is Player Controlled
                            #region Take action
                            // South
                            if (CheckKeys(Keys.Down, Keys.NumPad2, Keys.J))
                            {
                                if (actor.Move(CardinalDirection.South))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // North
                            if (CheckKeys(Keys.Up, Keys.NumPad8, Keys.K))
                            {
                                if (actor.Move(CardinalDirection.North))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // West
                            if (CheckKeys(Keys.Left, Keys.NumPad4, Keys.H))
                            {
                                if (actor.Move(CardinalDirection.West))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // East
                            if (CheckKeys(Keys.Right, Keys.NumPad6, Keys.L))
                            {
                                if (actor.Move(CardinalDirection.East))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // South West
                            if (CheckKeys(Keys.B, Keys.NumPad1))
                            {
                                if (actor.Move(CardinalDirection.SouthWest))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // South East
                            if (CheckKeys(Keys.N, Keys.NumPad3))
                            {
                                if (actor.Move(CardinalDirection.SouthEast))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // North West
                            if (CheckKeys(Keys.Y, Keys.NumPad7))
                            {
                                if (actor.Move(CardinalDirection.NorthWest))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }

                            // North East
                            if (CheckKeys(Keys.U, Keys.NumPad9))
                            {
                                if (actor.Move(CardinalDirection.NorthEast))
                                {
                                    this.turns++;
                                    waitPlayerAction = false;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            if (!waitPlayerAction)
                            {
                                // Actor / Unit is Non-Player Controlled
                                actor.Move(AI.DrunkardWalk());
                            }
                        }
                    }
                }
            }
            #endregion

            if (CheckKeys(Keys.Escape))
            {
                if (inMainMenu)
                {
                    Quit();
                }
                else if (inGame)
                {
                    inGame = false;
                    inOverWriteMenu = false;
                    inMainMenu = true;
                    gameData.SaveGameData();
                }
                else if (inOverWriteMenu)
                {
                    inOverWriteMenu = false;
                    secondaryMenuChoice = false;
                    inGame = false;
                    inMainMenu = true;
                }
            }

            // Set the old keyboard state to newKBState
            this.oldKBState = newKBState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            // Clear window and set color
            if (inGame)
                GraphicsDevice.Clear(Color.Black);
            else if (inMainMenu)
                GraphicsDevice.Clear(borderColor);

            spriteBatch.Begin();
            
            // Define basic Texture2D element for drawing
            Texture2D simpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            simpleTexture.SetData<Color>( new[] { Color.White } );
            
            if (inMainMenu)
            {
                // Draw the main menu.
                mainMenu.Draw();
            }
            else if (inOverWriteMenu)
            {
                // Draw the overwrite "menu"
                // (ask whether we should overwrite 
                // an existing save or load it).
                overwriteMenu.Draw();
            }
            else if (inGame)
            {
                // This is the gameBox background
                spriteBatch.Draw(simpleTexture, gameBox, backgroundColor);

                #region Draw grid
                bool drawGrid = false;
                if (drawGrid)
                {
                    for (int rows = gameBox.Y; rows < gameBox.Y + gameBox.Height; rows += TILE_SIZE)
                    {
                        var tempRect = new Rectangle(gameBox.X, rows, gameBox.Width, 1);
                        spriteBatch.Draw(simpleTexture, tempRect, Color.DarkSlateGray);
                    }
                    for (int cols = gameBox.X; cols < gameBox.Y + gameBox.Width; cols += TILE_SIZE)
                    {
                        var tempRect = new Rectangle(cols, gameBox.Y, 1, gameBox.Height);
                        spriteBatch.Draw(simpleTexture, tempRect, Color.DarkSlateGray);
                    }
                }
                #endregion

                #region Draw window & gameBox borders
                // draw the window borders
                for (int i = 0; i < windowBorders.Count; i++)
                {
                    spriteBatch.Draw(simpleTexture, windowBorders[i], this.borderColor);
                }

                // gameBox left border
                spriteBatch.Draw(simpleTexture, new Rectangle(this.gameBox.Width, 10, 10, this.gameBox.Height), this.borderColor);

                // gameBox bottom border
                spriteBatch.Draw(simpleTexture, new Rectangle(10, this.gameBox.Height + 10, this.gameBox.Width, 10), this.borderColor);
                #endregion

                #region Draw Map
                // if other units/moveable objects that are not transparent
                // move in player's fov, check becomes wrong
                if (PC.X != fovSource.X || PC.Y != fovSource.Y)
                {
                    fovSource.X = PC.X;
                    fovSource.Y = PC.Y;
                    fieldOfView.ComputeFov(PC.X, PC.Y, 5, true, FOVMethod.MRPAS, RangeLimitShape.Octagon);
                }

                // Get the start coordinates for the Map
                int mapX = PC.X - (viewBoxTiles.X / 2);
                int mapY = PC.Y - (viewBoxTiles.Y / 2);

                // Check the lowerbound X and Y
                if (mapX < 0) mapX = 0;
                if (mapY < 0) mapY = 0;

                // Number of tiles in the gameBox are predefined in 
                // viewBoxTiles point, values calculated in SetupPredefined();

                // Check the higherbound (X)
                if (mapX + viewBoxTiles.X >= gameData.MapList[PC.MapID].Height)
                    mapX = gameData.MapList[PC.MapID].Height - viewBoxTiles.X;
                // Check the higherbound (Y)
                if (mapY + viewBoxTiles.Y >= gameData.MapList[PC.MapID].Width)
                    mapY = gameData.MapList[PC.MapID].Width - viewBoxTiles.Y;

                for (int x = 0; x < viewBoxTiles.X; x++)
                {
                    for (int y = 0; y < viewBoxTiles.Y; y++)
                    {
                        Vector2 vect = new Vector2(14 + y * TILE_SIZE, 10 + x * TILE_SIZE);
                        
                        spriteBatch.DrawString(
                            K8KurrierFixed20,
                            gameData.MapList[PC.MapID].TileVisual(mapX + x, mapY + y),
                            vect,
                            fontColor);
                    }
                }
                #endregion
            }
            
            #region Debug string
            // Show debug info for mouse coordinates
            // MouseState mouse = Mouse.GetState();
            string debugInfo = "";
            if (PC != null)
            {
                debugInfo = 
                    //string.Format("row:{0};col:{1};ttl_time:{2}", this.PC.X, this.PC.Y, gameTime.TotalGameTime);
                    string.Format("[x:{0};y:{1}] energy:{2}; turns:{3}", PC.X, PC.Y, PC.Energy, turns);
                    //string.Format("x:{0},y:{1}", mouse.X, mouse.Y);
                    //string.Format("gameBox[x:{0};y:{1};width:{2};height{3};]", gameBox.X, gameBox.Y, gameBox.Width, gameBox.Height);
            }

            spriteBatch.DrawString(consolas12, debugInfo, new Vector2(15, GraphicsDevice.Viewport.Height - 32), this.fontColor);
            #endregion

            #region Font testing
            /* 
            for (int i = 0; i < fontTesting.Length; i++)
			{
                var vector2 = new Vector2(20, 20 + (i * 48));
                var testStr = "`~!@#$%^&*()_+=-0987654321\\|;<>,./? A single newline in enriched text 
             * is treated as a space. Formatting commands are in the same style as SGML and HTML. They must be balanced and nested.";
			    this.spriteBatch.DrawString(fontTesting[i], testStr, vector2, Color.White);
			}
            */
            #endregion

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Start a new game.
        /// </summary>
        private void NewGame(bool overwrite = false)
        {
            // ask for player name
            // hardcode for now - MonoGame doesn't have a Buffer Input
            string playerName = "SCiENiDE";

            // ask to overwrite or to load if the save exists
            if (FileManager.SaveExists(playerName) && !overwrite)
            {
                this.overwriteMenu = UI.OverwriteMenu(
                    this.spriteBatch,
                    specialElite22,
                    new Vector2(340f, 60f),
                    this.fontColor,
                    this.inActiveFontColor
                    );

                inOverWriteMenu = true;
                secondaryMenuChoice = false;
            }
            else
            {
                this.gameData = new GameData(playerName, overwrite);

                #region Create Test Map
                // Map size
                int x = GraphicsDevice.Viewport.Height / TILE_SIZE;
                int y = GraphicsDevice.Viewport.Width / TILE_SIZE;

                Map testMap = new Map(
                    1,  // Map ID
                    "TEST-MAP",
                    new FlatArray<Tile>(x, y),
                    this.gameData,
                    new int[] { 0 }
                    );

                // Create new GameData
                this.gameData.MapList.Add(testMap);
                #endregion

                // Check for first free coordinates for the PC
                Point freePos = Point.Zero;
                bool outerBreak = false;

                for (int i = 0; i < testMap.Height; i++)
                {
                    for (int j = 0; j < testMap.Width; j++)
                    {
                        if (!testMap.GetTerrain(i, j).IsBlocked)
                        {
                            freePos = new Point(i, j);
                            outerBreak = true;
                            break;
                        }
                    }

                    if (outerBreak)
                        break;
                }

                // Create a Player Character and spawn it on the map
                PC = new Unit(
                    this.gameData,
                    1,
                    "SCiENiDE",
                    "@",
                    Color.LightGreen,
                    testMap.ID,
                    10,
                    freePos.X,
                    freePos.Y);

                PC.IsPlayerControl = true;
                PC.Spawn();

                // Add the player to the Unit List
                this.gameData.UnitList.Add(PC);

                // Create Field of View (shadows)
                this.fieldOfView = new FieldOfView<Tile>(gameData.MapList[PC.MapID].Tiles);
                this.fovSource = new Point(PC.X + 1, PC.Y);

                // Reset waitForAction state
                waitPlayerAction = false;
                // Indicate we are currently playing
                inGame = true;
            }
        }
        
        private void LoadGame()
        {
            throw new NotImplementedException("LoadGame() is not yet implemented!");
            // Reset previous settings/values
            // waitForAction = false;

            //currentMap = gameDataBase.LoadMap(@"../../../maps/0_TEST-MAP[22x36].cumap");

            // . . .
        }

        /// <summary>
        /// Check if any of the keys was pressed. Returns true once per key press.
        /// </summary>
        /// <param name="keysDown">The keys to check for.</param>
        /// <returns>True if any of the keys is down for the first time. Otherwise returns false.</returns>
        private bool CheckKeys(params Keys[] keysDown)
        {
            bool result = false;

            foreach(var key in keysDown)
            {
                if (oldKBState.IsKeyUp(key) &&
                    newKBState.IsKeyDown(key))
                    result = true;
            }
            
            return result;
        }

        /// <summary>
        /// Setup the border box of the game window,
        /// the gameBox and viewBoxTiles point for future use.
        /// </summary>
        private void SetupPredefined()
        {
            this.windowBorders = new List<Rectangle> {
                // top border
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 10),

                // right border
                new Rectangle(GraphicsDevice.Viewport.Width - 10, 0, 10, GraphicsDevice.Viewport.Height),

                // bottom border
                new Rectangle(0, GraphicsDevice.Viewport.Height - 10, GraphicsDevice.Viewport.Width, 10),

                // left border
                new Rectangle(0, 0, 10, GraphicsDevice.Viewport.Height), 

            };
            //(GraphicsDevice.Viewport.Height / TILE_SIZE) * numberOfTiles

            // define gameBox - the game view rectangle
            this.gameBox = new Rectangle(
                10,
                10,
                (GraphicsDevice.Viewport.Width / 3) * 2,
                (GraphicsDevice.Viewport.Height / 4) * 3
                );

            this.viewBoxTiles = new Point(
                gameBox.Height / TILE_SIZE,
                gameBox.Width / TILE_SIZE
                );
        }

        /// <summary>
        /// Save the current game and exit the application.
        /// </summary>
        private void Quit()
        {
            gameData.SaveGameData();

            Exit();
        }

        /* *
        private string AskForName()
        {
            spriteBatch.DrawString(
                specialElite22,
                "Character name:",
                new Vector2(0f, 0f),
                Color.Red   //debug color
                );

            return "";
        }

        private char GetPressedKeyChar()
        {
            char ch = '\0';

            foreach (Keys key in newKBState.GetPressedKeys())
            {
                if (CheckKeys(key))
                {
                    ch = key.ToString()[0];
                }
            }

            return ch;
        }
        * */
    }
}
