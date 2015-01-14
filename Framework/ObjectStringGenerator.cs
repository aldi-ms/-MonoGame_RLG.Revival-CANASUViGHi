using System;
using Newtonsoft.Json;
using CanasUvighi;

namespace CanasUvighi.ObjectGenerator
{
    public static class ObjectStringGenerator
    {
        public static string Serialize(Terrain terrain)
        {
            return JsonConvert.SerializeObject(terrain);
        }
    }
}
