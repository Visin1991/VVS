using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace V.VEditorGUI
{
    public class DraggableSeparator 
    {
        public DraggableSeparator(Texture2D _texture2D)
        {
            texture2D = _texture2D;
        }

        Texture2D texture2D;

        public bool dragging = false;

        public Rect rect;

        public bool initialized = false;

        int minX;
        public int MinX
        {
            get {
                return minX;
            }
            set {
                minX = value;
                ClampX();
            }
        }

        int maxX;
        public int MaxX
        {
            get
            {
                return maxX;
            }
            set
            {
                maxX = value;
                ClampX();
            }
        }

        void ClampX()
        {
            rect.x = Mathf.Clamp(rect.x, minX, maxX);
        }

        public void Draw(int yPos, int height)
        {
            if(texture2D==null)
            {
                Debug.LogError("DraggableSeparator Draw without Texture2D");
                return;
            }

            rect.y = yPos;
            rect.height = height;
            rect.width = 7;

            GUI.Box(rect, "", EditorStyles.textField);
            Rect rHandle = new Rect(rect);
            rHandle.xMin += 0;
            rHandle.xMax -= 0;
            Rect uv = new Rect(rect);
            uv.x = 0;
            uv.y = 0;
            uv.width = 1;
            uv.height /= texture2D.height;

            GUI.DrawTextureWithTexCoords(rHandle, texture2D, uv);

            if (rect.Contains(Event.current.mousePosition) || dragging)
            {
                V.VEditorGUI.Utility.AssignCursor(rect, MouseCursor.ResizeHorizontal);
            }

            if (Event.current.isMouse)
            {

                if (V.VEditorGUI.Utility.ReleasedRawLMB())
                {
                    StopDrag();
                }
                if (dragging)
                {
                    UpdateDrag();
                }
                if (V.VEditorGUI.Utility.PressedLMB(rect))
                {
                    StartDrag();
                }
            }

        }

        int startDragOffset = 0;

        void StartDrag()
        {
            dragging = true;
            startDragOffset = (int)(Event.current.mousePosition.x - rect.x);
        }
        void UpdateDrag()
        {
            rect.x = Event.current.mousePosition.x - startDragOffset;
            ClampX();
        }
        void StopDrag()
        {
            dragging = false;
        }

    }
}
