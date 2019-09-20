using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.VEditorGUI
{
    public static class VStyles 
    {

        private static GUIStyle nodeStyle;
        public static GUIStyle NodeStyle
        {
            get
            {
                if (nodeStyle == null)
                {
                    //if( Application.unityVersion.StartsWith( "4" ) )
                    nodeStyle = new GUIStyle((GUIStyle)"flow node 0");
                    nodeStyle.alignment = TextAnchor.UpperCenter;
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                        nodeStyle.fontSize = 9;
                    else
                        nodeStyle.fontSize = 11;
                    nodeStyle.font = EditorStyles.standardFont;
                    nodeStyle.fontStyle = FontStyle.Bold;
                    nodeStyle.padding.top = 23;
                    nodeStyle.padding.left = 1;
                    //nodeStyle.margin.right = 8;
                    //nodeStyle.border.right = 25;
                    nodeStyle.border.left = 25;
                    if (EditorGUIUtility.isProSkin)
                        nodeStyle.normal.textColor = new Color(1f, 1f, 1f, 0.75f);
                    else
                        nodeStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);

                }

                return nodeStyle;
            }
        }


        private static GUIStyle nodeStyleDiscrete;
        public static GUIStyle NodeStyleDiscrete
        {
            get
            {
                if (nodeStyleDiscrete == null)
                {
                    nodeStyleDiscrete = new GUIStyle(NodeStyle);
                    nodeStyleDiscrete.normal.textColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.75f / 5f) : new Color(0f, 0f, 0f, 0.7f / 5f);
                }
                return nodeStyleDiscrete;
            }
        }
    }
}
