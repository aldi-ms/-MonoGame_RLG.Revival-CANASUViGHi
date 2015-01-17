namespace CanasUvighi
{
    /// <summary>
    /// Wrapper around 1-Dimensional array of <typeparamref name="T"/> 
    /// elements to use as a 2-Dimensional array (when and where the 
    /// use of 2D array and fast array performance is needed).
    /// </summary>
    /// <typeparam name="T">Array elements type.</typeparam>
    public class FlatArray<T>
    {
        private int height;
        private int width;
        private T[] array;

        /// <summary>
        /// Create a new 2D (two dimensional) array with specified height and width.
        /// Internally this flattens the array and keeps it as a simple one dimensional
        /// array. 
        /// </summary>
        /// <param name="height">Height of the array / number of rows.</param>
        /// <param name="width">Width of the array / number of columns.</param>
        public FlatArray(int height, int width)
        {
            this.height = height;
            this.width = width;
            this.array = new T[height * width];
        }

        /// <summary>
        /// Access the elements like you would a 2-Dimensional array.
        /// </summary>
        /// <param name="x">X-axis / row.</param>
        /// <param name="y">Y-axis / column.</param>
        /// <returns>The <typeparamref name="T"/> element at given position.</returns>
        public T this[int x, int y]
        {
            get { return this.array[x * this.width + y]; }
            set { this.array[x * this.width + y] = value; }
        }
        
        /// <summary>
        /// The height of the array / number of rows. (X-axis)
        /// </summary>
        public int X
        {
            get { return this.height; }
        }

        /// <summary>
        /// The width of the array / number of columns. (Y-axis)
        /// </summary>
        public int Y
        {
            get { return this.width; }
        }

        /* *
         * For testing purposes
        public int GetRealIndex(int x, int y)
        {
            return x * this.width + y;
        }
        * */
    }
}