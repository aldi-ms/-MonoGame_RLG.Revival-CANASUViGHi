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
    public class MenuItem
    {
        public bool IsActive { get; set; }
        public bool IsOption { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Vector { get; set; }

        /// <summary>
        /// Create a Menu Item.
        /// </summary>
        /// <param name="font">The SpriteFont used to draw text.</param>
        /// <param name="str">The string we want displayed.</param>
        /// <param name="vector2">Position vector of the string.</param>
        /// <param name="isActive">Is this Menu element active/choosen at the moment?</param>
        /// <param name="isOption">Is this Menu element an option/choice?</param>
        public MenuItem(SpriteFont font, string str, Vector2 vector, bool isActive, bool isOption)
        {
            this.Font = font;
            this.Text = str;
            this.Vector = vector;
            this.IsActive = isActive;
            this.IsOption = isOption;
        }
    }
}
