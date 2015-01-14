//using System;

//namespace CanasUvighi
//{
//    public class Camera
//    {
//        private int
//            x,
//            y,
//            height,
//            width;

//        #region Constructors
//        /// <summary>
//        /// Create a camera object.
//        /// </summary>
//        /// <param name="x">Camera X (lower left).</param>
//        /// <param name="y">Camera Y (lower left).</param>
//        /// <param name="height">Height - should be odd.</param>
//        /// <param name="width">Width - should be odd.</param>
//        public Camera(int x, int y, int height, int width)
//        {
//            this.x = x;
//            this.y = y;
//            this.height = height;
//            this.width = width;
//        }

//        /// <summary>
//        /// Create a camera object.
//        /// </summary>
//        /// <param name="height">Height - should be odd.</param>
//        /// <param name="width">Width - should be odd.</param>
//        public Camera(int height, int width)
//            : this(0, 0, height, width)
//        { }
//        #endregion

//        #region Properties
//        public int X
//        {
//            get { return this.x; }
//            set { this.x = value; }
//        }

//        public int Y
//        {
//            get { return this.y; }
//            set { this.y = value; }
//        }

//        public int Height
//        {
//            get { return this.height; }
//            set { this.height = value; }
//        }

//        public int Width
//        {
//            get { return this.width; }
//            set { this.width = value; }
//        }
//        #endregion
//    }
//}
