using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Xml;

namespace V
{
    [System.Serializable]
    public class VS_Node : ScriptableObject, IDependable<VS_Node>
    {

        public const int NODE_SIZE = 96;
        public const int NODE_WIDTH = NODE_SIZE + 3;    // This fits a NODE_SIZE texture inside
        public const int NODE_HEIGHT = NODE_SIZE + 16;  // This fits a NODE_SIZE texture inside

        public int node_width = NODE_WIDTH;
        public int node_height = NODE_HEIGHT;


        [SerializeField]
        public VS_NodeConnector[] connectors;

        public VS_NodePreview preview;


        public Guid id;

        public string nodeName;

        public bool discreteTitle = false;

        public string _variableName;
        public string variableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }

        public bool selected = false;

        Rect rect;
        public Rect rectInner;
        public Rect lowerRect;

        float commentYposTarget;
        float commentYposCurrent;

        public bool displayVectorDataMask = false;

        public bool showLowerPropertyBox;
        public bool showLowerPropertyBoxAlways;
        public bool showLowerReadonlyValues;
        public bool initialized = false;


        //.........
        public bool alwaysDefineVariable = false;
        public bool neverDefineVariable = false;



        public VS_Node()
        {
            //Debug.Log("NODE " + GetType());
        }

        public void Initialize(string name)
        {
            Vector2 pos = VS_Editor.instance.mousePosition;

            AssignID();

            this.nodeName = name;

            preview = new VS_NodePreview(this);

            //preview.Fil
            preview.Fill(Color.black);

            //Generate Base Data

            preview.LoadAndInitializeIcons(this.GetType());

            Debug.Log("Mouse Pos : " + pos); 

            pos = VS_NodeCanvas.Instance.ScreenSpaceToZoomSpace(pos);

            Debug.Log("Zoom Space Pos : " + pos);

            InitializeDefaultRect(pos);

        }

        void InitializeDefaultRect(Vector2 pos)
        {
            float height = (showLowerPropertyBox ? (node_height) : (node_height + 20));
            this.rect = new Rect(
                pos.x - node_width / 2,
                pos.y - node_height / 2,
                node_width,height);

            rectInner = rect;
            rectInner.x = 1;
            rectInner.y = 15;

            lowerRect = rectInner;
            lowerRect.y += rectInner.height;
            lowerRect.height = 20;

        }

        public void AssignID()
        {
            this.id = VS_NodeCanvas.Instance.GetUniqueNodeID();
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
        }


        //IDependable Interfaces......
        private List<VS_Node> dependencies;

        private int iDepth = 0;
        int IDependable<VS_Node>.Depth
        {
            get {
                return iDepth;
            }
            set
            {
                iDepth = value;
            }

        }
        List<VS_Node> IDependable<VS_Node>.Dependencies
        {
            get
            {
                if (dependencies == null)
                {
                    dependencies = new List<VS_Node>();
                }
                return dependencies;
            }
            set
            {
                dependencies = value;
            }
        }
        void IDependable<VS_Node>.AddDependency(VS_Node dp)
        {
            (this as IDependable<VS_Node>).Dependencies.Add(dp);
        }

        public virtual void Initialize()
        {
            // Override
        }


        public bool MouseOverNode(bool world = false)
        {

            return false;
        }



        public void DrawHighlight()
        {
            if (selected)
            {
                Rect r = new Rect(rect);

                r.xMin -= 1;

                if (IsProperty())
                    r.yMin -= 20;

                //Draw Line HeighLight
            }


        }


        public bool IsProperty()
        {
            //if(property == null)
            return false;

        }

        public void UndoRecord(string undoMsg, UpToDateState tempOutdatedState = UpToDateState.OutdatedHard)
        {
            Undo.RecordObject(this, undoMsg);
        }

        public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style, bool readPropertyName = true)
        {
            if (style == null)
                style = EditorStyles.textField;
            string newValue = EditorGUI.TextField(r, value, style);
            if (newValue != value)
            {
                UndoRecord("edit " + undoInfix + " of " + nodeName + " node");    
                return newValue;
            }
            return value;
        }

        public string VarNameControl()
        {
            return "ctrl_" + id + "_varname";
        }

        // GUI color based on Property
        void PrepareWindowColor()
        {

        }

        //Set back to default GUI color
        void ResetWindowColor()
        {

        }

        public void OnPress()
        {
            //Dragging Nodes..........
        }

        public void OnRelease()
        {

        }

        public void UseLowerPropertyBox(bool use, bool always = false)
        {
            rect.height = (use ? (node_height + 20) : (node_height));
            showLowerPropertyBox = use;
            if (always)
                showLowerPropertyBoxAlways = use;
        }

        public virtual void NeatWindow()
        {
            GUI.BeginGroup(rect);

            GUI.skin.box.clipping = TextClipping.Overflow;

            //.......
            //Node Preview Draw

            //......
            //Draw Lower Property Box

            GUI.EndGroup();

        }

        protected void DrawWindow()
        {

            //Debug.Log("VS_Node  DrawWindow");

            if (Event.current.type == EventType.Repaint)
            {
                commentYposCurrent = Mathf.Lerp(commentYposCurrent, commentYposTarget, 0.4f);
            }

            //UnavailableInTHisRenderPath

            GUI.Box(rect, nodeName, discreteTitle ? VEditorGUI.VStyles.NodeStyleDiscrete : VEditorGUI.VStyles.NodeStyle);   //Create a box show the name : slider, Texture2D, Color..... whatever 

            //Detect if this node only available in Forward rendering path

            //....................

            ResetWindowColor();

            NeatWindow();

            if (VEditorGUI.Utility.PressedLMB())
            {
                OnPress();
            }
            else if (VEditorGUI.Utility.ReleasedLMB())
            {
                OnRelease();
            }
            else if (Event.current.type == EventType.ContextClick)
            {
                if (MouseOverNode(world: true))
                {
                    //......................

                }

            }

            {
                Rect ur = rect;

                Rect varNameRect = ur.Pad(4);

                GUI.SetNextControlName(VarNameControl());
                variableName = UndoableTextField(varNameRect, variableName, (IsProperty() ? "variable" : "internal") + " name", EditorStyles.textField, false);
                GUI.enabled = true;
                //----------------------------------------------------------------
            }

        }

        //===========================================================
        public virtual bool Draw()
        {

            DrawHighlight();

            PrepareWindowColor();

            //if(showLowerPropertyBoxAlways) ......

            DrawWindow();

            ResetWindowColor();

            return true;
        }
    }
}