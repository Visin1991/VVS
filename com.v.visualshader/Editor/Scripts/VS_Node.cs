using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    [System.Serializable]
    public class VS_Node : ScriptableObject, IDependable<VS_Node>
    {

        public VS_Node()
        {
            //Debug.Log("NODE " + GetType());
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

    }
}