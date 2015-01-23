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
        static void Main()
        {
            if (false)
            {
                // JSON Tests
                GameData gData = new GameData("koz");
                Unit test = new Unit(gData, 3, "test", "#", Color.Aqua, 0, 12, 3, 4);

                string str = JsonConvert.SerializeObject(test);

                Unit deserialized = new Unit(gData, JsonConvert.DeserializeObject<Unit>(str));

                str = null;
            }
            else
            {
                using (var game = new GameMain())
                    game.Run();
            }
        }
    }
#endif
}
