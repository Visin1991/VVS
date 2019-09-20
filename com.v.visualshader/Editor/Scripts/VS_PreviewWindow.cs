using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace V
{
    public class VS_PreviewWindow
    {
        const float minFOV = 1f;
        float targetFOV = 30f;
        float smoothFOV = 30f;
        const float maxFOV = 60f;

        public bool previewAutoRotate = true;
        Color colorBg = EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f, 1f) : new Color(0.6f, 0.6f, 0.6f, 1f);


        public Mesh mesh;
        Material internalMaterial;
        public Material InternalMaterial
        {
            get
            {
                if (internalMaterial == null)
                {
                    internalMaterial = new Material(VS_Editor.instance.currentShaderAsset);
                }
                return internalMaterial;
            }
            set
            {
                internalMaterial = value;
            }
        }


        public RenderTexture renderTexture; // TODO: Why is this separated from the RT itself?
        GUIStyle previewStyle;

        //Used for solid color Sky
        public Texture2D backgroundTexture;

        bool previewIsSetUp = false; // Intentionally non-serialized


        public bool isDraggingLMB = false;

        Vector2 dragStartPosLMB = Vector2.zero;

        Vector2 rotMeshStart = new Vector2(-30f, 0f);
        Vector2 rotMesh = new Vector2(30f, 0f);

        Vector2 rotMeshSmooth = new Vector2(-30f, 0f);
        public bool isDraggingRMB = false;


        public Camera cam;
        Transform camPivot;
        Light[] lights;

        Mesh _sphereMesh;

        Mesh sphereMesh
        {
            get
            {
                if (_sphereMesh == null)
                {
                    _sphereMesh = VVS_Sphere.Create();
                }
                return _sphereMesh;
            }
        }

        // Reflection to call Handles.SetCameraOnlyDrawMesh(this.m_Camera);
        MethodInfo mSetCameraOnlyDrawMesh;

        bool enabled = true;

        public bool SkyboxOn
        {
            get
            {
                return cam.clearFlags == CameraClearFlags.Skybox;
            }
            set
            {
                cam.clearFlags = value ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
            }
        }

        public void OnEnable()
        {
            enabled = true;
            SetupPreview();
        }

        public void OnDisable()
        {
            enabled = false;
            CleanupObjects();
        }

        public int OnGUI(int yOffset, int maxWidth)
        {
            if (enabled == false)
                return yOffset;

            Rect topBar = new Rect(0, yOffset, maxWidth, 18);

            GUI.Box(topBar, "", EditorStyles.toolbar);

            //Divide the topBar to 3 Chuncks

            

            Rect r = new Rect(topBar);
            float xPadding = 10;

            r.width = maxWidth / 3;
            r.height = 16;
            r.y += 1;

            //Mesh Filed
            {
                r.x += xPadding;
                DrawMeshFiled(r);
            }

            //SkyColor
            {
                r.x += r.width + xPadding;
                r.width *= 0.5f;
                DrawSkyColorGUI(r);
            }

            //SkyBox
            {
                r.x += r.width + xPadding;
                r.width += 10;
                DrawSkyBoxGUI(r);
            }

            //Rotation
            {
                r.x += r.width + xPadding;
                previewAutoRotate = GUI.Toggle(r, previewAutoRotate, "Rotate");
            }


            Rect previewRect = new Rect(topBar);
            previewRect.y += topBar.height;
            previewRect.height = topBar.width;  //Make a squre previewRect


            UpdateCameraZoom();

            DrawMeshGUI(previewRect);

            return (int)previewRect.yMax;
        }

        public VS_PreviewWindow()
        {
            UpdatePreviewBackgroundColor();
            this.mesh = sphereMesh;
            SetupPreview();
        }

        public void DrawMeshGUI(Rect _previewRect)
        {

            if (_previewRect.width > 1)
                this.previewRect = _previewRect;

            //Mouse Up Event
            if (Event.current.rawType == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                    StopDragLMB();
                else if (Event.current.button == 1)
                    StopDragRMB();
            }

            //Mouse Down Event
            if (Event.current.type == EventType.MouseDown && MouseOverPreview())
            {
                if (Event.current.button == 0)
                    StartDragLMB();
                else if (Event.current.button == 1)
                    StartDragRMB();
            }

            if (isDraggingLMB)
                UpdateDragLMB();
            if (isDraggingRMB)
                UpdateDragRMB();

            if (mesh == null || InternalMaterial == null || Event.current.type != EventType.Repaint)
                return;

            if (previewStyle == null)
            {
                previewStyle = new GUIStyle(EditorStyles.textField);
            }

            previewStyle.normal.background = backgroundTexture;

            bool makeNew = false;

            if (renderTexture == null)
            {
                makeNew = true;
            }
            else if (renderTexture.width != (int)_previewRect.width || renderTexture.height != (int)_previewRect.height)
            {
                RenderTexture.DestroyImmediate(renderTexture);
                makeNew = true;
            }

            if (makeNew)
            {
                renderTexture = new RenderTexture((int)_previewRect.width, (int)_previewRect.height, 24, RenderTextureFormat.ARGB32);
                renderTexture.antiAliasing = 8;
            }

            DrawMesh();

            GUI.DrawTexture(_previewRect, renderTexture, ScaleMode.StretchToFill, false);
        }
        void DrawMesh(Material overrideMaterial = null)
        {
            if (backgroundTexture == null)
                UpdatePreviewBackgroundColor();

            // Make sure all objects are set up properly
            if (previewIsSetUp == false)
            {
                SetupPreview();
            }

            cam.targetTexture = renderTexture;

            SetCustomLight(on: true);

            Mesh drawMesh = mesh;

            float A =  rotMeshSmooth.y;
            float B =  rotMeshSmooth.x;
            Quaternion rotA = Quaternion.Euler(0f, A, 0f);
            Quaternion rotB = Quaternion.Euler(B, 0f, 0f);
            Quaternion finalRot = rotA * rotB;
            camPivot.rotation = finalRot;
            float meshExtents = drawMesh.bounds.extents.magnitude;

            Vector3 _999 = Vector3.one * -9999;
            Vector3 pos = new Vector3(-drawMesh.bounds.center.x, -drawMesh.bounds.center.y, -drawMesh.bounds.center.z);
            pos += _999;
            cam.transform.localPosition = new Vector3(0f, 0f, -3f * meshExtents);
            cam.transform.position += _999;

            int smCount = drawMesh.subMeshCount;

            Material mat = (overrideMaterial == null) ? InternalMaterial : overrideMaterial;

            for (int i = 0; i < smCount; i++)
            {
                Graphics.DrawMesh(drawMesh, Quaternion.identity * pos, Quaternion.identity, mat, 31, cam, i);
            }

            
            cam.farClipPlane = 3f * meshExtents * 2f;
            cam.nearClipPlane = 0.1f;
            cam.fieldOfView = smoothFOV;
            cam.Render();

            // Reset things
            SetCustomLight(on: false);

        }

        void DrawMeshFiled(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            mesh = (Mesh)EditorGUI.ObjectField(r, mesh, typeof(Mesh), false);
            if (EditorGUI.EndChangeCheck())
            {
                targetFOV = 35f;
            }
        }

        void DrawSkyColorGUI(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            GUI.enabled = cam.clearFlags != CameraClearFlags.Skybox;
            colorBg = EditorGUI.ColorField(r, "", colorBg);
            cam.backgroundColor = colorBg;
            GUI.enabled = true;
            if (EditorGUI.EndChangeCheck())
                UpdatePreviewBackgroundColor();
        }

        void DrawSkyBoxGUI(Rect r)
        {
            GUI.enabled = RenderSettings.skybox != null;
            SkyboxOn = GUI.Toggle(r, SkyboxOn, "Skybox");
            if (RenderSettings.skybox == null && SkyboxOn)
            {
                SkyboxOn = false;
            }
            GUI.enabled = true;
        }

        void UpdateCameraZoom()
        {
            if (Event.current.type == EventType.ScrollWheel && MouseOverPreview())
            {
                if (Event.current.delta.y > 0f)
                {
                    targetFOV += 2f;
                }
                else if (Event.current.delta.y < 0f)
                {
                    targetFOV -= 2f;
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                smoothFOV = Mathf.Lerp(cam.fieldOfView, targetFOV, 0.5f);
            }
        }

        void CleanupObjects()
        {
            GameObject.DestroyImmediate(cam.gameObject);
            GameObject.DestroyImmediate(camPivot.gameObject);
            for (int i = 0; i < lights.Length; i++)
            {
                GameObject.DestroyImmediate(lights[i].gameObject);
            }
        }

        static Vector2 rotMeshSphere = new Vector2(22, -18 - 90 - 12);
        const float fovSphere = 23.4f;

        void PrepareForDataScreenshot()
        {

            // Reset rotation
            // Reset zoom
            // Stop auto-rotate
            rotMesh.x = rotMeshSmooth.x = rotMeshSphere.x;
            rotMesh.y = rotMeshSmooth.y = rotMeshSphere.y;
            cam.fieldOfView = targetFOV = smoothFOV = fovSphere;

        }

        public void UpdateRot()
        {
            if (previewAutoRotate)
            {
                rotMesh.y += (float)(VS_Editor.instance.deltaTime * -22.5);
            }
            rotMeshSmooth = Vector2.Lerp(rotMeshSmooth, rotMesh, 0.5f);
        }

        void StartDragLMB()
        {
            isDraggingLMB = true;
            if (previewAutoRotate == true)
            {
                previewAutoRotate = false;
            }
            dragStartPosLMB = Event.current.mousePosition;
            rotMeshStart = rotMesh;
        }

        void UpdateDragLMB()
        {
            rotMesh.y = rotMeshStart.y + (-(dragStartPosLMB.x - Event.current.mousePosition.x)) * 0.4f;
            rotMesh.x = Mathf.Clamp(rotMeshStart.x + (-(dragStartPosLMB.y - Event.current.mousePosition.y)) * 0.4f, -90f, 90f);
        }

        void StopDragLMB()
        {
            isDraggingLMB = false;
        }

        void StartDragRMB()
        {
            isDraggingRMB = true;
        }

        void UpdateDragRMB()
        {
            if (Event.current.isMouse && Event.current.type == EventType.MouseDrag)
            {
                float x = (-(Event.current.delta.x)) * 0.4f;
                float y = (-(Event.current.delta.y)) * 0.4f;
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].transform.RotateAround(Vector3.zero, cam.transform.right, y);
                    lights[i].transform.RotateAround(Vector3.zero, cam.transform.up, x);
                }
            }
        }

        void StopDragRMB()
        {
            isDraggingRMB = false;
        }

        bool MouseOverPreview()
        {
            return previewRect.Contains(Event.current.mousePosition);
        }

        [SerializeField]
        Rect previewRect = new Rect(0f, 0f, 1f, 1f);

        void SetupPreview()
        {
            previewIsSetUp = true;


            // Create Camera Object
            GameObject camObj = new GameObject("VVS Camera");
            camObj.hideFlags = HideFlags.HideAndDontSave;
            cam = camObj.AddComponent<Camera>();
            cam.targetTexture = renderTexture;
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.renderingPath = RenderingPath.Forward;
            cam.enabled = false;
            cam.useOcclusionCulling = false;
            cam.cameraType = CameraType.Preview;
            cam.fieldOfView = targetFOV;

          

            // Make sure it only renders using DrawMesh, to make ignore the scene. This is a bit risky, due to using reflection :(
            //BindingFlags bfs = BindingFlags.Static | BindingFlags.NonPublic;
            //Type[] args = new Type[] { typeof(Camera) };

            //mSetCameraOnlyDrawMesh = typeof(Handles).GetMethod("SetCameraOnlyDrawMesh", bfs, null, args, null);
            //if (mSetCameraOnlyDrawMesh != null)
            //    mSetCameraOnlyDrawMesh.Invoke(null, new object[] { cam });


            // Create pivot/transform to hold it
            camPivot = new GameObject("VVS Camera Pivot").transform;
            camPivot.gameObject.hideFlags = HideFlags.HideAndDontSave;
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.transform.parent = camPivot;

            // Create custom light sources
            lights = new Light[] {
                new GameObject("Light 0").AddComponent<Light>(),
                new GameObject("Light 1").AddComponent<Light>()
            };

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].gameObject.hideFlags = HideFlags.HideAndDontSave;
                lights[i].type = LightType.Directional;
                lights[i].lightmapBakeType = LightmapBakeType.Realtime;
                lights[i].enabled = false;
            }

            lights[0].intensity = 1f;
            lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
            lights[1].intensity = 0.75f;
            lights[1].color = new Color(1f, 0.5f, 0.25f);
            lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 177f);


        }

        void UpdatePreviewBackgroundColor()
        {
            if (backgroundTexture == null)
            {
                backgroundTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, QualitySettings.activeColorSpace == ColorSpace.Linear);
                backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
            }
            Color c = colorBg;
            backgroundTexture.SetPixels(new Color[] { c });
            backgroundTexture.Apply();
        }

        public void SetCustomLight(bool on)
        {
            if (on)
            {
                UnityEditorInternal.InternalEditorUtility.SetCustomLighting(lights, RenderSettings.ambientLight);
            }
            else
            {
                UnityEditorInternal.InternalEditorUtility.RemoveCustomLighting();
            }
        }
    }

    public static class VVS_Sphere
    {
        private static Vector3[] directions = {
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.forward
        };

        public static Mesh Create(int subdivisions = 3, float radius = 1)
        {
            if (subdivisions < 0)
            {
                subdivisions = 0;
                Debug.LogWarning("Octahedron Sphere subdivisions increased to minimum, which is 0.");
            }
            else if (subdivisions > 6)
            {
                subdivisions = 6;
                Debug.LogWarning("Octahedron Sphere subdivisions decreased to maximum, which is 6.");
            }

            //Each time, we do a subdivision, We turns a triangle into 4.
            //so the a triangle turns into 4^s or 2^(2s).  s is the number of subdivision.
            //We start with eight triangles, our subdivided octahedron will end up with 2^(2s + 3) triangles

            /*           .   .   .   .
                        / \ / \ / \ / \
                       . - . - . - . - .
                        \ / \ / \ / \ /
                         .   .   .   .

                   r is the subdivision number   

                   As we can see each lozenge have (r + 1)^2 vertices
                   then four lozenge will have 4 * (r + 1)^2 vertices

                   However,we can find the center of the middle row have three shared point,
                   Through observation and analysis, eacj lozenge will share 2r -1 vertices

                    we have 1 << (subdivisions * 2 + 3) triangles, each triangles have 3 indices
                    so total tris =  (1 << (subdivisions * 2 + 3) ) * 3

             */

            int resolution = 1 << subdivisions;
            Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1) * 4 - (resolution * 2 - 1) * 3];
            int[] triangles = new int[(1 << (subdivisions * 2 + 3)) * 3];
            CreateOctahedron(vertices, triangles, resolution);

            Vector3[] normals = new Vector3[vertices.Length];
            Normalize(vertices, normals);

            Vector2[] uv = new Vector2[vertices.Length];
            CreateUV(vertices, uv);

            Vector4[] tangents = new Vector4[vertices.Length];
            CreateTangents(vertices, tangents);

            if (radius != 1f)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] *= radius;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "Octahedron Sphere";
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.tangents = tangents;
            mesh.triangles = triangles;
            return mesh;
        }

        private static void CreateOctahedron(Vector3[] vertices, int[] triangles, int resolution)
        {
            //Build the mesh from the bottom to the top

            int v = 0, vBottom = 0, t = 0;

            //On the Botton, We use Four Vertices, if we only use one vertex on the polar, 
            //The UV along the longitude will get twisted.
            for (int i = 0; i < 4; i++)
            {
                vertices[v++] = Vector3.down;
            }

            for (int i = 1; i <= resolution; i++)
            {
                float progress = (float)i / resolution;
                Vector3 from, to;
                vertices[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);

                //Loop Through Left back right forward
                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.down, directions[d], progress);
                    t = CreateLowerStrip(i, v, vBottom, t, triangles);
                    v = CreateVertexLine(from, to, i, v, vertices);
                    vBottom += i > 1 ? (i - 1) : 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = resolution - 1; i >= 1; i--)
            {
                float progress = (float)i / resolution;
                Vector3 from, to;
                vertices[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);

                //Loop Through Left back right forward
                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.up, directions[d], progress);
                    t = CreateUpperStrip(i, v, vBottom, t, triangles);
                    v = CreateVertexLine(from, to, i, v, vertices);
                    vBottom += i + 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = 0; i < 4; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = v;
                triangles[t++] = ++vBottom;

                //Same Thing for The botton
                vertices[v++] = Vector3.up;
            }
        }

        private static int CreateVertexLine(Vector3 from, Vector3 to, int steps, int v, Vector3[] vertices)
        {
            for (int i = 1; i <= steps; i++)
            {
                vertices[v++] = Vector3.Lerp(from, to, (float)i / steps);
            }
            return v;
        }

        private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
        {
            for (int i = 1; i < steps; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;

                triangles[t++] = vBottom++;
                triangles[t++] = vTop++;
                triangles[t++] = vBottom;
            }
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            return t;
        }

        private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = ++vBottom;
            for (int i = 1; i <= steps; i++)
            {
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;
                triangles[t++] = vBottom;

                triangles[t++] = vBottom;
                triangles[t++] = vTop++;
                triangles[t++] = ++vBottom;
            }
            return t;
        }

        private static void Normalize(Vector3[] vertices, Vector3[] normals)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = vertices[i] = vertices[i].normalized;
            }
        }

        private static void CreateUV(Vector3[] vertices, Vector2[] uv)
        {
            float previousX = 1f;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                if (v.x == previousX)
                {
                    uv[i - 1].x = 1f;
                }
                previousX = v.x;
                Vector2 textureCoordinates;
                textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
                if (textureCoordinates.x < 0f)
                {
                    textureCoordinates.x += 1f;
                }
                textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
                uv[i] = textureCoordinates;
            }

            //Because the polar has four vertices
            //We mannully adjust the horizontal coodinates ---- to solve the uv twisted issue.
            uv[vertices.Length - 4].x = uv[0].x = 0.125f;
            uv[vertices.Length - 3].x = uv[1].x = 0.375f;
            uv[vertices.Length - 2].x = uv[2].x = 0.625f;
            uv[vertices.Length - 1].x = uv[3].x = 0.875f;
        }

        private static void CreateTangents(Vector3[] vertices, Vector4[] tangents)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                v.y = 0f;
                v = v.normalized;
                Vector4 tangent;
                tangent.x = -v.z;
                tangent.y = 0f;
                tangent.z = v.x;
                tangent.w = -1f;
                tangents[i] = tangent;
            }

            tangents[vertices.Length - 4] = tangents[0] = new Vector3(-1f, 0, -1f).normalized;
            tangents[vertices.Length - 3] = tangents[1] = new Vector3(1f, 0f, -1f).normalized;
            tangents[vertices.Length - 2] = tangents[2] = new Vector3(1f, 0f, 1f).normalized;
            tangents[vertices.Length - 1] = tangents[3] = new Vector3(-1f, 0f, 1f).normalized;
            for (int i = 0; i < 4; i++)
            {
                tangents[vertices.Length - 1 - i].w = tangents[i].w = -1f;
            }
        }


    }
}
