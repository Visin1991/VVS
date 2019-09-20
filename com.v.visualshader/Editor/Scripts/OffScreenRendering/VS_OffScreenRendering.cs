using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VS_OffScreenRendering
    {
        static Mesh vs_OffScreenQuad;
        public static Mesh blitQuad
        {
            get
            {
                if (vs_OffScreenQuad == null)
                {

                    vs_OffScreenQuad = new Mesh();

                    Vector3[] verts = new Vector3[]{
                        new Vector3(-1,1),
                        new Vector3(1,1),
                        new Vector3(-1,-1),
                        new Vector3(1,-1)
                    };
                    Vector2[] uvs = new Vector2[]{
                        new Vector2(0,1),
                        new Vector2(1,1),
                        new Vector2(0,0),
                        new Vector2(1,0)
                    };
                    Vector3[] normals = new Vector3[]{
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                        Vector3.back
                    };
                    Vector4[] tangents = new Vector4[]{
                        new Vector4(1,0,0,1),
                        new Vector4(1,0,0,1),
                        new Vector4(1,0,0,1),
                        new Vector4(1,0,0,1)
                    };
                    Color[] colors = new Color[]{
                        new Color(1f,0f,0f,0f),
                        new Color(0f,1f,0f,0f),
                        new Color(0f,0f,1f,0f),
                        new Color(0f,0f,0f,1f),
                    };

                    int[] triangles = new int[]{
                        0,1,2,
                        1,3,2
                    };

                    vs_OffScreenQuad.vertices = verts;
                    vs_OffScreenQuad.triangles = triangles;
                    vs_OffScreenQuad.normals = normals;
                    vs_OffScreenQuad.tangents = tangents;
                    vs_OffScreenQuad.colors = colors;
                    vs_OffScreenQuad.uv = uvs;
                    vs_OffScreenQuad.uv2 = uvs;
                    vs_OffScreenQuad.uv3 = uvs;
                    vs_OffScreenQuad.uv4 = uvs;

                }
                return vs_OffScreenQuad;
            }


        }


        static Material _matColor;
        public static Material matColor
        {
            get
            {
                if (_matColor == null)
                    _matColor = new Material(Shader.Find("Hidden/Shader Forge/FillColor"));
                return _matColor;
            }
        }

        public static void Render(RenderTexture target, Material material)
        {
            //Rendering Stuff 
        }

        public static void Render(RenderTexture target, Color color)
        {
            //matColor.color = color;
            //Render(target, matColor);
        }



    }
}
