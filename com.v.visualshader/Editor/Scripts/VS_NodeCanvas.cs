using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace V
{

    [System.Serializable]
    public class VS_SetNodeSource
    {
        public VS_NodeConnector con;

        public VS_SetNodeSource(VS_Node node)
        {
            con = node.connectors[0];
        }

        public Guid NodeID
        {
            get { return con.node.id; }
        }

        public string Name
        {
            get { return con.node.variableName; }
        }
    }


    public class VS_NodeCanvas : ScriptableObject
    {
        public static VS_NodeCanvas Instance;

        const int TOOLBAR_HEIGHT = 18;

        public Vector2 cameraPos = Vector3.zero;

        bool panCamera = false;

        Vector2 mousePosStart;

        public Rect rect;

        public GUIStyle toolbarStyle;

        public List<VS_SetNodeSource> relayInSources;
        public string[] relayInNames;

        public Vector2 nodeSpaceMousePos;
        public Vector2 viewSpaceMousePos;

        public float zoom = 1f;
        public float zoomTarget = 1f;

        bool isCutting;
        Vector2 cutStart;


        protected virtual void Awake()
        {
            Instance = this;

            cameraPos = new Vector2(32768 - 400, 32768 - 300);

        }


        public void RefreshRelaySources()
        {
            relayInSources = new List<VS_SetNodeSource>();

            //Get all Set node ...........

            //Sort Node by name

            //Get a string array of the node's name

        }

        public Vector2 ZoomSpaceToScreenSpace(Vector2 in_vec)
        {
            return (in_vec - cameraPos + VS_Editor.instance.separatorLeft.rect.TopRight()) * zoom + rect.TopLeft() + (Vector2.up * (VS_Editor.instance.TabOffset)) * (zoom - 1);
        }

        public Vector2 ScreenSpaceToZoomSpace(Vector2 in_vec)
        {
            return (in_vec - (Vector2.up * (VS_Editor.instance.TabOffset)) * (zoom - 1) - rect.TopLeft()) / zoom - VS_Editor.instance.separatorLeft.rect.TopRight() + cameraPos ;
        }

        public Vector2 GetNodeSpaceMousePos()
        {
            return nodeSpaceMousePos;
        }

        public void StartCutting()
        {
            isCutting = true;
            cutStart = GetNodeSpaceMousePos();
        }

        public void StopCutting()
        {
            isCutting = false; //.....................................
        }

        public void UpdteCutLine()
        {
            if (VEditorGUI.Utility.HoldingAlt() && Event.current.type == EventType.MouseDown && Event.current.button == 1)
            { //Alt + RMB Drag
                StartCutting();
            }
            else if (VEditorGUI.Utility.ReleasedRawRMB())
            {
                StopCutting();
            }

            if (isCutting)
            {

                //.............

            }

        }

        void UpdateCameraPanning()
        {
            if (VEditorGUI.Utility.ReleasedCameraMove())
            {
                panCamera = false;
            }

            bool insideNodeView = MouseInsideNodeView(true);
            bool dragging = (Event.current.type == EventType.MouseDrag && panCamera);

            if (dragging)
            {
                cameraPos -= Event.current.delta;
                SnapCamera();

                BoundsAdjustCamera();
                VS_Editor.instance.Defocus();

                Event.current.Use();
            }

            if (VEditorGUI.Utility.PressedCameraMove())
            {
                panCamera = true;
            }

        }

        public float ClampZoom(float in_zoom)
        {
            return Mathf.Clamp(in_zoom, 0.125f, 1f);
        }

        void BoundsAdjustCamera()
        {

            //Do nothing at all
        }

        void SnapCamera()
        {
            cameraPos.x = Mathf.Round(cameraPos.x);
            cameraPos.y = Mathf.Round(cameraPos.y);
        }

        public void SetZoom(float setZoom)
        {
            Vector2 oldWidth = new Vector2(rect.width, rect.height) / zoom;
            zoom = ClampZoom(setZoom);
            Vector2 newWidth = new Vector2(rect.width, rect.height) / zoom;
            Vector2 delta = newWidth - oldWidth;

            Vector2 normalizedMouseCoords = (Event.current.mousePosition - new Vector2(VS_Editor.instance.separatorLeft.rect.xMax, VS_Editor.instance.TabOffset));

            normalizedMouseCoords.x /= rect.width;
            normalizedMouseCoords.y /= rect.height;

            cameraPos -= Vector2.Scale(delta, normalizedMouseCoords);

            if (delta.sqrMagnitude != 0f)
            {
                // Correct in here to prevent going outside the bounds
                BoundsAdjustCamera();
            }

            if (zoom == 1f)
                SnapCamera();

        }

        public bool MouseInsideNodeView(bool offset = false)
        {

            if (offset)
            {
                return rect.Contains(viewSpaceMousePos);
            }
            else
            {
                return rect.Contains(Event.current.mousePosition);
            }

        }


        public void Draw(Rect r)
        {
            VS_Editor.instance.mousePosition = Event.current.mousePosition;
            rect = r;

            Rect localRect = new Rect(r);
            localRect.x = 0;
            localRect.y = 0;

            //View
            Rect rectInner = new Rect(rect);
            rectInner.width = float.MaxValue / 2f;
            rectInner.height = float.MaxValue / 2f;

            if (Event.current.type == EventType.Repaint)
            {
                nodeSpaceMousePos = ScreenSpaceToZoomSpace(Event.current.mousePosition);
            }

            bool mouseOverNode = false;

            //Debug.Log("Zoom Rect (rect) : " + rect);

            ZoomArea.Begin(zoom, rect, cameraPos);
            {

                if (Event.current.type == EventType.Repaint)
                {
                    viewSpaceMousePos = ZoomSpaceToScreenSpace(Event.current.mousePosition);
                }

                if (VS_Editor.instance.nodes != null)
                {

                    for (int i = VS_Editor.instance.nodes.Count - 1; i >= 0; i--)
                    {
                        if (!VS_Editor.instance.nodes[i].Draw())
                        {
                            break;
                        }
                    }

                    if (!mouseOverNode)
                    {

                        for (int i = 0; i < VS_Editor.instance.nodes.Count; i++)
                        {
                            if (VS_Editor.instance.nodes[i].MouseOverNode(world: true))
                            {
                                mouseOverNode = true;
                            }
                        }
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        for (int i = 0; i < VS_Editor.instance.nodes.Count; i++)
                        {
                            //Nodes Draw Connectors......
                        }
                    }
                }

                UpdteCutLine();
                UpdateCameraPanning();

            }
            ZoomArea.End(zoom);


            //......
            //Check if Editing Any Node Text Field

            //Check if Node inside the Canvas && if we ScrollWhell
            if (MouseInsideNodeView(false) && Event.current.type == EventType.ScrollWheel)
            {
                zoomTarget = ClampZoom(zoomTarget * (1f - Event.current.delta.y * 0.02f));
                Debug.Log(zoomTarget);
            }




            //........
            SetZoom(Mathf.Lerp(zoom, zoomTarget, 0.2f));



            if (Event.current.type == EventType.ContextClick && !VEditorGUI.Utility.HoldingAlt())
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (rect.Contains(mousePos))
                {
                    //GenericMenu menu = new GenericMenu();
                    VS_Editor.instance.AddNode();

                }
            }

            //........................

            //........................
            if (MouseInsideNodeView(false) && Event.current.type == EventType.MouseUp)
            {
                bool ifCursorStayed = Vector2.SqrMagnitude(mousePosStart - Event.current.mousePosition) < VS_Tools.stationaryCursorRadius;

                if (ifCursorStayed && !VEditorGUI.Utility.MultiSelectModifierHeld())
                {
                    //Selection Deselect All....
                }
            }

            //........................
            if (Event.current.type == EventType.MouseDown && MouseInsideNodeView(false))
            {
                mousePosStart = Event.current.mousePosition;
                VS_Editor.instance.Defocus();
            }
        }

        public Guid GetUniqueNodeID()
        {
            return Guid.NewGuid();
        }
    }
}
