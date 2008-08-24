using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace VolumeRendering
{
    public static class ColorHelper
    {
        /// <summary>
        /// Convert RGB color to HSV
        /// </summary>
        /// <param name="rgb">RGBA color in [0,1]</param>
        /// <returns>HSV color with h in [0,360] and s,v in [0,1], alpha is not changed</returns>
        static public Vector4 RgbToHsv(Vector4 rgb)
        {

            float max = MathHelper.Max(MathHelper.Max(rgb.X, rgb.Y), rgb.Z);
            float min = MathHelper.Min(MathHelper.Min(rgb.X, rgb.Y), rgb.Z);
            float dist = max - min;

            if (dist == 0)
                return new Vector4(0, 0, max, rgb.W);

            float s = (max < float.Epsilon) ? 0 : 1 - min / max;
            Vector4 hsv = new Vector4(0, s, max, rgb.W);
            if (max == rgb.X)
            {
                hsv.X = (int)(60.0f * (rgb.Y - rgb.Z) / dist) % 360;
            }
            else if (max == rgb.Y)
            {
                hsv.X = 120.0f + (60.0f * (rgb.Z - rgb.X) / dist);
            }
            else if (max == rgb.Z)
            {
                hsv.X = 240.0f + (60.0f * (rgb.X - rgb.Y) / dist);
            }

            if (hsv.X < 0)
                hsv.X += 360.0f;

            return hsv;
        }
        
        /// <summary>
        /// Convert HSV color into RGB color
        /// </summary>
        /// <param name="hsv">HSV color with h in [0,360] and s,v in [0,1]</param>
        /// <returns>RGBA color in [0,1], alpha is not changed</returns>
        static public Vector4 HsvToRgb(Vector4 hsv)
        {            
            int hi = (int)(hsv.X / 60.0f);
            float f = hsv.X / 60.0f - hi;
            float p = hsv.Z * (1.0f - hsv.Y);
            float q = hsv.Z * (1.0f - f * hsv.Y);
            float t = hsv.Z * (1.0f - (1.0f - f) * hsv.Y);
            
            switch (hi % 6)
            {
                case 0:
                    return new Vector4(hsv.Z, t, p, hsv.W);
                case 1:
                    return new Vector4(q, hsv.Z, p, hsv.W);
                case 2:
                    return new Vector4(p, hsv.Z, t, hsv.W);
                case 3:
                    return new Vector4(p, q, hsv.Z, hsv.W);
                case 4:
                    return new Vector4(t, p, hsv.Z, hsv.W);
                case 5:                
                    return new Vector4(hsv.Z, p, q, hsv.W);                
                default:
                    Debug.Assert(false);
                    return new Vector4(0, 0, 0, 0);                                    
            }
        }

    }
}
