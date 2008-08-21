using System;

namespace VolumeRendering
{
    class Volume<T>
    {
        public Volume(T[,,] data)
        {
            _data = data;
        }
        
        private T[,,] _data;
    }
}
