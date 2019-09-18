using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.VVS
{
    public class VVS_StatusBox
    {
        private VVS_Editor editor;

        VVS_MinMax vCount = new VVS_MinMax();

        VVS_MinMax fCount = new VVS_MinMax();

        VVS_MinMax vtCount = new VVS_MinMax();

        VVS_MinMax ftCount = new VVS_MinMax();

        private GUIStyle labelStyle;
        private GUIStyle labelStyleCentered;
        private GUIStyle holderStyle;
        private GUIStyle headerStyle;

        public VVS_StatusBox()
        {
        }

        public void Initialize(VVS_Editor editor)
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
