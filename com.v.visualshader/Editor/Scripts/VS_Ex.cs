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



        public static Vector2 TopLeft(this Rect r)
        {
            return new Vector2(r.x, r.y);
        }

        public static Vector2 TopRight(this Rect r)
        {
            return new Vector2(r.xMax, r.y);
        }

        public static Vector2 BottomRight(this Rect r)
        {
            return new Vector2(r.xMax, r.yMax);
        }

        public static Vector2 BottomLeft(this Rect r)
        {
            return new Vector2(r.x, r.yMax);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;

            return result;
        }

        public static Rect Margin(this Rect r, int pixels)
        {
            r.xMax += pixels;
            r.xMin -= pixels;
            r.yMax += pixels;
            r.yMin -= pixels;
            return r;
        }

        public static Rect Pad(this Rect r, int pixels)
        {
            return r.Margin(-pixels);
        }

    }
}
