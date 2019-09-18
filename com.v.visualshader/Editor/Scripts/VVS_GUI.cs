﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace V
{
    public static class VVS_GUI
    {
        private static Texture2D icon;
        public static Texture2D Icon
        {
            get {
                if (icon == null)
                {
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.v.visualshader/Editor/VSSResources/icon.png");
                    if (icon == null)
                    {
                        Debug.Log("Get Error When Load VSSResources/icon.png");
                    }
                }
                return icon;
            }
        }

        private static Texture2D logo;
        public static Texture2D Logo
        {
            get {
                if(logo == null)
                    logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.v.visualshader/Editor/VSSResources/logo.png");
                 return logo;
            }     
        }

        private static Texture2D handle_drag;
        public static Texture2D Handle_drag
        {
            get {
                if (handle_drag == null)
                    handle_drag = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.v.visualshader/Editor/VSSResources/handle_drag.tga");
                return handle_drag;
            }
        }


        public static void FillBackGround(Rect r)
        {
            Color pCol = GUI.color;
            UseBackgroundColor();
            GUI.DrawTexture(r, EditorGUIUtility.whiteTexture);
            GUI.color = pCol;
        }

        public const byte ColBgPro = (byte)56;
        public const byte ColBgFree = (byte)194;

        public static void UseBackgroundColor()
        {
            byte v = EditorGUIUtility.isProSkin ? ColBgPro : ColBgFree;
            GUI.color = new Color32(v, v, v, (byte)255);
        }





        //====================================================
        public static bool PressedLMB(Rect r)
        {
            return (PressedLMB() && r.Contains(Event.current.mousePosition));
        }

        public static bool PressedLMB()
        {
            return (Event.current.type == EventType.MouseDown) && (Event.current.button == 0);
        }

        public static bool ReleasedLMB()
        {
            return (Event.current.type == EventType.MouseUp) && (Event.current.button == 0);
        }

        public static bool PressedMMB()
        {
            return (Event.current.type == EventType.MouseDown) && (Event.current.button == 2);
        }

        public static bool ReleasedRawMMB()
        {
            return (Event.current.rawType == EventType.MouseUp) && (Event.current.button == 2);
        }

        public static bool ReleasedRawLMB()
        {
            return (Event.current.rawType == EventType.MouseUp) && (Event.current.button == 0);
        }

        public static bool ReleasedRawRMB()
        {
            return (Event.current.rawType == EventType.MouseUp) && (Event.current.button == 1);
        }

        public static bool PressedRMB()
        {
            return (Event.current.type == EventType.MouseDown) && (Event.current.button == 1);
        }

        public static bool ReleasedRMB()
        {
            return (Event.current.type == EventType.MouseUp) && (Event.current.button == 1);
        }

        public static bool HoldingAlt()
        {
            return (Event.current.modifiers & EventModifiers.Alt) != 0; // Alt is held
        }


        public static void AssignCursor(Rect r, MouseCursor cursor)
        {
            EditorGUIUtility.AddCursorRect(r, cursor);
        }


        public static bool HoldingControl()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
                return (Event.current.modifiers & EventModifiers.Command) != 0; // Command is held
            else
            {
                return (Event.current.control); // Control is held
            }

        }

        //====================================================

        public static int WidthOf(GUIContent s, GUIStyle style)
        {
            return (int)style.CalcSize(s).x;
        }

        public static int WidthOf(string s, GUIStyle style)
        {
            return (int)style.CalcSize(new GUIContent(s)).x;
        }

        //====================================================

        public static int ContentScaledToolbar(Rect r, string label, int selected, string[] labels)
        {

            r.height = 15;

            Rect rLeft = new Rect(r);
            Rect rRight = new Rect(r);

            rLeft.width = VVS_GUI.WidthOf(label, EditorStyles.miniLabel) + 4;
            rRight.width = r.width - rLeft.width;
            rRight.x += rLeft.width;

            GUI.Label(rLeft, label, EditorStyles.miniLabel);


            // Full pixel width of strings:
            float[] lblPxWidth = new float[labels.Length];
            float pxWidthTotal = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                lblPxWidth[i] = VVS_GUI.WidthOf(labels[i], EditorStyles.miniButtonMid);
                pxWidthTotal += lblPxWidth[i];
            }

            // Scale all buttons to fit the rect
            float scale = rRight.width / pxWidthTotal;
            for (int i = 0; i < labels.Length; i++)
            {
                lblPxWidth[i] *= scale;
            }




            GUIStyle style = EditorStyles.miniButtonLeft;
            int retval = selected;

            Rect rTemp = new Rect(rRight);

            for (int i = 0; i < labels.Length; i++)
            {

                rTemp.width = lblPxWidth[i];

                if (i == labels.Length - 1)
                {
                    style = EditorStyles.miniButtonRight;
                }
                else if (i > 0)
                {
                    style = EditorStyles.miniButtonMid;
                }

                bool prev = selected == i;
                bool newVal = GUI.Toggle(rTemp, prev, labels[i], style);
                if (newVal != prev)
                {
                    retval = i;
                }

                rTemp.x += rTemp.width;
            }
            GUI.color = Color.white;
            return retval;

        }

        public static void ConditionalToggle(Rect r, ref bool value, bool usableIf, bool disabledDisplayValue, string label)
        {
            if (usableIf)
            {
                value = GUI.Toggle(r, value, label);
            }
            else
            {
                GUI.enabled = false;
                GUI.Toggle(r, disabledDisplayValue, label);
                GUI.enabled = true;
            }
        }

        public static System.Enum LabeledEnumField(Rect r, string label, System.Enum enumVal, GUIStyle style, bool zoomCompensate = false)
        {
            return LabeledEnumField(r, new GUIContent(label), enumVal, style, zoomCompensate);
        }

        public static System.Enum LabeledEnumField(Rect r, GUIContent label, System.Enum enumVal, GUIStyle style, bool zoomCompensate = false)
        {
            Rect leftRect = new Rect(r);
            Rect rightRect = new Rect(r);
            int width = WidthOf(label, style) + 4;
            leftRect.width = width;
            rightRect.xMin += width;
            GUI.Label(leftRect, label, style);

            return VVS_GUI.EnumPopup(rightRect, GUIContent.none, enumVal, EditorStyles.popup, zoomCompensate);
        }

        public static Enum EnumPopup(Rect position, GUIContent label, Enum selected, GUIStyle style, bool zoomCompensate = false)
        {


            Type type = selected.GetType();
            if (!type.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            string[] names = Enum.GetNames(type);
            int num = Array.IndexOf<string>(names, Enum.GetName(type, selected));
            Matrix4x4 prevMatrix = Matrix4x4.identity;
            if (zoomCompensate)
            {
                prevMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.identity;
            }
            num = EditorGUI.Popup(position, label, num, TempContent((
                from x in names
                select ObjectNames.NicifyVariableName(x)).ToArray<string>()), style);
            if (num < 0 || num >= names.Length)
            {
                if (zoomCompensate)
                    GUI.matrix = prevMatrix;
                return selected;
            }
            if (zoomCompensate)
                GUI.matrix = prevMatrix;
            return (Enum)Enum.Parse(type, names[num]);
        }

        public static int LabeledEnumFieldNamed(Rect r, string[] names, GUIContent label, int enumVal, GUIStyle style)
        {
            Rect leftRect = new Rect(r);
            Rect rightRect = new Rect(r);
            int width = WidthOf(label, style) + 4;
            leftRect.width = width;
            rightRect.xMin += width;
            GUI.Label(leftRect, label, style);
            return EditorGUI.Popup(rightRect, (int)enumVal, names);
        }

        private static GUIContent[] TempContent(string[] texts)
        {
            GUIContent[] array = new GUIContent[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                array[i] = new GUIContent(texts[i]);
            }
            return array;
        }
    }
}
