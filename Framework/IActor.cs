using System;
using Microsoft.Xna.Framework;

namespace CanasUvighi
{
    public interface IActor
    {
        bool IsPlayerControl { get; set; }
        int Energy { get; set; }
        int Speed { get; }

        bool Move(CardinalDirection dir);
    }
}
