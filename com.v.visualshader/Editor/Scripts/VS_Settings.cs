using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public enum NodeRenderMode { Mixed, Spheres, Viewport };

    public static class VS_Settings
    {
        // Cached, for speed
        public static bool autoCompile;
        public static bool hierarchalNodeMove;
        public static bool quickPickScrollWheel;
        public static bool showVariableSettings;
        public static bool showNodeSidebar;
        public static bool realtimeNodePreviews;
        public static NodeRenderMode nodeRenderMode;

        public static bool showMainMenu;

    }

}