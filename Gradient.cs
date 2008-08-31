using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace VolumeRendering
{
    class GradientSegment : IComparable<GradientSegment>
    {
        public delegate float ComputeBlendFactorFunc(float pos, float middle);
        
        public enum ColorType
        {
            RGB = 0,
            HSV_CCW,
            HSV_CW,
            LOOKUP
        }
        
        public enum SegmentType
        {
            LINEAR = 0,
            CURVED,
            SINE,
            SPHERE_INCREASING,
            SPHERE_DECREASING            
        }

        public GradientSegment(float left, float middle, float right, 
                               Vector4 leftColor, Vector4 rightColor,
                               ColorType colType, SegmentType segFunc)
        {
            _Left = MathHelper.Clamp(left, 0, 1);
            _Right = MathHelper.Clamp(right, 0, 1); ;
            _Middle = MathHelper.Clamp(middle, 0, 1);
            _LeftColor = Vector4.Clamp(leftColor, Vector4.Zero, Vector4.One);
            _RightColor = Vector4.Clamp(rightColor, Vector4.Zero, Vector4.One);
            _ColType = colType;
            _SegType = segFunc;            
        }
        
        public GradientSegment()
        {
            _Left = 0;
            _Right = 1.0f;
            _Middle = 0.5f;
            _LeftColor = Vector4.Zero;
            _RightColor = Vector4.One;
            _ColType = ColorType.RGB;
            _SegType = SegmentType.LINEAR;
        }

        public Color[] LookUpTable
        {
            set 
            {
                _ColType = ColorType.LOOKUP;
                _lookUp = value; 
            }            
        }

        public float Left
        {
            get { return _Left;  }
            set { _Left = MathHelper.Clamp(value, 0, 1.0f); }
        }

        public float Right
        {
            get { return _Right; }
            set { _Right = MathHelper.Clamp(value, 0, 1); }
        }

        public float Middle
        {
            get { return _Middle; }
            set { _Middle = MathHelper.Clamp(value, 0, 1); }
        }

        public Vector4 LeftColor
        {
            get { return _LeftColor; }
            set { _LeftColor = Vector4.Clamp(value, Vector4.Zero, Vector4.One); }
        }

        public Vector4 RightColor
        {
            get { return _RightColor; }
            set { _RightColor = Vector4.Clamp(value, Vector4.Zero, Vector4.One); }
        }

        public ColorType ColType
        {
            get { return _ColType; }
            set { _ColType = value; }
        }

        public SegmentType SegType
        {
            get { return _SegType; }
            set { _SegType = value; }
        }

        /// <summary>
        /// Return true if given position fall into segment
        /// </summary>
        /// <param name="pos">The position to check</param>
        /// <returns>True if into the segment</returns>
        public bool IsInto(float pos)
        {
            return pos >= Left && pos <= _Right;
        }
                
        /// <summary>
        /// Evaluate gradient color at the given position
        /// </summary>
        /// <param name="pos">The position to evaluate</param>
        /// <returns>The computed color</returns>
        public Vector4 Eval(float pos)
        {
            float len = _Right - Left;
            float middle = RelativePos(_Middle);
            pos = RelativePos(pos);

            float factor = ComputeBlendFactor(pos, middle);

            if (_ColType == GradientSegment.ColorType.RGB)
                return _LeftColor * (1 - factor) + factor * _RightColor;

            if (_ColType == GradientSegment.ColorType.LOOKUP)
            {
                int idx = (int)MathHelper.Clamp(_lookUp.Length * factor, 0, 255);
                return _lookUp[idx].ToVector4();                
            }

            Vector4 leftHsv = ColorHelper.RgbToHsv(_LeftColor);
            Vector4 rightHsv = ColorHelper.RgbToHsv(_RightColor);

            float offset = (leftHsv.X < rightHsv.X) ? 0 : 360;
            if (_ColType == GradientSegment.ColorType.HSV_CW)
                offset = (rightHsv.X < leftHsv.X) ? 0 : -360;

            rightHsv.X += offset;
            Vector4 result = leftHsv * (1 - factor) + factor * rightHsv;
            if (result.X > 360.0f)
                result.X -= 360.0f;
            if (result.X < 0.0f)
                result.X += 360.0f;
            return ColorHelper.HsvToRgb(result);
        }

        /// <summary>
        /// Put position between [0,1] relatively to the segement
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>The relative position</returns>
        private float RelativePos(float pos)
        {
            float len = (_Right - Left);
            if (len <= float.Epsilon)            
                return 0.5f;
            return (pos - Left) / len;
        }

        /// <summary>
        /// Compute the factor used for interpolation
        /// </summary>
        /// <param name="pos">The relative current positon</param>
        /// <param name="middle">The relative middle of the segment</param>
        /// <returns>The interpolation factor</returns>
        private float ComputeBlendFactor(float pos, float middle)
        {
            return ComputeFactorFuncs[(int)_SegType](pos, middle);
        }

        /// <summary>
        /// Compute the factor used for a linear gradient 
        /// </summary>
        /// <param name="pos">The relative current positon</param>
        /// <param name="middle">The relative middle of the segment</param>
        /// <returns>The interpolation factor</returns>
        static private float ComputeBlendFactorLinear(float pos, float middle)
        {
            float offset = 0;
            if (pos > middle)
            {
                pos -= middle;
                middle = 1.0f - middle;
                offset = 0.5f;
            }
            if (middle < float.Epsilon)            
                return offset * 2.0f;

            return offset + 0.5f * pos / middle;
        }

        public int CompareTo(GradientSegment seg)
        {
            return _Left.CompareTo(seg._Left);
        }

        static ComputeBlendFactorFunc[] ComputeFactorFuncs = new ComputeBlendFactorFunc[5] { ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear };

        private float _Left;
        private float _Right;
        private float _Middle;
        private Vector4 _LeftColor;
        private Vector4 _RightColor;
        private ColorType _ColType;
        private SegmentType _SegType;
        private Color[] _lookUp;
    }

    class Gradient
    {
        public Gradient()
        {
            _min = 0;
            _max = 1;
        }

        public void AddSegment(GradientSegment seg)
        {
            _segments.Add(seg);
            _segments.Sort();
        }

        /// <summary>
        /// Eval will return (0,0,0,0) for all position inferior to Min and
        /// the gradient will be compressed between Min and Max
        /// </summary>
        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }
        /// <summary>
        /// Eval will return (0,0,0,0) for all position superior to Max
        /// and the gradient willbe compressed between Min and Max
        /// </summary>
        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public Color[] GetColors(uint width)
        {
            Color[] colors = new Color[width];
            float dx = 1.0f / (width - 1);
            float x = 0;
            for (int i = 0; i < width; i++)
            {
                Vector4 c = Eval(x) * 255.0f;
                colors[i] = new Color((byte)c.X, (byte)c.Y, (byte)c.Z, (byte)c.W);                
                x += dx;
            }

            return colors;
        }

        public Vector4 Eval(float pos)
        {
            pos = MathHelper.Clamp(pos, 0, 1);
            if (pos < _min || pos > _max || _min == _max )
                return new Vector4(0, 0, 0, 0);

            pos = (pos - _min) / (_max - _min);
            GradientSegment seg = GetSegment(pos);
            return seg.Eval(pos);
        }

        private GradientSegment GetSegment(float pos)
        {
            int i = _segments.FindIndex(s => s.IsInto(pos));
            if (i < 0)
                throw new System.IndexOutOfRangeException("Position is out of the gradient");
            return _segments[i];
        }
        
        private List<GradientSegment> _segments = new List<GradientSegment>();

        private float _max;
        private float _min;
    }
}
