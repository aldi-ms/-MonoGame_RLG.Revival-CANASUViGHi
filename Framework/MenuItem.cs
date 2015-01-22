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
    /// <summary>
    /// Direct successor of the DrawString class adding 2 boolean used for Menu.
    /// </summary>
    public class MenuItem : DrawString
    {
        public bool isActive;
        public bool isOption;

        /// <summary>
        /// Create a Menu Item as a DrawString + boolean isActive & isOption.
        /// </summary>
        /// <param name="font">The SpriteFont used to draw text.</param>
        /// <param name="str">The string we want displayed.</param>
        /// <param name="vector2">Position vector of the string.</param>
        /// <param name="isActive">Is this Menu element active/choosen at the moment?</param>
        /// <param name="isOption">Is this Menu element an option/choice?</param>
        public MenuItem(SpriteFont font, string str, Vector2 vector2, bool isActive, bool isOption)
            : base(font, str, vector2)
        {
            this.isActive = isActive;
            this.isOption = isOption;
        }
    }
}
