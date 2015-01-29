using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanasUvighi
{
    public static class UI
    {
        /// <summary>
        /// Create the main menu.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatck used to draw the Menu.</param>
        /// <param name="titleFont">SpriteFont used for the menu title.</param>
        /// <param name="menuOptionsFont">SpriteFont used for menu options.</param>
        /// <param name="vector">Start vector for the menu.</param>
        /// <param name="inactiveColor">The Color of the not-active/inactive menu items.</param>
        /// <param name="activeColor">The Color of the active menu items.</param>
        /// <returns></returns>
        public static Menu MainMenu(
            SpriteBatch spriteBatch,
            SpriteFont titleFont,
            SpriteFont menuOptionsFont,
            Vector2 vector,
            Color inActiveColor,
            Color activeColor)
        {
            List<MenuItem> menuList = new List<MenuItem>();

            menuList.Add(new MenuItem(titleFont, "CANAS UViGHi", vector, false, false));

            vector.X += 40;
            vector.Y += 80;
            menuList.Add(new MenuItem(menuOptionsFont, "new game", vector, true, true));

            float yStep = 50f;

            vector.Y += yStep;
            menuList.Add(new MenuItem(menuOptionsFont, "load game", vector, false, true));

            vector.Y += yStep;
            menuList.Add(new MenuItem(menuOptionsFont, "options", vector, false, true));

            vector.Y += yStep;
            menuList.Add(new MenuItem(menuOptionsFont, "exit", vector, false, true));

            Menu mainMenu = new Menu(spriteBatch, inActiveColor, activeColor);
            mainMenu.AddMenuItems(menuList);

            return mainMenu;
        }

        /// <summary>
        /// Create the overwrite Menu.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatck used to draw the Menu.</param>
        /// <param name="font">The font used for the menu items.</param>
        /// <param name="vector">Start vector for the menu.</param>
        /// <param name="inactiveColor">The Color of the not-active/inactive menu items.</param>
        /// <param name="activeColor">The Color of the active menu items.</param>
        /// <returns></returns>
        public static Menu OverwriteMenu(
            SpriteBatch spriteBatch, 
            SpriteFont font,
            Vector2 vector,
            Color inActiveColor,
            Color activeColor)
        {
            List<MenuItem> menuList = new List<MenuItem>();
            menuList.Add(new MenuItem(font, "Overwrite existing save?", vector, false, false));

            vector.X += 50f;
            vector.Y += 50f;

            menuList.Add(new MenuItem(font, "yes", vector, false, true));

            vector.Y += 50f;

            menuList.Add(new MenuItem(font, "no (load save)", vector, true, true));

            Menu overwriteMenu = new Menu(spriteBatch, inActiveColor, activeColor);
            overwriteMenu.AddMenuItems(menuList);

            return overwriteMenu;
        }
    }
}
