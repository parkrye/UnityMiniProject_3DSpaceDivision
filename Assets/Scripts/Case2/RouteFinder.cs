using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class RouteFinder : MonoBehaviour
    {
        [SerializeField] SpaceDivider spaceDivider;
        [SerializeField] Space root;
        [SerializeField] List<Space> leaves;

        public void CreateRoute()
        {
            root = spaceDivider.RootSpace;
            leaves = new List<Space>();
            AddLeaf(root);
            SetRoute();
        }

        void AddLeaf(Space now)
        {
            if (!now.IsLeaf)
            {
                foreach(Space child in now.Children)
                {
                    if (child.IsLeaf && child.Usable && !leaves.Contains(now))
                    {
                        leaves.Add(now);
                    }
                    else
                    {
                        AddLeaf(child);
                    }
                }
            }
        }

        void SetRoute()
        {
            for(int i = 0; i < leaves.Count; i++)
            {
                for(int j = i + 1; j < leaves.Count; j++)
                {
                    if (Physics.Raycast(leaves[i].transform.position, 
                        (leaves[j].transform.position - leaves[i].transform.position).normalized, 
                        (leaves[j].transform.position - leaves[i].transform.position).magnitude, 
                        LayerMask.GetMask("Obstacle")))
                    {
                        leaves[i].ImPassable.Add(leaves[j]);
                        leaves[j].ImPassable.Add(leaves[i]);
                    }
                    else
                    {
                        leaves[i].Passable.Add(leaves[j]);
                        leaves[j].Passable.Add(leaves[i]);
                    }
                }
            }
        }
    }
}