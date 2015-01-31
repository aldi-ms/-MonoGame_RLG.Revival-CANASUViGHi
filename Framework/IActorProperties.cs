using System;
using Microsoft.Xna.Framework;

namespace CanasUvighi
{
    public interface IActorProperties
    {
        int Energy { get; set; }
        int Speed { get; set; }
        string Name { get; }
        string Visual { get; }
        Color Color { get; }
        int ID { get; }
        int MapID { get; set; }
    }
}
