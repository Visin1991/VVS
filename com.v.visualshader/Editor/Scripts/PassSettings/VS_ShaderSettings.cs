﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

using V.VEditorGUI;

namespace V
{
    [System.Serializable]
    public class VS_ShaderSettings : CollapsGUI
    {
        public enum Inspector3DPreviewType { Sphere, Plane, Skybox };
        public string[] strInspector3DPreviewType = new string[] { "3D object", "2D sprite", "Sky" };

        public enum BatchingMode { Enabled, Disabled, DisableDuringLODFade };
        public string[] strBatchingMode = new string[] { "Enabled", "Disabled", "Disabled during LOD fade" };
        public Inspector3DPreviewType previewType = Inspector3DPreviewType.Sphere;

        public BatchingMode batchingMode = BatchingMode.Enabled;
        public bool canUseSpriteAtlas = false;

        public bool[] usedRenderers = new bool[12]{ // TODO: Load from project settings
				true,	// - Direct3D 9
				true,	// - Direct3D 11
				true,	// - OpenGL Core
				true,	// - OpenGL ES 2.0
				false,  // - OpenGL ES 3.0
				false,	// - iOS Metal
				false,	// - Direct3D 11 windows RT
				false,	// - Xbox One
				false,	// - PlayStation 4
				false,	// - PlayStation Vita
				false,	// - Nintendo 3DS
				false	// - Wii U
			};


        public string fallback = "";
        public int LOD = 0; // TODO: Serialization

        public List<string> cgIncludes = new List<string>();

        char[] splitChars = new char[] { '|' };

        //MenuCommand mc;
        private void DisplayShaderContext(Rect r)
        {
            //if (mc == null)
            //    mc = new MenuCommand(this, 0);
            //Material temp = new Material(Shader.Find("Lightweight Render Pipeline/Lit")); // This will make it highlight none of the shaders inside.
            //UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(temp); // Rebuild shader menu
            //DestroyImmediate(temp, true); // Destroy material

            //EditorUtility.DisplayPopupMenu(r, "CONTEXT/ShaderPopup", mc); // Display shader popup
        }

        private void OnSelectedShaderPopup(string command, Shader shader)
        {
            if (shader != null)
            {
                if (fallback != shader.name)
                {
                    Undo.RecordObject(this, "pick fallback shader");
                    fallback = shader.name;
                    VS_Editor.instance.Defocus();
                    //editor.OnShaderModified( NodeUpdateType.Hard );
                }
            }
        }

        public void ShaderPicker(Rect r, string s)
        {
            if (GUI.Button(r, s, EditorStyles.popup))
            {
                DisplayShaderContext(r);
            }
        }


