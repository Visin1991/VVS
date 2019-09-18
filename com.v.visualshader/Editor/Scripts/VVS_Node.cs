using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.VVS
{
    [System.Serializable]
    public class VVS_Node : ScriptableObject, IDependable<VVS_Node>
    {

        public VVS_Node()
        {
            //Debug.Log("NODE " + GetType());
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
        }


        //IDependable Interfaces......
        private List<VVS_Node> dependencies;

        private int iDepth = 0;
        int IDependable<VVS_Node>.Depth
        {
            get {
                return iDepth;
            }
            set
            {
                iDepth = value;
            }

        }
        List<VVS_Node> IDependable<VVS_Node>.Dependencies
        {
            get
            {
                if (dependencies == null)
                {
                    dependencies = new List<VVS_Node>();
                }
                return dependencies;
            }
            set
            {
                dependencies = value;
            }
        }
        void IDependable<VVS_Node>.AddDependency(VVS_Node dp)
        {
            (this as IDependable<VVS_Node>).Dependencies.Add(dp);
        }

        public virtual void Initialize()
        {
            // Override
        }

    }
}