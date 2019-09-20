using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public class ZoomArea
    {
        private static float kEditorWindowTabHeight = 22.0f;
        private static Matrix4x4 prevGuiMatrix;

        public static Rect Begin(float zoomScale, Rect screenCoordsArea, Vector2 cameraPos)
        {
            //Debug.Log("zoomScale : " + zoomScale + " | scrrenCoordsArea : " + screenCoordsArea + " | cameraPos : " + cameraPos);

            GUI.EndGroup();
            kEditorWindowTabHeight = screenCoordsArea.y;
            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
            GUI.BeginGroup(clippedArea);

            prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            Rect offsetRect = screenCoordsArea;
            offsetRect.x -= cameraPos.x;
            offsetRect.y -= cameraPos.y;
            offsetRect.width = int.MaxValue / 2;
            offsetRect.height = int.MaxValue / 2;
            GUI.BeginGroup(offsetRect);

            return clippedArea;
        }

        public static void End(float zoomScale)
        {
            GUI.EndGroup();
            GUI.matrix = prevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
        }

    }

}