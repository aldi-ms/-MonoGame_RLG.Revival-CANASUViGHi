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
    /// A basic element for keeping just enough data for a text element to draw.
    /// </summary>
    public class DrawString
    {
        public SpriteFont font;
        public string str;
        public Vector2 vector2;

        /// <summary>
        /// Create a new element for future work on / drawing.
        /// </summary>
        /// <param name="font">The SpriteFont used to draw this text.</param>
        /// <param name="str">The text we want drawn.</param>
        /// <param name="vector2">The position of the text (start drawing from here).</param>
        public DrawString(SpriteFont font, string str, Vector2 vector2)
        {
            this.font = font;
            this.str = str;
            this.vector2 = vector2;
        }
    }
}
