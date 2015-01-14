﻿#region Using Statements
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
    /// This is the main type for the game
    /// </summary>
    public class GameMain : Game
    {
        #region Variables
        // Game window resolution
        // 1024 x 640 is 16:10 aspect ratio
        private const int
            TILE_SIZE = 28,
            SCREEN_WIDTH = 1024,
            SCREEN_HEIGHT = 640,

            // minimum energy needed for taking a turn
            TURN_COST = 100;


        private ulong turns;

        private bool
            inGame = false,
            inMenu = false;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont
            instruction22,
            K8KurrierFixed20,
            specialElite22,
            consolas12;
        /* *
            GOSTTypeA30,
            GOSTTypeA60,
            consolas16,
            square10,
            square18,
         * */

        // Game view elements
        private Rectangle
            gameBox;
        private List<Rectangle> windowBorders;
        private Map map;
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
            // Load game data first of all (DB).
            this.gameData = new GameData();

            // Create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Font Load
            /* *
            GOSTTypeA30 = Content.Load<SpriteFont>("GOST_type_A30");
            GOSTTypeA60 = Content.Load<SpriteFont>("GOST_type_A60");
            square10 = Content.Load<SpriteFont>("square10");
            GOSTTypeA30 = Content.Load<SpriteFont>("GOST_type_A30");
            consolas16 = Content.Load<SpriteFont>("Consolas16");
            square18 = Content.Load<SpriteFont>("Square18");
             * */

            consolas12 = Content.Load<SpriteFont>("Consolas12");                // Debug
            instruction22= Content.Load<SpriteFont>("Instruction22");           // Headlines. No lowcap letters.
            K8KurrierFixed20 =  Content.Load<SpriteFont>("K8KurierFixed20");    // Great Courier. Use for map/visual characters.
            specialElite22 = Content.Load<SpriteFont>("SpecialElite22");        // Objectives, text, explanations, etc.


            /*
             * Old Fonts (implement font loading
             * & more fonts for UI extensibility/configurability)
             * 
            pericles18 = Content.Load<SpriteFont>("pericles18");
            captureIt42 = Content.Load<SpriteFont>("capture_it42");
            sketchFlow50 = Content.Load<SpriteFont>("SketchFlow_Print50");
            GOSTTypeA50 = Content.Load<SpriteFont>("GOST_type_A50");
            GtekNova60 = Content.Load<SpriteFont>("GtekNova60");
             */
            #endregion

            // Create and configure a Menu, which will be used as the Game's Main Menu
            this.mainMenu = new Menu(this.spriteBatch, this.fontColor, new Color(232, 221, 203));
            this.mainMenu.ConfigureMenu(this.MainMenuItems());
            // Indicates that we are in the game menu.
            this.inMenu = true;

            // Initialize visual borders.
            SetupBorderRectangles();

            // initialize a list for the units
            this.unitActors = new List<Unit>();

            // get a state for the oldState of keyboard
            this.oldKBState = Keyboard.GetState();
                        
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

            // Get the current KB state
            this.newKBState = Keyboard.GetState();

            if (CheckKeys(Keys.Escape))
            {
                if (inMenu)
                {
                    this.Quit();
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
                // Down
                if (CheckKeys(Keys.Down))
                {
                    this.mainMenu.Next();
                }

                // Up
                if (CheckKeys(Keys.Up))
                {
                    this.mainMenu.Previous();
                }

                // Enter
                if (CheckKeys(Keys.Enter))
                {
                    inMenu = false; 
                    int choice = this.mainMenu.Choose();

                    switch (choice)
                    {
                        case 0: 
                            this.NewGame(); 
                            break;
                        case 3:
                            Quit(); 
                            break;
                        default:
                            this.inMenu = true; 
                            break;
                    }
                }
            }
            #endregion

            #region In Game
            if (this.inGame)
            {
                // Spawn npc unit
                if (CheckKeys(Keys.Q))
                {
                    Unit npc = new Unit(2, "npc_test", "q", Color.Blue, this.map, 15, 0, 0);
                    this.unitActors.Add(npc);
                    npc.Spawn();
                }

                bool waitForAction = false;

                // first save number of units/actors that have energy >= TURN_COST
                // and go to next turn after all of them have take action
                foreach(Unit actor in unitActors)
                {
                    actor.Energy += actor.Speed;

                    if (actor.Energy >= TURN_COST)
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
                                actor.Move((CardinalDirection)AI.DrunkardWalk());
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

            // Set window color
            if (inGame)
                GraphicsDevice.Clear(Color.Black);
            else if (inMenu)
                GraphicsDevice.Clear(new Color(3, 54, 73));

            this.spriteBatch.Begin();
            
            // Define basic Texture2D element for drawing
            Texture2D simpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            simpleTexture.SetData<Color>( new[] { Color.White } );

            if (inMenu)
            {
                // Begin with main menu
                this.mainMenu.Draw();
            }
            else if (inGame)
            {
                // This is the gameBox background
                this.spriteBatch.Draw(simpleTexture, this.gameBox, this.backgroundColor);

                #region Draw grid
                /*
                if (drawGrid)
                {
                    for (int rows = this.gameBox.Y; rows < this.gameBox.Y + this.gameBox.Height; rows += TILE_SIZE)
                    {
                        var tempRect = new Rectangle(this.gameBox.X, rows, this.gameBox.Width, 1);
                        this.spriteBatch.Draw(simpleTexture, tempRect, Color.DarkSlateGray);
                    }
                    for (int cols = this.gameBox.X; cols < this.gameBox.Y + this.gameBox.Width; cols += TILE_SIZE)
                    {
                        var tempRect = new Rectangle(cols, this.gameBox.Y, 1, this.gameBox.Height);
                        this.spriteBatch.Draw(simpleTexture, tempRect, Color.DarkSlateGray);
                    }
                }
                */
                #endregion

                #region Draw window & gameBox borders
                // draw the window borders
                for (int i = 0; i < windowBorders.Count; i++)
                {
                    this.spriteBatch.Draw(simpleTexture, windowBorders[i], this.borderColor);
                }

                // gameBox left border
                this.spriteBatch.Draw(simpleTexture, new Rectangle(this.gameBox.Width, 10, 10, this.gameBox.Height), this.borderColor);

                // gameBox bottom border
                this.spriteBatch.Draw(simpleTexture, new Rectangle(10, this.gameBox.Height + 10, this.gameBox.Width, 10), this.borderColor);
                #endregion

                #region Draw Map

                int viewBoxTilesHeight = this.gameBox.Height / TILE_SIZE,
                    viewBoxTilesWidth = this.gameBox.Width / TILE_SIZE;

                for (int x = 0; x < viewBoxTilesHeight; x++)
                {
                    for (int y = 0; y < viewBoxTilesWidth; y++)
                    {
                        Vector2 vect = new Vector2(14 + y * TILE_SIZE, 10 + x * TILE_SIZE);

                        this.spriteBatch.DrawString(
                            this.K8KurrierFixed20,
                            this.map.GetTileVisual(x, y),
                            vect,
                            this.fontColor);
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
                string.Format("row:{0};col:{1};turns:{2}", this.PC.X, this.PC.Y, this.turns);
                //string.Format("x:{0},y:{1}", mouse.X, mouse.Y);
                //string.Format("gameBox[x:{0};y:{1};width:{2};height{3};]", gameBox.X, gameBox.Y, gameBox.Width, gameBox.Height);
            }

            this.spriteBatch.DrawString(consolas12, debugInfo, new Vector2(15, GraphicsDevice.Viewport.Height - 32), this.fontColor);
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

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Start a new game.
        /// </summary>
        private void NewGame()
        {
            // map size
            int x = GraphicsDevice.Viewport.Height / TILE_SIZE;
            int y = GraphicsDevice.Viewport.Width / TILE_SIZE;

            this.map = new Map(
                "Testing map", 
                new FlatArray<Tile>(x, y), 
                new GameData(),
                true
                );

            // check for first free coordinates for PC
            int pcX = 0, pcY = 0;
            bool leave = false;
            for (int i = 0; i < map.Height; i++)
            {
                for (int j = 0; j < map.Width; j++)
                {
                    if (!this.gameData.TerrainDB[this.map.Tiles[i, j].Terrain].IsBlocked) 
                    {
                        pcX = i;
                        pcY = j;
                        leave = true;
                        break;
                    }
                }
                if (leave) break;
            }

            // create a Player Character and spawn it on the map
            this.PC = new Unit(1, "SCiENiDE", "@", Color.LightGreen, this.map, 10, pcX, pcY);
            this.PC.MakePlayerControl();
            this.PC.Spawn();
            // add PC to the unit list
            this.unitActors.Add(this.PC);

            // indicate we are currently playing (in game)
            this.inGame = true;
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
                if (this.oldKBState.IsKeyUp(key) &&
                    this.newKBState.IsKeyDown(key))
                    result = true;
            }
            
            return result;
        }

        /// <summary>
        /// Setup the border box of the game window.
        /// </summary>
        private int SetupBorderRectangles()
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

            return this.windowBorders.Count;
        }


        // change menu items load from file (with JSON strings).
        private MenuItem[] MainMenuItems()
        {
            SpriteFont titleFont = this.instruction22,
                menuOptionsFont = this.specialElite22;
            MenuItem[] menuItems = 
            {
                new MenuItem(
                    titleFont,
                    "CANAS UViGHi",
                    new Vector2(20f, 10f),
                    false,
                    false
                    ),

                new MenuItem(
                    menuOptionsFont,
                    "new game",
                    new Vector2(80f, 100f),
                    true,
                    true
                    ),

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
            this.gameData.SaveToFiles();
            Exit();
        }
    }
}
