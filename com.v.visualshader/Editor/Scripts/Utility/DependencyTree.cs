using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace V.VVS
{

    public class DependencyTree<T> where T : IDependable<T>
    {
        public List<IDependable<T>> tree;

        public DependencyTree()
        {
            tree = new List<IDependable<T>>();
        }

        public void Add(params IDependable<T>[] deps)
        {

        }

        public void Add(IDependable<T> dep)
        {
            AddUnique(dep);  //Add A Unique Dependency 

            //Add Dependencies of the parameter's dependency.

            foreach (IDependable<T> d in dep.Dependencies)
            {
                AddUnique(d);
            }
        }

        private void AddUnique(IDependable<T> dep)
        {
            if (!tree.Contains(dep))
            {
                tree.Add(dep);
            }
        }

        public void Sort()
        {
            AssignDepthValues();
            SortByDepth();
        }

        private void MoveUpNode(IDependable<T> dp,bool initial)
        {
            //Increase the value of the Depth
            if (!initial)
                dp.Depth++;

            //Once we move up the Node.  We need move up it all dependencies reciersively
            foreach (IDependable<T> d in dp.Dependencies)
            {
                if (d.Depth <= dp.Depth)
                {
                    MoveUpNode(d, initial: false);
                }
            }
        }

        private void AssignDepthValues()
        {
            ResetNodeDepths();
            foreach (IDependable<T> dp in tree)
            {
                MoveUpNode(dp, initial: true);
            }
        }

        private void SortByDepth()
        {
            tree.OrderBy(o => o.Depth).ToList();
        }

        private void ResetNodeDepths()
        {
            foreach (IDependable<T> dp in tree)
            {
                dp.Depth = 0;
            }
        }

        public List<List<T>> GetDependenciesByGroup(out int maxWidth)
        {
            List<List<T>> group = new List<List<T>>();
            maxWidth = 0;

            IEnumerable<IGrouping<int,IDependable<T>>> itOfGroup  = tree.GroupBy(p => p.Depth);       
            IEnumerable<IDependable<T>> firstOfEachGroup                  = itOfGroup.Select(g => g.First());
            var groupCount     = firstOfEachGroup.Count();

            for (int i = 0; i < groupCount; i++)
            {
                IEnumerable<T> listIt = tree.Select(x => (T)x);                 //Select All
                IEnumerable<T> elementIt = listIt.Where(x => x.Depth == i);     //Get a list of element where it's Depth == i
                List<T> listOfDependency = elementIt.ToList();                  //Convert enumerator to a List
                group.Add(listOfDependency);                                    //Copy List
                maxWidth = Mathf.Max(maxWidth, group[i].Count);
            }
            return group;
        }

    }

    public interface IDependable<T>
    {
        int Depth { get; set;}

        List<T> Dependencies { get; set; }

        void AddDependency(T dp);
    }

}