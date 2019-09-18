﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;


namespace V.VVS
{

    public enum UpToDateState { UpToDate, OutdatedSoft, OutdatedHard };


    [Serializable]
    public class VVS_Editor : EditorWindow
    {
        public VVS_DraggableSeparator separatorLeft;
        public VVS_DraggableSeparator separatorRight;


        public VVS_PreviewWindow preview;
        VVS_StatusBox statusBox;
        public VVS_PassSettings ps; // TODO: Move



        public double deltaTime = 0.02;
        public static bool ProSkin = true;

        public static VVS_Editor instance;
        public Rect previousPosition;
        public bool closeMe = false;
        public bool initialized = false;

        [SerializeField]
        public Shader currentShaderAsset
        {
            get {
                return Shader.Find("Lightweight Render Pipeline/Lit");
            }
        }
        [SerializeField]
        public string currentShaderPath;

        [SerializeField]
        GUIStyle windowStyle;
        [SerializeField]
        GUIStyle titleStyle;
        [SerializeField]
        GUIStyle versionStyle;
        [SerializeField]
        GUIStyle nodeScrollbarStyle;

        private enum MainMenuState{Main,Credits,PresetPick}
        private MainMenuState menuState = MainMenuState.Main;

        public bool displaySettings = false;
        int previewButtonHeightOffset = 20;

        public List<VVS_Node> nodes;

        DateTime startTime = DateTime.UtcNow;
        double prevFrameTime;
        float fps;

        //====================================================================================================================
        [MenuItem("V/VVS")]
        static void OpenWindow()
        {
            if (VVS_Editor.instance == null)
            {
                InitEditor(null);
            }
            EditorWindow.GetWindow(typeof(VVS_Editor)).Show();
        }
        //====================================================================================================================
        private void OnEnable()
        {
            //Load All From Disk();

            titleContent = new GUIContent("V Visual Shader", VVS_GUI.Icon);

            if (preview != null)
                preview.OnEnable();
        }

        private void OnDisable()
        {
            if (preview != null)
                preview.OnDisable();
        }
        //====================================================================================================================
        private void OnGUI()
        {


            if (position != previousPosition)
            {
                OnWindowResized((int)(position.width - previousPosition.width), (int)(position.height - previousPosition.height));
                previousPosition = position;
            }

            Rect fullRect = new Rect(0, 0, Screen.width, Screen.height);

            if (currentShaderAsset == null)
            {
                DrawMainMenu();
                return;
            }

            if (Event.current.rawType == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
            {
                //
            }


            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    VVS_Node n = nodes[i];
                    if (n != null)
                    {
                        //n.DrawConnections()
                    }
                }
            }

            Rect pRect = new Rect(fullRect);
            pRect.height /= EditorGUIUtility.pixelsPerPoint;
            pRect.width /= EditorGUIUtility.pixelsPerPoint;
            pRect.width = separatorLeft.rect.x;
            VVS_GUI.FillBackGround(pRect);
            DrawPreviewPanel(pRect);

            Rect previewPanelRect = pRect;

            separatorLeft.MinX = 320;
            separatorLeft.MaxX = (int)(fullRect.width / 2f - separatorLeft.rect.width);
            separatorLeft.Draw((int)pRect.y, (int)pRect.height);

            pRect.x = separatorLeft.rect.x + separatorLeft.rect.width;

            if (VVS_Settings.showNodeSidebar)
                pRect.width = separatorRight.rect.x - separatorLeft.rect.x - separatorLeft.rect.width;
            else
                pRect.width = Screen.width - separatorLeft.rect.x - separatorLeft.rect.width;

            //Draw Debug Nodes
            //......................
            //......................


            //Draw NodeView....
            //......................
            //......................

            if (VVS_Settings.showNodeSidebar)
            {
                separatorRight.MinX = (int)(fullRect.width / EditorGUIUtility.pixelsPerPoint) - 150;
                separatorRight.MaxX = (int)(fullRect.width / EditorGUIUtility.pixelsPerPoint) - 32;
                separatorRight.Draw((int)pRect.y, (int)pRect.height);

                pRect.x += pRect.width + separatorRight.rect.width;
                pRect.width = (fullRect.width / EditorGUIUtility.pixelsPerPoint) - separatorRight.rect.x - separatorRight.rect.width;

                VVS_GUI.FillBackGround(pRect);
                //nodeBrowser.OnLocalGUI( pRect );
            }





        }
        //====================================================================================================================
        private void Update()
        {
            TimeSpan t = (DateTime.UtcNow - startTime);
            double now = t.TotalSeconds;
            deltaTime = now - prevFrameTime;
            fps = 1f / (float)deltaTime;

            prevFrameTime = now;
            preview.UpdateRot();

            if (focusedWindow == this)
                Repaint(); // Update GUI every frame if focused

        }
        //====================================================================================================================

