using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.VVS
{
    public class VVS_PassSettings : ScriptableObject
    {
        public VVS_Editor editor;

        Rect innerScrollRect = new Rect(0, 0, 0, 0);
        Vector2 scrollPos;

        float targetScrollWidth = 0f;
        float currentScrollWidth = 0f;

        public int maxWidth;

        public bool guiChanged = false;


        public List<VVS_PS_Category> cats;
        public VVS_PS_Meta catMeta;


        public VVS_PassSettings()
        {

        }

        public VVS_PassSettings Initialize(VVS_Editor materialEditor)
        {

            this.editor = materialEditor;
            //fChecker = ScriptableObject.CreateInstance<SF_FeatureChecker>().Initialize(this, materialEditor);

            cats = new List<VVS_PS_Category>();
            catMeta = (VVS_PS_Meta)ScriptableObject.CreateInstance<VVS_PS_Meta>().Initialize(editor,this,"Shader Settings");
            catMeta.Initialize(editor,this, "Shader Settings");       
            cats.Add(catMeta);

            return this;
        }


        public int OnLocalGUI(int yOffset, int in_maxWidth)
        {
            if (Event.current.type == EventType.Repaint)
                currentScrollWidth = Mathf.Lerp(currentScrollWidth, targetScrollWidth, 0.3f);

            this.maxWidth = in_maxWidth;
            Rect scrollRectPos = new Rect(0f, yOffset, in_maxWidth, Screen.height / EditorGUIUtility.pixelsPerPoint - yOffset - 20);
            bool useScrollbar = (innerScrollRect.height > scrollRectPos.height);
            targetScrollWidth = useScrollbar ? 15 : 0;

            int scrollBarWidth = (int)currentScrollWidth;

            innerScrollRect.width = in_maxWidth - scrollBarWidth;

            guiChanged = false;

            int offset = 0;

            if (innerScrollRect.height < scrollRectPos.height)
                innerScrollRect.height = scrollRectPos.height;

            this.maxWidth -= scrollBarWidth;

            int scrollPad = scrollBarWidth - 15;
            GUI.BeginGroup(scrollRectPos);
            Rect scrollWrapper = scrollRectPos;
            scrollWrapper.x = 0;
            scrollWrapper.y = 0; // Since it's grouped

            scrollPos = GUI.BeginScrollView(scrollWrapper.PadRight(scrollPad), scrollPos, innerScrollRect, false, true);

            {
                //bool showErrors = editor.nodeView.treeStatus.Errors.Count > 0;

                offset = catMeta.Draw(offset);
                offset = GUISeparator(offset); // ----------------------------------------------
            }
            GUI.EndScrollView();
            GUI.EndGroup();
            this.maxWidth += scrollBarWidth;

            if (guiChanged)
            {
                editor.ps = this;
                editor.OnShaderModified();
            }

            innerScrollRect.height = offset;
            return offset;
        }

        public int GUISeparator(int yOffset)
        {
            GUI.Box(new Rect(0, yOffset, maxWidth, 1), "", EditorStyles.textField);
            return yOffset + 1;
        }


        private bool prevChangeState;
        public void StartIgnoreChangeCheck()
        {
            prevChangeState = EditorGUI.EndChangeCheck(); // Don't detect changes when toggling
        }

        public void EndIgnoreChangeCheck()
        {
            EditorGUI.BeginChangeCheck(); // Don't detect changes when toggling
            if (prevChangeState)
            {
                GUI.changed = true;
            }
        }

    }
}
