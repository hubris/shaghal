using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VolumeRendering
{
    class TransferFunction
    {
        private Color[] _colors;
        private Color[] _preIntTable = new Color[256*256];
        private readonly float _stepSize = 0.05f;
        private float _sliceDist = 1.0f;

        public Color[] Colors
        {
            set { _colors = value; }
        }

        public Color[] PreIntTable
        {
            get { return _preIntTable; }
        }

        /// <summary>
        /// Compute the preintegrated transfer function
        /// </summary>
        public void preIntegrate()
        {
            for(int front = 0; front < 256; front++)
                for (int back = front; back < 256; back++)
                    _preIntTable[back * 256 + front] = _preIntTable[front * 256 + back] = computeColor(1, front, back);                    
        }

        /// <summary>
        /// Return preintegrated color for segment [front,back]
        /// </summary>
        /// <param name="d">Distance between segment</param>
        /// <param name="front">front voxel value</param>
        /// <param name="back">back voxel value</param>
        /// <returns>Preintegrated segment color</returns>
        Color computeColor(float d, int front, int back)
        {
            Vector4 I = Vector4.Zero;
            float att = 0;
            for (float t = 0; t <= 1; t += _stepSize)
            {
                float s = front * (1 - t) + back * t;
                int colIdx = (int)s;
                att = integrateSegment(t, front, back);                
                I.X += att * _colors[colIdx].R;
                I.Y += att * _colors[colIdx].G;
                I.Z += att * _colors[colIdx].B;
            }
            I *= _stepSize * 1 / 255.0f;
            I.W = 1-att;

            return new Color(I);
        }

        /// <summary>
        /// Compute integrate(TF[lerp(front, back, t)], t, 0, d)
        /// </summary>
        /// <param name="d">Upper bound of the integral</param>
        /// <param name="front">front voxel</param>
        /// <param name="back">back voxel</param>
        /// <returns></returns>
        float integrateSegment(float d, int front, int back)
        {
            float alpha = 0;
            for (float t = 0; t <= d; t += _stepSize)
            {
                float s = front * (1 - t) + back * t;
                int colIdx = (int)s;                
                alpha += _colors[colIdx].A;
            }
            alpha *= _stepSize * 1.0f / 255.0f * _sliceDist;
            return (float)Math.Exp(-alpha);
        }
    }
}
