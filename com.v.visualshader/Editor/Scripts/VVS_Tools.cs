using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace V.VVS
{

    public enum RenderPlatform
    {
        d3d9 = 0,   // - Direct3D 9
        d3d11 = 1,  // - Direct3D 11 / 12
        glcore = 2, // - OpenGL Core
        gles = 3,   // - OpenGL ES 2.0
        gles3 = 4,  // - OpenGL ES 3.0
        metal = 5,  // - iOS Metal
        d3d11_9x = 6,   // - Direct3D 11 windows RT
        xboxone = 7,    // - Xbox One
        ps4 = 8,    // - PlayStation 4
        psp2 = 9,   // - PlayStation Vita
        n3ds = 10,  // - Nintendo 3DS
        wiiu = 11   // - Nintendo Wii U
    };

    public static class VVS_Tools
    {


        public static void FormatShaderPath(ref string s)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9/s -_]"); // Only allow Alphanumeric, forward slash, space, dash and underscore
            s = rgx.Replace(s, "");
        }

        public static string[] rendererLabels = new string[]{
            "Direct3D 9",
            "Direct3D 11 & 12",
            "OpenGL Core",
            "OpenGL ES 2.0",
            "OpenGL ES 3.0",
            "iOS Metal",
            "Direct3D 11 for Windows RT/Phone",
            "Xbox One",
            "PlayStation 4",
            "PlayStation Vita",
            "Nintendo 3DS",
            "Nintendo Wii U"
        };

    }
}
