using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace V
{
    public static class VS_GUI
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
    }
}
