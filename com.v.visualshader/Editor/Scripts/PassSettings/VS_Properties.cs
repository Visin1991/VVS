using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using V.VEditorGUI;

namespace V
{
    public class VS_Properties : CollapsGUI
    {
        const float propertyHeight = 60f;

        public VS_Node dragginProperty = null;

        public float dragStartOffsetY = 0;

        public int dragStartIndex;

        public float startMouseY;

        public float DragRectPosY
        {
            get
            {
                return Event.current.mousePosition.y + dragStartOffsetY;
            }
        }

        public override void PreDraw(Rect r)
        {
            

        }


    }
}
