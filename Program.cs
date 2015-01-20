#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
#endregion

namespace CanasUvighi
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            using (var game = new GameMain())
                game.Run();

            /* *
            // JSON Tests
            Tile testTile = new Tile(1, 0, 24, 1);

            string str = JsonConvert.SerializeObject(testTile);

            Tile newColor = JsonConvert.DeserializeObject<Tile>(str);
             * */
        }
    }
#endif
}
