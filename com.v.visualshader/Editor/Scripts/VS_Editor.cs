using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using V.VEditorGUI;

namespace V
{

    public enum UpToDateState { UpToDate, OutdatedSoft, OutdatedHard };


    [Serializable]
    public class VS_Editor : EditorWindow
    {
        public DraggableSeparator separatorLeft;
        public DraggableSeparator separatorRight;

        public VS_PreviewWindow preview;
        VS_StatusBox statusBox;


        public double deltaTime = 0.02;
        public static bool ProSkin = true;

        public static VS_Editor instance;
        public Rect previousPosition;
        public bool closeMe = false;
        public bool initialized = false;

        public Vector2 mousePosition = Vector2.zero;

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

        public List<VS_Node> nodes;

        DateTime startTime = DateTime.UtcNow;
        double prevFrameTime;
        float fps;


        VS_NodeCanvas nodeCanvas;


        //========================================================================================================================
        MethodInfo isDockedMethod;
        const float dockedCheckInterval = 1f;
        public float dockedLastUpdate = -100f;
        public bool _docked = false;
        public bool Docked
        {
            get
            {
                if (EditorApplication.timeSinceStartup - dockedLastUpdate > dockedCheckInterval)
                {
                    dockedLastUpdate = (float)EditorApplication.timeSinceStartup;
                    if (isDockedMethod == null)
                    {
                        BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                        isDockedMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);
                    }
                    _docked = (bool)isDockedMethod.Invoke(this, null);
                }
                return _docked;
            }
        }

        public int TabOffset
        {
            get
            {
                return Docked ? 19 : 22;
            }
        }

        //====================================================================================================================
        [MenuItem("V/VVS")]
        static void OpenWindow()
        {
            if (VS_Editor.instance == null)
            {
                InitEditor(null);
            }
            EditorWindow.GetWindow(typeof(VS_Editor)).Show();
        }
        //====================================================================================================================
        private void OnEnable()
        {
            //Load All From Disk();

            titleContent = new GUIContent("V Visual Shader", VS_GUI.Icon);

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
                    VS_Node n = nodes[i];
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
            VS_GUI.FillBackGround(pRect);
            DrawPreviewPanel(pRect);

            Rect previewPanelRect = pRect;

            separatorLeft.MinX = 320;
            separatorLeft.MaxX = (int)(fullRect.width / 2f - separatorLeft.rect.width);
            separatorLeft.Draw((int)pRect.y, (int)pRect.height);

            pRect.x = separatorLeft.rect.x + separatorLeft.rect.width;

            if (VS_Settings.showNodeSidebar)
                pRect.width = separatorRight.rect.x - separatorLeft.rect.x - separatorLeft.rect.width;
            else
                pRect.width = Screen.width - separatorLeft.rect.x - separatorLeft.rect.width;

            //Draw Debug Nodes
            //......................
            //......................


            //Draw NodeView....
            //......................
            //......................

            Rect canvasRect = pRect.PadTop(TabOffset);

            nodeCanvas.Draw(canvasRect); // // 22 when not docked, 19 if docked






            if (VS_Settings.showNodeSidebar)
            {
                separatorRight.MinX = (int)(fullRect.width / EditorGUIUtility.pixelsPerPoint) - 150;
                separatorRight.MaxX = (int)(fullRect.width / EditorGUIUtility.pixelsPerPoint) - 32;
                separatorRight.Draw((int)pRect.y, (int)pRect.height);

                pRect.x += pRect.width + separatorRight.rect.width;
                pRect.width = (fullRect.width / EditorGUIUtility.pixelsPerPoint) - separatorRight.rect.x - separatorRight.rect.width;

                VS_GUI.FillBackGround(pRect);
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

        public VS_Node AddNode()
        {
            VS_Node_Tex2d tex2DNode = VS_Node_Tex2d.CreateInstance<VS_Node_Tex2d>();
            tex2DNode.Initialize();

            nodes.Add(tex2DNode);

            if (Event.current != null)
                Event.current.Use();

            return tex2DNode;
        }

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

            VS_Settings.autoCompile = GUI.Toggle(btnRect, VS_Settings.autoCompile, "Auto");

            btnRect.y += 4;

            //======================================================================================
            if (displaySettings)
            {
                btnRect.y += btnRect.height;
                btnRect.x = r.x - 4;
                btnRect.width = r.width / 4f;
                btnRect.x += btnRect.width;
                btnRect.width *= 2.55f;


                if (VS_Settings.nodeRenderMode == NodeRenderMode.Viewport)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(btnRect, true, "Real-time node rendering");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    VS_Settings.realtimeNodePreviews = GUI.Toggle(btnRect, VS_Settings.realtimeNodePreviews, "Real-time node rendering");
                    if (EditorGUI.EndChangeCheck())
                    {
                        
                    }
                }

                btnRect = btnRect.MovedDown();
                VS_Settings.quickPickScrollWheel = GUI.Toggle(btnRect, VS_Settings.quickPickScrollWheel, "Use scroll in the quickpicker");
                btnRect = btnRect.MovedDown();
                VS_Settings.showVariableSettings = GUI.Toggle(btnRect, VS_Settings.showVariableSettings, "Show variable name & precision");
                btnRect = btnRect.MovedDown();
                VS_Settings.showNodeSidebar = GUI.Toggle(btnRect, VS_Settings.showNodeSidebar, "Show node browser panel");
                btnRect = btnRect.MovedDown();

                if (V.VEditorGUI.Utility.HoldingControl())
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(btnRect, !VS_Settings.hierarchalNodeMove, "Hierarchal Node Move");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    VS_Settings.hierarchalNodeMove = GUI.Toggle(btnRect, VS_Settings.hierarchalNodeMove, "Hierarchal Node Move");
                }

                btnRect.y += 4;
            }
            //======================================================================================
            previewButtonHeightOffset = (int)btnRect.yMax + 24;
            int previewOffset = preview.OnGUI((int)btnRect.yMax, (int)r.width);
            int statusBoxOffset = statusBox.OnGUI(previewOffset, (int)r.width);


            VVS_PassSettings.Instance.Draw(statusBoxOffset, (int)r.width);

        }

        public static bool InitEditor(Shader initShader = null)
        {
            // To make sure you get periods as decimal separators
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            VS_Editor materialEditor = (VS_Editor)EditorWindow.GetWindow(typeof(VS_Editor));

            VS_Editor.instance = materialEditor;

            bool loaded = materialEditor.InitializeInstance(initShader);
            if (!loaded)
                return false;
            return true;

        }

        public bool InitializeInstance(Shader initShader = null)
        {
            //Setting Initialization

            this.initialized = true;

            preview = new VS_PreviewWindow();
            statusBox = new VS_StatusBox();
            statusBox.Initialize();

            //Use Scriptable Object Because we can use Undo ......  which Unity Provide
            ScriptableObject.CreateInstance<VVS_PassSettings>();


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

            separatorLeft = new DraggableSeparator(VS_GUI.Handle_drag);
            separatorRight = new DraggableSeparator(VS_GUI.Handle_drag);

            separatorLeft.rect = new Rect(340, 0, 0, 0);
            separatorRight.rect = new Rect(Screen.width - 130f, 0, 0, 0);


            //Create Nodes List

            this.nodes = new List<VS_Node>();

            // Create main output node and add to list
            nodeCanvas = VS_NodeCanvas.CreateInstance<VS_NodeCanvas>();


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
                    GUILayout.Label(VS_GUI.Logo);
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
