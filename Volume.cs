
namespace VolumeRendering
{
    struct Dim3
    {
        private int width;
        private int height;
        private int depth;

        public Dim3(int w, int h, int d)
        {
            width = w;
            height = h;
            depth = d;
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }
    };

    class Volume<T>
    {
        public Volume(T[] data, Dim3 dim)
        {
            _data = data;
            _dim = dim;
        }

        public T[] Data
        {
            get { return _data;  }            
        }

        public Dim3 Dim
        {
            get { return _dim;  }
        }
        
        private T[] _data;
        private Dim3 _dim;
    }
}
