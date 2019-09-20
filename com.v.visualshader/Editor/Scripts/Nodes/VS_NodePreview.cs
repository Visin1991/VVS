using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace V
{
    public class VS_NodePreview 
    {
        public static int R = 0;
        public static int G = 1;
        public static int B = 2;
        public static int A = 3;

        // The color representation 
        public RenderTexture texture;           // RGBA combined
        public RenderTexture[] textureChannels; // RGBA separated, created on-demand

        // Icons, if any
        public Texture2D[] icons;
        public Texture2D iconActive;

        public void SetIconId(int id)
        {
            iconActive = icons[id];
        }

        public VS_Node node;

        public VS_NodePreview(VS_Node _node)
        {
            node = _node;
        }

        public bool final = true;

        public Color iconColor = Color.white;
        public bool uniform = false;
        public bool coloredAlphaOverlay = false; // Used to render two images on top of eachother, as in the fog node

        public Vector4 dataUniform;
        public Color dataUniformColor
        {
            get { return (Color)dataUniform; }
        }

        // The amount of components used (1-4) // THIS SHOULDN'T BE USED. USE CONNECTOR COMP COUNT INSTEAD
        [SerializeField]
        private int compCount = 1;

        public int CompCount
        {
            get { return compCount; }
            set
            {
                if (compCount == value)
                    return;
                if (value > 4 || value < 1)
                {
                    //Debug.LogError( "Component count out of range: " + value + " on " + node.nodeName + " " + node.id );
                    compCount = 4;
                }
                else
                {
                    compCount = value;
                }
            }
        }


        public Color ConvertToDisplayColor(Color fa, bool forceVisible = false)
        {
            if (CompCount == 1)
            {
                return new Color(fa[0], fa[0], fa[0], forceVisible ? 1f : fa[0]);
            }
            else if (CompCount == 2)
            {
                return new Color(fa[0], fa[1], 0f, forceVisible ? 1f : 0f);
            }
            else if (CompCount == 3)
            {
                return new Color(fa[0], fa[1], fa[2], forceVisible ? 1f : 0f);
            }
            return new Color(fa[0], fa[1], fa[2], forceVisible ? 1f : fa[3]);
        }

        public void Fill(Color col)
        {
            VS_OffScreenRendering.Render(texture, col);
        }

        public void LoadAndInitializeIcons(Type type)
        {
            string nodeNameLover = type.Name.ToLower();

            iconActive = VS_GUI.LoadNodeIcon(nodeNameLover);

            if (iconActive)
            {
                List<Texture2D> iconList = new List<Texture2D>();
                iconList.Add(iconActive);



            }

        }

        public void Draw(Rect r, bool dim = false)
        {
            if (iconActive != null)
            {
                //if node is a Final Out put node ----- Main Node
                if (final)
                {
                    Rect tmp = new Rect(r.x, r.y - 1, iconActive.width, iconActive.height);
                    GUI.color = new Color(1f, 1f, 1f, node.selected ? 1f : 0.5f);
                    GUI.DrawTexture(tmp, iconActive, ScaleMode.ScaleToFit, true);
                }
                else if (coloredAlphaOverlay)
                {
                    GUI.DrawTexture(r, icons[0]);
                    GUI.color = ConvertToDisplayColor(dataUniform, true);
                    GUI.DrawTexture(r, icons[1], ScaleMode.ScaleToFit, true);
                }
                else
                {
                    GUI.color = iconColor;
                    if (dim)
                    {
                        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
                    }
                    GUI.DrawTexture(r, iconActive);
                }
                GUI.color = Color.white;
            }
            else if (uniform)
            {
                GUI.color = ConvertToDisplayColor(dataUniform, true);
                GUI.DrawTexture(r, EditorGUIUtility.whiteTexture);
                GUI.color = Color.white;
            }
            else
            {
                GUI.DrawTexture(r, texture, ScaleMode.ScaleAndCrop, false);
                if (node.displayVectorDataMask)
                {
                    //GUI.DrawTexture(r, VEditorGUI.Utility.VectorIconOverlay, ScaleMode.ScaleAndCrop, true);
                }

            }

        }


    }
}
