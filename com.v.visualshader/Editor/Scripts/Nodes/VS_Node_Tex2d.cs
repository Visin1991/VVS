using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public enum NoTexValue { White,Gray,Black,Bump};
    public class VS_Node_Tex2d : VS_Node
    {
        public Texture textureAsset;


        public NoTexValue noTexValue = NoTexValue.White;
        public bool markedAsNormalMap = false;

        public VS_Node_Tex2d()
        {

        }

        public override void Initialize()
        {
            base.Initialize("Texture 2D");
            base.UseLowerPropertyBox(true, true);

            //Create Property  

            //Create Connector

            base.alwaysDefineVariable = true;
            base.neverDefineVariable = false;
            base.preview.CompCount = 4;

            //connectors[0].usageC
        }

        public override bool Draw()
        {
            //Check Property Input

            //Base---Process Input

            //Base---Draw Highlight

            //Base---Prepare Window Color

            //Base---DrawWindow

            //Base---ResetWindowColor


            DrawHighlight();

            DrawWindow();


            return true;
        }

    }
}