        public void DrawPreviewPanel(Rect r)
        {
            // Left side shader preview
            //Rect logoRect = new Rect( 1, 0, SF_GUI.Logo.width, SF_GUI.Logo.height );
            //GUI.DrawTexture( logoRect, SF_GUI.Logo );
            Rect btnRect = new Rect(r);
            btnRect.y += 4;
            btnRect.x += 2;


            int wDiff = 8;

            btnRect.height = 17;
            btnRect.width /= 4;
            btnRect.width += wDiff;

            GUIStyle btnStyle = EditorStyles.miniButton;

            if (GUI.Button(btnRect, "Return to menu", btnStyle))
            {
                //OnPressBackToMenuButton();
            }

            btnRect.x += btnRect.width;
            btnRect.xMax -= wDiff * 2;
            btnRect.width *= 0.75f;
            displaySettings = GUI.Toggle(btnRect, displaySettings, "Settings", btnStyle);

            btnRect.x += btnRect.width;
            btnRect.width *= 2f;

            GUI.color = new Color(0.7f, 1f, 0.7f);
            if (GUI.Button(btnRect, "Compile shader", btnStyle))
            {
            
            }
            GUI.color = Color.white;

            btnRect.x += btnRect.width;
            btnRect.width *= 0.5f;

            VVS_Settings.autoCompile = GUI.Toggle(btnRect, VVS_Settings.autoCompile, "Auto");

            btnRect.y += 4;

            //======================================================================================
            if (displaySettings)
            {
                btnRect.y += btnRect.height;
                btnRect.x = r.x - 4;
                btnRect.width = r.width / 4f;
                btnRect.x += btnRect.width;
                btnRect.width *= 2.55f;


                if (VVS_Settings.nodeRenderMode == NodeRenderMode.Viewport)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(btnRect, true, "Real-time node rendering");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    VVS_Settings.realtimeNodePreviews = GUI.Toggle(btnRect, VVS_Settings.realtimeNodePreviews, "Real-time node rendering");
                    if (EditorGUI.EndChangeCheck())
                    {
                        
                    }
                }

                btnRect = btnRect.MovedDown();
                VVS_Settings.quickPickScrollWheel = GUI.Toggle(btnRect, VVS_Settings.quickPickScrollWheel, "Use scroll in the quickpicker");
                btnRect = btnRect.MovedDown();
                VVS_Settings.showVariableSettings = GUI.Toggle(btnRect, VVS_Settings.showVariableSettings, "Show variable name & precision");
                btnRect = btnRect.MovedDown();
                VVS_Settings.showNodeSidebar = GUI.Toggle(btnRect, VVS_Settings.showNodeSidebar, "Show node browser panel");
                btnRect = btnRect.MovedDown();

                if (VVS_GUI.HoldingControl())
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(btnRect, !VVS_Settings.hierarchalNodeMove, "Hierarchal Node Move");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    VVS_Settings.hierarchalNodeMove = GUI.Toggle(btnRect, VVS_Settings.hierarchalNodeMove, "Hierarchal Node Move");
                }

