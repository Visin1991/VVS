using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.VVS
{
    public static class VVS_Styles
    {
        private static GUIStyle richLabel;
        public static GUIStyle RichLabel
        {
            get
            {
                if (richLabel == null)
                {
                    richLabel = new GUIStyle(EditorStyles.label);
                    richLabel.richText = true;
                }
                return richLabel;
            }
        }


    }

}