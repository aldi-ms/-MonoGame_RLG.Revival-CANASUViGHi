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
    /// Create a menu to be used/shown to the user.
    /// Has good structure for MenuItems, Next and Previous, get choice of user, etc.
    /// </summary>
    public class Menu
    {
        private SpriteBatch spriteBatch;
        private MenuItem[] menuItems;
        private List<MenuItem> choosableItems;
        private Color inactiveColor = Color.Wheat;
        private Color activeColor = Color.Crimson;

        /// <summary>
        /// Create a menu item.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatck used to draw the Menu.</param>
        /// <param name="inactiveColor">The Color of the not-active/inactive menu items.</param>
        /// <param name="activeColor">The Color of the active menu items.</param>
        public Menu(SpriteBatch spriteBatch, Color inactiveColor, Color activeColor)
        {
            this.spriteBatch = spriteBatch;
            this.inactiveColor = inactiveColor;
            this.activeColor = activeColor;

            this.choosableItems = new List<MenuItem>();
        }

        /// <summary>
        /// Configure the menu with the given menu items.
        /// </summary>
        /// <param name="menuItems">Create menu items from each MenuItem, keeps them ordered.</param>
        public void ConfigureMenu(MenuItem[] menuItems)
        {
            bool oneActive = false;

            foreach (var item in menuItems)
            {
                // Check if there is only 1 (one) active item in the menu, otherwise throw an exception
                if (item.isActive && oneActive)
                {
                    // Change type of exception?
                    throw new ArgumentException("More than one active items in menu. There has to be exactly one.");
                }
                if (item.isActive && !oneActive)
                    oneActive = true;

                // Add items with isOption = true in a separate List<> for easier iteration.
                if (item.isOption)
                    choosableItems.Add(item);
            }

            // If there was no active menu element.
            if (!oneActive)
                throw new ArgumentException("There has to be exactly one active item in menu item list.");

            this.menuItems = menuItems;
        }

        /// <summary>
        /// Draw all menu items or a specific item given by index.
        /// </summary>
        /// <param name="index">Index of specific item to draw, default "-1" for all items.</param>
        public void Draw(int index = -1)
        {
            if (index == -1)
            {
                foreach (var drawStr in menuItems)
                {
                    // Send each string for drawing
                    CustomDrawString(drawStr);
                }
            }
            else
            {
                CustomDrawString(menuItems[index]);
            }
        }

        /// <summary>
        /// Manually change Menu item colors.
        /// </summary>
        /// <param name="inactiveColor">The Color of the not-active/inactive menu items.</param>
        /// <param name="activeColor">The Color of the active menu items.</param>
        public void ChangeMenuColors(Color inactiveColor, Color activeColor)
        {
            this.inactiveColor = inactiveColor;
            this.activeColor = activeColor;
        }

        public int Choose()
        {
            for (int i = 0; i < choosableItems.Count; i++)
            {
                if (choosableItems[i].isActive)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get the previous menu item in order (returns last if active was first).
        /// </summary>
        public void Previous()
        {
            bool activatePrev = false;

            for (int i = choosableItems.Count - 1; i >= 0; i--)
            {
                // this is the item we want to activate
                if (activatePrev)
                {
                    activatePrev = false;

                    choosableItems[i].isActive = true;

                    // and exit for loop, our work here is done.
                    break;
                }

                // Get the active menu item
                if (choosableItems[i].isActive)
                {
                    // Inactivate it
                    choosableItems[i].isActive = false;

                    // Activate the previous menu item
                    activatePrev = true;
                }
            }

            if (activatePrev)
            {
                activatePrev = false;

                this.choosableItems[this.choosableItems.Count - 1].isActive = true;
            }
        }

        /// <summary>
        /// Get the next menu item in order (returns first if active was last).
        /// </summary>
        public void Next()
        {
            bool activateNext = false;

            for (int i = 0; i < choosableItems.Count; i++)
            {
                if (activateNext)
                {
                    activateNext = false;

                    choosableItems[i].isActive = true;
                    break;
                }

                // Get the active menu item
                if (choosableItems[i].isActive)
                {
                    // Inactivate it
                    choosableItems[i].isActive = false;

                    // Activate the next menu item
                    activateNext = true;
                }
            }

            if (activateNext)
            {
                activateNext = false;

                this.choosableItems[0].isActive = true;
            }
        }

        /// <summary>
        /// Draw the string using the specified in/active colors for the menu.
        /// </summary>
        /// <param name="menuItem">The MenuItem element to be drawn.</param>
        private void CustomDrawString(MenuItem menuItem)
        {
            Color col = menuItem.isActive ? this.activeColor : this.inactiveColor;
            this.spriteBatch.DrawString(menuItem.font, menuItem.str, menuItem.vector2, col);
        }
    }
}
