using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class VS_StatusBox
    {
        private VS_Editor editor;

        VS_MinMax vCount = new VS_MinMax();

        VS_MinMax fCount = new VS_MinMax();

        VS_MinMax vtCount = new VS_MinMax();

        VS_MinMax ftCount = new VS_MinMax();

        private GUIStyle labelStyle;
        private GUIStyle labelStyleCentered;
        private GUIStyle holderStyle;
        private GUIStyle headerStyle;

        public VS_StatusBox()
        {
        }

        public void Initialize(VS_Editor editor)
        {
            this.editor = editor;
            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.margin = new RectOffset(0, 0, 0, 0);
            labelStyle.padding = new RectOffset(8, 0, 3, 1);

            labelStyleCentered = new GUIStyle(labelStyle);
            labelStyleCentered.alignment = TextAnchor.MiddleCenter;

            holderStyle = new GUIStyle();
            holderStyle.margin = new RectOffset(0, 0, 0, 0);
            holderStyle.padding = new RectOffset(0, 0, 0, 0);


            headerStyle = new GUIStyle(EditorStyles.toolbar);
            headerStyle.alignment = TextAnchor.MiddleLeft;
            headerStyle.fontSize = 10;
            //headerStyle.fontStyle = FontStyle.Bold;
        }

        public int OnGUI(int yOffset, int in_maxWidth)
        {
            Rect r = new Rect(0, yOffset, in_maxWidth, 18);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            GUI.Label(r, string.Empty, EditorStyles.toolbar);

            Rect iRect = r;
            iRect.width = 64f;

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            if (GUI.Button(iRect, "Select", EditorStyles.toolbarButton))
            {
                Selection.activeObject = editor.currentShaderAsset;
                EditorGUIUtility.PingObject(editor.currentShaderAsset);
            }
            GUI.color = Color.white;

            return (int)r.yMax;
        }

    }
}