        //====================================================================================================
        public override float DrawInner(ref Rect r)
        {
            EditorGUI.BeginChangeCheck();

            float prevYpos = r.y;
            r.y = 0;

            
            r.xMin += 20;                                                   // GUI Content Left move 20 pixels
            r.y += 20;                                                      // GUI Content Move Down 20 pixels

            string prevShaderPath = VS_Editor.instance.currentShaderPath;

            //------------------------------------------------------------------------------------------------------
            //Draw Path GUI
            //------------------------------------------------------------------------------------------------------
            {
                EditorGUI.LabelField(r, "Path", EditorStyles.miniLabel);        //Label

                r.xMin += 27;                                                   // GUI Content Left move 30 pixels
                r.height = 17;                                                  // Restrict height

                //What this doing???
                VVS_PassSettings.Instance.StartIgnoreChangeCheck();

                GUI.SetNextControlName("shdrpath");
                
                VS_Editor.instance.currentShaderPath = UndoableTextField(r, VS_Editor.instance.currentShaderPath, "shader path", null, VS_Editor.instance, showContent: false);

                if (VS_Editor.instance.currentShaderPath != prevShaderPath)
                {
                    VS_Tools.FormatShaderPath(ref VS_Editor.instance.currentShaderPath);
                }

                if (Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath")
                {
                    VS_Editor.instance.Defocus();
                    VS_Editor.instance.OnShaderModified();
                }

                VVS_PassSettings.Instance.EndIgnoreChangeCheck();
            }


            //------------------------------------------------------------------------------------------------------
            //Draw Fallback GUI
            //------------------------------------------------------------------------------------------------------


            r.xMin -= 27;
            r.height = 20;
            r.xMax += 3;
            r.y += 20;


            EditorGUI.LabelField(r, "Fallback", EditorStyles.miniLabel);
            Rect rStart = new Rect(r);
            r.xMin += 50;
            r.height = 17;
            r.xMax -= 47;
            VVS_PassSettings.Instance.StartIgnoreChangeCheck();
            GUI.SetNextControlName("shdrpath");
            prevShaderPath = fallback;
            fallback = UndoableTextField(r, fallback, "shader fallback", null, null, showContent: false);
            r.x += r.width + 2;
            r.width = 42;
            ShaderPicker(r, "Pick");
            if (fallback != prevShaderPath)
            {
                VS_Tools.FormatShaderPath(ref fallback);
            }
            if (Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath")
            {
                VS_Editor.instance.Defocus();
                VS_Editor.instance.OnShaderModified();
            }
            VVS_PassSettings.Instance.EndIgnoreChangeCheck();


            //------------------------------------------------------------------------------------------------------
            //Draw LOD GUI
            //------------------------------------------------------------------------------------------------------

            r = rStart;
            r.y += r.height;


            EditorGUI.LabelField(r, "LOD", EditorStyles.miniLabel);
            r.xMin += 30;
            r.height = 17;
            r.xMax -= 3;
            LOD = UndoableIntField(r, LOD, "LOD");
            r.xMin -= 30;
            r.height = 20;
            r.xMax += 3;
            r.y += 20;

            //------------------------------------------------------------------------------------------------------
            //Draw Atlased Option
            //------------------------------------------------------------------------------------------------------

            canUseSpriteAtlas = UndoableToggle(r, canUseSpriteAtlas, "Allow using atlased sprites", "allow using atlased sprites", null);
            r.y += 20;

            //------------------------------------------------------------------------------------------------------
            //Draw Call Batching Option
            //------------------------------------------------------------------------------------------------------

            batchingMode = (BatchingMode)UndoableLabeledEnumPopupNamed(r, "Draw call batching", batchingMode, strBatchingMode, "draw call batching");
            r.y += 20;

            //------------------------------------------------------------------------------------------------------
            //Prewview Mode Options
            //------------------------------------------------------------------------------------------------------

            previewType = (Inspector3DPreviewType)UndoableLabeledEnumPopupNamed(r, "Inspector preview mode", previewType, strInspector3DPreviewType, "inspector preview mode");
            r.y += 20;

            r.y += 10;

            //------------------------------------------------------------------------------------------------------
            //CG include Option
            //------------------------------------------------------------------------------------------------------

            if (cgIncludes.Count == 0)
            {
                Rect rBtn = r;
                rBtn.height -= 4;
                rBtn.width = 100;
                if (GUI.Button(rBtn, "Add CG Include", EditorStyles.miniButton))
                {
                    Undo.RecordObject(this, "add CG include");
                    cgIncludes.Add("");
                }
                //r.y += 20;
            }
            else
            {

                EditorGUI.LabelField(r, "CG Includes:");
                r.y += 20;


                int removeTarget = -1;

                for (int i = 0; i < cgIncludes.Count; i++)
                {

                    Rect smallRect = r;
                    smallRect.width = 20;
                    smallRect.height -= 2;

                    if (GUI.Button(smallRect, "-"))
                    {
                        removeTarget = i;
                    }

                    r.xMin += 22;

                    Rect textFieldRect = r;
                    textFieldRect.height -= 2;
                    textFieldRect.width -= 3;
                    cgIncludes[i] = UndoableTextField(textFieldRect, cgIncludes[i], "cg include", null);
                    textFieldRect.x += 1;
                    GUI.color = new Color(1f, 1f, 1f, 0.3f);
                    GUI.Label(textFieldRect, "<color=#00000000>" + cgIncludes[i] + "</color>.cginc", VS_Styles.RichLabel);
                    GUI.color = Color.white;
                    r.y += 20;

                    r.xMin -= 22;
                }

                if (removeTarget != -1)
                {
                    Undo.RecordObject(this, "remove CG include");
                    cgIncludes.RemoveAt(removeTarget);
                }

                Rect buttonRect = r;
                buttonRect.width = 20;
                buttonRect.height -= 2;
                if (GUI.Button(buttonRect, "+"))
                {
                    Undo.RecordObject(this, "add CG include");
                    cgIncludes.Add("");
                }
            }


            //------------------------------------------------------------------------------------------------------
            //Render Platform Option
            //------------------------------------------------------------------------------------------------------


            r.y += 40;



            EditorGUI.LabelField(r, "Target renderers:");
            r.xMin += 20;
            r.y += 20;
            r.height = 17;
            float pWidth = r.width;


            bool onlyDX11GlCore = false;//ps.mOut.tessellation.IsConnectedAndEnabled();


            for (int i = 0; i < usedRenderers.Length; i++)
            {
                bool isDX11orGlCore = (i == (int)RenderPlatform.d3d11) || i == (int)RenderPlatform.glcore;

                r.width = 20;

                bool prevEnable = GUI.enabled;
                //bool displayBool = usedRenderers[i];

                bool shouldDisable = !isDX11orGlCore && onlyDX11GlCore;

                if (shouldDisable)
                {
                    GUI.enabled = false;
                    EditorGUI.Toggle(r, false);
                }
                else
                {
                    usedRenderers[i] = UndoableToggle(r, usedRenderers[i], VS_Tools.rendererLabels[i] + " renderer");
                    //usedRenderers[i] = EditorGUI.Toggle( r, usedRenderers[i] );
                }


                r.width = pWidth;
                r.xMin += 20;
                EditorGUI.LabelField(r, VS_Tools.rendererLabels[i], EditorStyles.miniLabel);

                if (shouldDisable)
                {
                    GUI.enabled = prevEnable;
                }

                r.xMin -= 20;
                r.y += r.height + 1;
            }

            

            if (EditorGUI.EndChangeCheck())
            {
                //Send A Message of Change
      
            }

            r.y += prevYpos;
            return (int)r.yMax;
        }
        //========================================================================================================================================



    }
}
