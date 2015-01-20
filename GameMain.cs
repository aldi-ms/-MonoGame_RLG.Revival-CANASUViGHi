#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
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
        private const int
            TILE_SIZE = 28,
            SCREEN_WIDTH = 1024,
            SCREEN_HEIGHT = 640,

            // minimum energy needed for taking a turn
            ACTION_COST = 100;

        private ulong turns;

        private bool
            inGame = false,
            inMenu = false,
            waitForAction = false;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont
            instruction22,
            K8KurrierFixed20,
            specialElite22,
            consolas12;

        // gameBox is the rectangle containing
        // the actual game field/board
        private Rectangle
            gameBox;
        private List<Rectangle> windowBorders;

        private Map currentMap;

        // Loaded text-file based predefined game objects
        private GameData gameData;

        // The previous (old) and the current (new)
        // keyboard states to check for key presses
        private KeyboardState 
            oldKBState,
            newKBState;

        // Units Player Character
        private Unit PC;

        // list for the rest of the units, iterate over it.
        private List<Unit> unitActors;

        // the main menu
        private Menu mainMenu;

        // colors used for the game
        private Color
            borderColor = new Color(3, 54, 73),
            fontColor = new Color(205, 179, 128),
            backgroundColor = new Color(3, 101, 100);
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
            mainMenu = new Menu(this.spriteBatch, this.fontColor, new Color(232, 221, 203));
            mainMenu.ConfigureMenu(MainMenuItems(this.instruction22, this.specialElite22));

            // Indicates that we are in the game menu.
            inMenu = true;

            // Initialize visual borders.
            SetupWindowBorders();
            
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

            if (CheckKeys(Keys.Escape))
            {
                if (inMenu)
                {
                    Quit();
                }
                if (inGame)
                {
                    inGame = false;
                    inMenu = true;
                }
            }

            #region In Menu
            if (inMenu)
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

                // Enter || Space
                if (CheckKeys(Keys.Enter, Keys.Space))
                {
                    inMenu = false; 

                    switch (this.mainMenu.Choose())
                    {
                        case 0:
                            NewGame(); 
                            break;

                        case 3:
                            Quit(); 
                            break;

                        default:
                            inMenu = true; 
                            break;
                    }
                }
            }
            #endregion

            #region In Game
            if (inGame)
            {
                // Spawn test NPC unit
                if (CheckKeys(Keys.Q))
                {
                    Unit npc = new Unit(2, "npc_test", "q", Color.Blue, this.currentMap, 15, 0, 0);
                    this.unitActors.Add(npc);
                    npc.Spawn();
                }
                
                // first save number of units/actors that have energy >= TURN_COST
                // and go to next turn after all of them have take action
                foreach(Unit actor in unitActors)
                {
                    if (!waitForAction)
                        actor.Energy += actor.Speed;

                    if (actor.Energy >= ACTION_COST)
                    {
                        if (actor.IsPlayerControl)
                        {
                            waitForAction = true;

                            // Actor / Unit is Player Controlled
                            #region Take action
                            // South
                            if (CheckKeys(Keys.Down, Keys.NumPad2, Keys.J))
                            {
                                if (actor.Move(CardinalDirection.South))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // North
                            if (CheckKeys(Keys.Up, Keys.NumPad8, Keys.K))
                            {
                                if (actor.Move(CardinalDirection.North))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // West
                            if (CheckKeys(Keys.Left, Keys.NumPad4, Keys.H))
                            {
                                if (actor.Move(CardinalDirection.West))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // East
                            if (CheckKeys(Keys.Right, Keys.NumPad6, Keys.L))
                            {
                                if (actor.Move(CardinalDirection.East))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // South West
                            if (CheckKeys(Keys.B, Keys.NumPad1))
                            {
                                if (actor.Move(CardinalDirection.SouthWest))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // South East
                            if (CheckKeys(Keys.N, Keys.NumPad3))
                            {
                                if (actor.Move(CardinalDirection.SouthEast))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // North West
                            if (CheckKeys(Keys.Y, Keys.NumPad7))
                            {
                                if (actor.Move(CardinalDirection.NorthWest))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }

                            // North East
                            if (CheckKeys(Keys.U, Keys.NumPad9))
                            {
                                if (actor.Move(CardinalDirection.NorthEast))
                                {
                                    this.turns++;
                                    waitForAction = false;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            if (!waitForAction)
                            {
                                // Actor / Unit is Non-Player Controlled
                                actor.Move(AI.DrunkardWalk());
                            }
                        }
                    }
                }

            #endregion
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
            else if (inMenu)
                GraphicsDevice.Clear(borderColor);

            spriteBatch.Begin();
            
            // Define basic Texture2D element for drawing
            Texture2D simpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            simpleTexture.SetData<Color>( new[] { Color.White } );

            if (inMenu)
            {
                // Begin with main menu
                mainMenu.Draw();
            }
            else if (inGame)
            {
                // This is the gameBox background
                spriteBatch.Draw(simpleTexture, gameBox, backgroundColor);

                #region Draw grid
                /* *
                bool drawGrid = true;
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
                * */
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

                int viewBoxTilesHeight = gameBox.Height / TILE_SIZE,
                    viewBoxTilesWidth = gameBox.Width / TILE_SIZE;

                for (int x = 0; x < viewBoxTilesHeight; x++)
                {
                    for (int y = 0; y < viewBoxTilesWidth; y++)
                    {
                        Vector2 vect = new Vector2(14 + y * TILE_SIZE, 10 + x * TILE_SIZE);

                        spriteBatch.DrawString(
                            K8KurrierFixed20,
                            currentMap.GetTileVisual(x, y),
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
                debugInfo = //Newtonsoft.Json.JsonConvert.SerializeObject(this.map.GetTerrain(PC.X, PC.Y).ToJSONTerrain());
                    //string.Format("row:{0};col:{1};ttl_time:{2}", this.PC.X, this.PC.Y, gameTime.TotalGameTime);
                string.Format("[{0};{1}] energy:{2}; turns:{3}", PC.X, PC.Y, PC.Energy, turns);
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
        private void NewGame()
        {
            // Load game data first of all (DB).
            gameData = new GameData("SCiENiDE");
            // initialize a list for the units
            unitActors = new List<Unit>(gameData.NPCList);

            // Reset previous settings/values
            waitForAction = false;
            unitActors.Clear();

            #region Create Test Map
            // Map size
            int x = GraphicsDevice.Viewport.Height / TILE_SIZE;
            int y = GraphicsDevice.Viewport.Width / TILE_SIZE;

            currentMap = new Map(
                0,  // ID
                "TEST-MAP",
                new FlatArray<Tile>(x, y),
                this.gameData,
                new int[] { 0, 1 }
                );
            #endregion

            // check for first free coordinates for PC
            int pcX = 0,
                pcY = 0;
            bool outerBreak = false;
            for (int i = 0; i < currentMap.Height; i++)
            {
                for (int j = 0; j < currentMap.Width; j++)
                {
                    if (!currentMap.GetTerrain(i, j).IsBlocked)
                    {
                        pcX = i;
                        pcY = j;
                        outerBreak = true;
                        break;
                    }
                }

                if (outerBreak) 
                    break;
            }

            // Create a Player Character and spawn it on the map
            PC = new Unit(1, "SCiENiDE", "@", Color.LightGreen, this.currentMap, 10, pcX, pcY);
            PC.IsPlayerControl = true;
            PC.Spawn();
            // Add PC to the actor list
            unitActors.Add(PC);
            
            // Indicate we are currently playing (in game)
            inGame = true;
        }      
        
        private void LoadGame()
        {
            // Reset previous settings/values
            waitForAction = false;
            unitActors.Clear();

            //currentMap = gameDataBase.LoadMap(@"../../../maps/0_TEST-MAP[22x36].cumap");

            // . . .
        }

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

        /// <summary>
        /// Setup the border box of the game window as well as the gameBox.
        /// </summary>
        private void SetupWindowBorders()
        {
            windowBorders = new List<Rectangle> {
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
            gameBox = new Rectangle(
                10,
                10,
                (GraphicsDevice.Viewport.Width / 3) * 2,
                (GraphicsDevice.Viewport.Height / 4) * 3
                );
        }

        // change menu items load from file (with JSON strings).
        /// <summary>
        /// Create/Load menu items for the main menu.
        /// </summary>
        /// <param name="titleFont">SpriteFont used for the menu title.</param>
        /// <param name="menuOptionsFont">SpriteFont used for menu options.</param>
        /// <returns>Array of pre-defined MenuItems, ready for display.</returns>
        private MenuItem[] MainMenuItems(SpriteFont titleFont, SpriteFont menuOptionsFont)
        {
            MenuItem[] menuItems = 
            {
                new MenuItem(
                    titleFont,
                    "CANAS UViGHi",
                    new Vector2(20f, 10f),
                    false,
                    false),

                new MenuItem(
                    menuOptionsFont,
                    "new game",
                    new Vector2(80f, 100f),
                    true,
                    true),

                new MenuItem(
                    menuOptionsFont,
                    "load game",
                    new Vector2(80f, 150f),
                    false,
                    true),

                new MenuItem(
                    menuOptionsFont,
                    "options",
                    new Vector2(80f, 200f),
                    false,
                    true),

                new MenuItem(
                    menuOptionsFont,
                    "exit",
                    new Vector2(80f, 250f),
                    false,
                    true)
            };

            return menuItems;
        }

        /// <summary>
        /// Save the current game and exit the application.
        /// </summary>
        private void Quit()
        {
            gameData.SaveGameData();

            Exit();
        }
    }
}
