using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VolumeRendering
{
    struct GradientSegment
    {
        public delegate float ComputeBlendFactorFunc(float pos, float middle);
        
        public enum ColorType
        {
            RGB = 0,
            HSV_CCW,
            HSV_CW,
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
            Left = MathHelper.Clamp(left, 0, 1);
            Right = MathHelper.Clamp(right, 0, 1); ;
            Middle = MathHelper.Clamp(middle, 0, 1);
            LeftColor = Vector4.Clamp(leftColor, Vector4.Zero, Vector4.One);
            RightColor = Vector4.Clamp(rightColor, Vector4.Zero, Vector4.One);
            ColType = colType;
            SegType = segFunc;            
        }

        //<summary>Return true if pos is between Left and Right </summary>
        public bool IsInto(float pos)
        {
            return pos >= Left && pos <= Right;
        }

        //<summry>Evaluate gradient color at the given pos</summary>
        public Vector4 Eval(float pos)
        {
            float len = Right - Left;
            float middle = RelativePos(Middle);
            pos = RelativePos(pos);

            float factor = ComputeBlendFactor(pos, middle);

            if (ColType == GradientSegment.ColorType.RGB)
                return LeftColor * (1 - factor) + factor * RightColor;

            Vector4 leftHsv = ColorHelper.RgbToHsv(LeftColor);
            Vector4 rightHsv = ColorHelper.RgbToHsv(RightColor);

            float offset = (leftHsv.X < rightHsv.X) ? 0 : 360;
            if (ColType == GradientSegment.ColorType.HSV_CW)
                offset = (rightHsv.X < leftHsv.X) ? 0 : -360;

            rightHsv.X += offset;
            Vector4 result = leftHsv * (1 - factor) + factor * rightHsv;
            if (result.X > 360.0f)
                result.X -= 360.0f;
            if (result.X < 0.0f)
                result.X += 360.0f;
            return ColorHelper.HsvToRgb(result);
        }

        private float RelativePos(float pos)
        {
            float len = (Right - Left);
            if (len <= float.Epsilon)            
                return 0.5f;
            return (pos - Left) / len;
        }

        private float ComputeBlendFactor(float pos, float middle)
        {
            return ComputeFactorFuncs[(int)SegType](pos, middle);
        }

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

        static ComputeBlendFactorFunc[] ComputeFactorFuncs = new ComputeBlendFactorFunc[5] { ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear, ComputeBlendFactorLinear };

        private float Left;
        private float Right;
        private float Middle;
        private Vector4 LeftColor;
        private Vector4 RightColor;
        private ColorType ColType;
        private SegmentType SegType;
    }

    class Gradient
    {
        public void AddSegment(GradientSegment seg)
        {
            _segments.Add(seg);
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
    }
}
