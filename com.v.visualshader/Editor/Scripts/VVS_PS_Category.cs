using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace V.VVS
{
    public class VVS_PS_Category : ScriptableObject
    {
        public VVS_Editor editor;
        public VVS_PassSettings ps;

        public string labelExpanded;
        public string labelContracted;

        public bool expanded = false;
        public float targetHeight = 0f;
        public float smoothHeight = 0f;

        public VVS_PS_Category Initialize(VVS_Editor editor, VVS_PassSettings ps, string label)
        {
            this.editor = editor;
            this.ps = ps;
            this.labelExpanded = label;
            this.labelContracted = label + "...";
            return this;
        }

        public virtual void ToggleExpansion()
        {

        }

        public virtual void PreDraw(Rect r)
        {

        }

        public int Draw(float yOffset)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (Mathf.Abs(smoothHeight - targetHeight) > 0.1f)
                    smoothHeight = Mathf.Lerp(smoothHeight, targetHeight, 0.5f);
                else
                    smoothHeight = targetHeight;
            }

            Rect topRect = new Rect(0f, yOffset, ps.maxWidth, 20);
            Rect r = new Rect(topRect);

            PreDraw(r);


            if (!StartExpanderChangeCheck(r, ref expanded, labelContracted, labelExpanded))
            {
                targetHeight = 0f;
            }

            Rect gRect = r;
            gRect.height = smoothHeight + 20;
            GUI.BeginGroup(gRect);
            //----------------------------------------------------
            yOffset = DrawInner(ref r);
            //----------------------------------------------------
            GUI.EndGroup();

            if (expanded)
                targetHeight = yOffset - topRect.yMax;
            if (EndExpanderChangeCheck())
                ps.guiChanged = true;

            GUI.color = Color.white;
            return (int)(topRect.yMax + smoothHeight);
        }

        public virtual float DrawInner(ref Rect r)
        {

            return 0f;
        }

        public bool StartExpanderChangeCheck(Rect r, ref bool foldVar, string labelContracted, string labelExpanded)
        {

            // TOOD: COLOR RECT BEHIND
            Color prev = GUI.color;
            GUI.color = new Color(0, 0, 0, 0);
            if (GUI.Button(r, string.Empty, EditorStyles.foldout))
            {
                Event.current.Use();
                Undo.RecordObject(this, foldVar ? "collapse " + labelExpanded : "expand " + labelExpanded);
                foldVar = !foldVar;
            }
            GUI.color = prev;
         
            EditorGUI.Foldout(r, foldVar, GetLabelString());
            DrawExtraTitleContent(r);

            EditorGUI.BeginChangeCheck();
            if (!foldVar)
                return false;
            return true;
        }

        public bool EndExpanderChangeCheck()
        {
            return EditorGUI.EndChangeCheck();
        }
        public virtual void DrawExtraTitleContent(Rect r)
        {
            // Override. Currently only used by Console
        }

        public string GetLabelString()
        {
            return expanded ? labelExpanded : labelContracted;
        }


        public int UndoableContentScaledToolbar(Rect r, string label, int selected, string[] labels, string undoInfix)
        {
            int newValue = VVS_GUI.ContentScaledToolbar(r, label, selected, labels);
            if (newValue != selected)
            {
                string undoName = "set " + undoInfix + " to " + labels[newValue];
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return selected;
        }


        public void UndoableConditionalToggle(Rect r, ref bool value, bool usableIf, bool disabledDisplayValue, string label, string undoSuffix)
        {
            bool nextValue = value;
            VVS_GUI.ConditionalToggle(r, ref nextValue, usableIf, disabledDisplayValue, label);
            if (nextValue != value)
            {
                string undoName = (nextValue ? "enable" : "disable") + " " + undoSuffix;
                Undo.RecordObject(this, undoName);
                value = nextValue;
            }
        }


        public bool UndoableToggle(Rect r, bool boolVar, string label, string undoActionName, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.toggle;
            bool newValue = GUI.Toggle(r, boolVar, label, style);
            if (newValue != boolVar)
            {
                string undoName = (newValue ? "enable" : "disable") + " " + undoActionName;
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return boolVar;
        }

        public bool UndoableToggle(Rect r, bool boolVar, string undoActionName, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.toggle;
            bool newValue = GUI.Toggle(r, boolVar, new GUIContent(""));
            if (newValue != boolVar)
            {
                string undoName = (newValue ? "enable" : "disable") + " " + undoActionName;
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return boolVar;
        }


        public Enum UndoableEnumPopup(Rect r, Enum enumValue, string undoInfix)
        {
            Enum nextEnum = EditorGUI.EnumPopup(r, enumValue);

            if (nextEnum.ToString() != enumValue.ToString())
            {
                string undoName = "set " + undoInfix + " to " + nextEnum;
                Undo.RecordObject(this, undoName);
                enumValue = nextEnum;
            }
            return enumValue;
        }


        public Enum UndoableLabeledEnumPopup(Rect r, string label, Enum enumValue, string undoInfix)
        {
            Enum nextEnum = VVS_GUI.LabeledEnumField(r, label, enumValue, EditorStyles.miniLabel);
            if (nextEnum.ToString() != enumValue.ToString())
            {
                string undoName = "set " + undoInfix + " to " + nextEnum;
                Undo.RecordObject(this, undoName);
                enumValue = nextEnum;
            }
            return enumValue;
        }


        public int UndoableEnumPopupNamed(Rect r, Enum enumValue, string[] displayedOptions, string undoInfix)
        {
            int nextEnum = EditorGUI.Popup(r, (int)((object)enumValue), displayedOptions);
            if (nextEnum != ((int)((object)enumValue)))
            {
                string undoName = "set " + undoInfix + " to " + displayedOptions[nextEnum];
                Undo.RecordObject(this, undoName);
                return nextEnum;
            }
            return (int)((object)enumValue);
        }

        public int UndoableLabeledEnumPopupNamed(Rect r, string label, Enum enumValue, string[] displayedOptions, string undoInfix)
        {
            int nextEnum = VVS_GUI.LabeledEnumFieldNamed(r, displayedOptions, new GUIContent(label), (int)((object)enumValue), EditorStyles.miniLabel);
            if (nextEnum != ((int)((object)enumValue)))
            {
                string undoName = "set " + undoInfix + " to " + displayedOptions[nextEnum];
                Undo.RecordObject(this, undoName);
                return nextEnum;
            }
            return (int)((object)enumValue);
        }


        //UndoablePopup

        public float UndoableFloatField(Rect r, float value, string undoInfix, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.textField;
            float newValue = EditorGUI.FloatField(r, value, style);
            if (newValue != value)
            {
                string undoName = "set " + undoInfix + " to " + newValue;
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return value;
        }

        public int UndoableIntField(Rect r, int value, string undoInfix, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.textField;
            int newValue = EditorGUI.IntField(r, value, style);
            if (newValue != value)
            {
                string undoName = "set " + undoInfix + " to " + newValue;
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return value;
        }

        public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.textField;
            string newValue = EditorGUI.TextField(r, value, style);
            if (newValue != value)
            {
                string undoName = "change " + undoInfix + " to " + newValue;
                Undo.RecordObject(this, undoName);
                return newValue;
            }
            return value;
        }

        public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style = null, UnityEngine.Object extra = null, bool showContent = true)
        {
            if (style == null)
                style = EditorStyles.textField;
            string newValue = EditorGUI.TextField(r, value, style);
            if (newValue != value)
            {
                string undoName = "change " + undoInfix;
                if (showContent)
                    undoName += " to " + newValue;
                Undo.RecordObject(this, undoName);
                if (extra != null)
                    Undo.RecordObject(extra, undoName);
                return newValue;
            }
            return value;
        }


    }

}