                btnRect.y += 4;
            }
            //======================================================================================
            previewButtonHeightOffset = (int)btnRect.yMax + 24;
            int previewOffset = preview.OnGUI((int)btnRect.yMax, (int)r.width);
            int statusBoxOffset = statusBox.OnGUI(previewOffset, (int)r.width);

            ps.OnLocalGUI(statusBoxOffset, (int)r.width);

        }

        public static bool InitEditor(Shader initShader = null)
        {
            // To make sure you get periods as decimal separators
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            VVS_Editor materialEditor = (VVS_Editor)EditorWindow.GetWindow(typeof(VVS_Editor));

            VVS_Editor.instance = materialEditor;

            bool loaded = materialEditor.InitializeInstance(initShader);
            if (!loaded)
                return false;
            return true;

        }

        public bool InitializeInstance(Shader initShader = null)
        {
            //Setting Initialization

            this.initialized = true;

            preview = new VVS_PreviewWindow(this);
            statusBox = new VVS_StatusBox();
            statusBox.Initialize(this);

            //Use Scriptable Object Because we can use Undo ......  which Unity Provide
            ps = ScriptableObject.CreateInstance<VVS_PassSettings>().Initialize(this);

            //...Initialize Pass Setting
            //...Initialize Shader Evaluator
            //...Initialize Preview
            //...Initialize statusBox

            //InitializeNodeTemplates();

            windowStyle = new GUIStyle(EditorStyles.textField);
            windowStyle.margin = new RectOffset(0, 0, 0, 0);
            windowStyle.padding = new RectOffset(0, 0, 0, 0);

            titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.fontSize = 24;

            versionStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            versionStyle.alignment = TextAnchor.MiddleLeft;
            versionStyle.fontSize = 9;
            versionStyle.normal.textColor = Color.gray;
            versionStyle.padding.left = 1;
            versionStyle.padding.top = 1;
            versionStyle.padding.bottom = 1;
            versionStyle.margin.left = 1;
            versionStyle.margin.top = 3;
            versionStyle.margin.bottom = 1;

            separatorLeft = new VVS_DraggableSeparator();
            separatorRight = new VVS_DraggableSeparator();

            separatorLeft.rect = new Rect(340, 0, 0, 0);
            separatorRight.rect = new Rect(Screen.width - 130f, 0, 0, 0);


            //Create Nodes List

            // Create main output node and add to list

            this.previousPosition = position;

            if (initShader == null)
            {
                // TODO: New menu etc
                //CreateOutputNode();
            }
            else
            {
                //currentShaderAsset = initShader;

                //Load Shader......
            }

            return true;
        }

        public void DrawMainMenu()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();

                FlexHorizontal(() =>
                {
                    GUILayout.Label(VVS_GUI.Logo);
                    GUILayout.Label("VVS" + "0.0.1", EditorStyles.boldLabel);
                });

                if (menuState == MainMenuState.Main)
                {
                    minSize = new Vector2(500, 400);
                    DrawPrimaryMainMenuGUI();
                }



                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();



        }

        public void DrawPrimaryMainMenuGUI()
        {
            FlexHorizontal(()=>
            {

                if (true)
                {
                    if (GUILayout.Button("New Shader", GUILayout.Width(128), GUILayout.Height(64)))
                    {
                        menuState = MainMenuState.PresetPick;
                    }
                    if (GUILayout.Button("Load Shader", GUILayout.Width(128), GUILayout.Height(64)))
                    {
                        LoadShaderDialog();
                    }
                }
            });
        }

        void LoadShaderDialog()
        {

        }

        public void FlexHorizontal(Action func)
        {
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            func();
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
        }

        void OnWindowResized(int deltaXsize,int deltaYsize)
        {
            // if(......)ForceClose();
        }

        void ForceClose()
        {
            closeMe = true;
            GUIUtility.ExitGUI();
        }


        static Assembly editorAssembly;

        public static Assembly EditorAssembly
        {
            get {
                if (editorAssembly == null)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (assembly.FullName.Split(',')[0].Trim() == "Assembly-CSharp-Editor")
                        {
                            editorAssembly = assembly;
                            return editorAssembly;
                        }
                    }
                }
                return editorAssembly;
            }
        }

        public static Type GetNodeType(string nodeName)
        {
            Assembly asm = EditorAssembly;
            if (asm == null)
                return null;

            string fullNodeName = nodeName;
            if (!nodeName.StartsWith("V.VVS."))
                fullNodeName = "V.VVS." + nodeName;
            if(VVS_Debug.dynamicNodeLoad)
                Debug.Log("Trying to dynamically load [" + fullNodeName + "]" + " in assembly [" + asm.FullName + "]");

            return asm.GetType(fullNodeName);

        }

        public void Defocus(bool deselectNodes = false)
        {
            //Debug.Log("DEFOCUS");
            //			string currentFocus = GUI.GetNameOfFocusedControl();
            //			if( currentFocus != "defocus"){
            GUI.FocusControl("null");
            //			}
        }

        public void OnShaderModified()
        {

        }

    }

    public static class VVS_EX
    {
        public static string[] DisplayStrings(this FloatPrecision fp)
        {
            return new string[]{
                "fixed (11 bit)",
                "half (16 bit)",
                "float (32 bit)"
            };
        }
    }


    public enum FloatPrecision { Fixed, Half, Float };
    public enum CompCount { c1, c2, c3, c4, c1x1, c2x2, c3x3, c4x4 };

    public static class VVS_Debug
    {

        public static bool nodes = false;
        public static bool window = false;
        public static bool evalFlow = false;
        public static bool screenshot = false;
        public static bool ghostNodes = false;
        public static bool nodeActions = false;
        public static bool performance = false;
        public static bool nodePreviews = false;
        public static bool dynamicNodeLoad = false;
        public static bool deserialization = false;
        public static bool renderDataNodes = false;

    }

}
