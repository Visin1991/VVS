using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace V.VVS
{
    public class VVS_DraggableSeparator 
    {
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
            uv.height /= VVS_GUI.Handle_drag.height;

            GUI.DrawTextureWithTexCoords(rHandle, VVS_GUI.Handle_drag, uv);

            if (rect.Contains(Event.current.mousePosition) || dragging)
            {
                VVS_GUI.AssignCursor(rect, MouseCursor.ResizeHorizontal);
            }

            if (Event.current.isMouse)
            {

                if (VVS_GUI.ReleasedRawLMB())
                {
                    StopDrag();
                }
                if (dragging)
                {
                    UpdateDrag();
                }
                if (VVS_GUI.PressedLMB(rect))
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
