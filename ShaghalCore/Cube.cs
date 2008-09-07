using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shaghal
{
    public class Cube
    {
        /**********************************************************************/
        public Cube(GraphicsDevice device, BoundingBox box)
        {
            _bbox = box;
            _graphicDevice = device;
            _vertexDeclaration = new VertexDeclaration(device, VertexPosition.VertexElements);

            Vector3 size = box.Max-box.Min;
            for (int i = 0; i < _cubeVertices.GetLength(0); i++)
            {
                _cubeVertices[i].position = (_cubeVertices[i].position*size)+box.Min;
            }
        }

        /**********************************************************************/
        public void Begin()
        {            
            _graphicDevice.VertexDeclaration = _vertexDeclaration;
        }
        
        /**********************************************************************/
        public void End()
        {
        }

        /**********************************************************************/
        public virtual void Draw()
        {
            _graphicDevice.DrawUserIndexedPrimitives<VertexPosition>(PrimitiveType.TriangleList, _cubeVertices,
                0, _cubeVertices.Length, _cubeIndices, 0, _cubeIndices.Length / 3);
        }

        /**********************************************************************/
        private struct VertexPosition
        {
            Vector3 _position;
            public Vector3 position
            {
                get { return _position; }
                set { _position = value; }
            }

            public static int SizeInBytes
            {
                get { return sizeof(float) * 3 ; }
            }

            public static readonly VertexElement[] VertexElements = new VertexElement[]
            {
                new VertexElement(0, 0, VertexElementFormat.Vector3, 
                                  VertexElementMethod.Default, VertexElementUsage.Position, 0)
            };

            public VertexPosition(Vector3 pos)
            {
                _position = pos;
            }
        };

        /**********************************************************************/
        private GraphicsDevice _graphicDevice;
        private BoundingBox _bbox;

        private VertexPosition[] _cubeVertices = new VertexPosition[8]
        {
            new VertexPosition(new Vector3(0, 0, 0)),
            new VertexPosition(new Vector3(1, 0, 0)),
            new VertexPosition(new Vector3(1, 1, 0)),
            new VertexPosition(new Vector3(0, 1, 0)),
            new VertexPosition(new Vector3(0, 0, 1)),
            new VertexPosition(new Vector3(1, 0, 1)),
            new VertexPosition(new Vector3(1, 1, 1)),
            new VertexPosition(new Vector3(0, 1, 1))
        };

        private readonly short[] _cubeIndices = new short[] { 1, 0, 3, 
                                                              2, 1, 3,
                                                              4, 5, 7,                                                   
                                                              5, 6, 7,
                                                              0, 4, 7,
                                                              0, 7, 3,
                                                              5, 1, 6,
                                                              6, 1, 2,
                                                              0, 1, 5,
                                                              0, 5, 4,
                                                              2, 3, 6,
                                                              6, 3, 7 };

        private VertexDeclaration _vertexDeclaration;
    }
}
