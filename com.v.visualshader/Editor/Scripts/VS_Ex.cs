using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VS_Ex
    {
        public static Rect MovedDown(this Rect r, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                r.y += r.height;
            }
            return r;
        }

        public static Rect PadBottom(this Rect r, int pixels)
        {
            r.yMax -= pixels;
            return r;
        }

        public static Rect PadTop(this Rect r, int pixels)
        {
            r.yMin += pixels;
            return r;
        }

        public static Rect PadRight(this Rect r, int pixels)
        {
            r.xMax -= pixels;
            return r;
        }

        public static Rect PadLeft(this Rect r, int pixels)
        {
            r.xMin += pixels;
            return r;
        }
    }
}
