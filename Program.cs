#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using CanasUvighi.ObjectGenerator;
using Newtonsoft.Json;
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

            /* JSON tests
            Terrain testTerrain = new Terrain(1, "wall", "#", false);
            JSONTerrain jsonTerrain = testTerrain.ToJSONTerrain();

            string str = JsonConvert.SerializeObject(jsonTerrain);
            Console.WriteLine(str);

            JSONTerrain newTestTerrain = JsonConvert.DeserializeObject<JSONTerrain>(str);
            Terrain final = newTestTerrain.ToTerrain();
            */
        }
    }
#endif
}
