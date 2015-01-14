namespace CanasUvighi
{
    /// <summary>
    /// FlatArray is a wrapper around 1D array of <typeparamref name="T"/> 
    /// elements and shows it as easy to visualize 2D array (when and where the 
    /// use of 2D array and fast array performance is needed).
    /// </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
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
        /// Access the elements like you would a 2d array.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <returns></returns>
        public T this[int x, int y]
        {
            get { return this.array[x * this.width + y]; }
            set { this.array[x * this.width + y] = value; }
        }

        public int GetRealIndex(int x, int y)
        {
            return x * this.width + y;
        }

        /// <summary>
        /// The height of the array / number of rows.
        /// </summary>
        public int Height
        {
            get { return this.height; }
        }

        /// <summary>
        /// The width of the array / number of columns.
        /// </summary>
        public int Width
        {
            get { return this.width; }
        }
    }
}