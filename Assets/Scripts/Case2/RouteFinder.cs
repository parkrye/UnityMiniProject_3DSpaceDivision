using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class RouteFinder : MonoBehaviour
    {
        [SerializeField] SpaceDivider spaceDivider;
        [SerializeField] Space root;
        [SerializeField] List<Space> leaves;
        [SerializeField] MoveObject red, yellow;
        [SerializeField] float speed;

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
                    if (child.IsLeaf && child.Usable && !leaves.Contains(child))
                    {
                        leaves.Add(child);
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
    
        public void FindRoute(bool isRed)
        {
            if (isRed)
            {
                Space among = FindTwoWayRoute(red.CurSpace, yellow.CurSpace);
                StartCoroutine(MoveObject(isRed, red.CurSpace, among));
            }
            else
            {
                Space among = FindTwoWayRoute(yellow.CurSpace, red.CurSpace);
                StartCoroutine(MoveObject(isRed, yellow.CurSpace, among));
            }
        }
    
        public void Hideoute(bool isRed)
        {
            if (isRed)
            {
                Space among = FindOneWayRoute(red.CurSpace, yellow.CurSpace);
                StartCoroutine(MoveObject(isRed, red.CurSpace, among));
            }
            else
            {
                Space among = FindOneWayRoute(yellow.CurSpace, red.CurSpace);
                StartCoroutine(MoveObject(isRed, yellow.CurSpace, among));
            }
        }

        IEnumerator MoveObject(bool isRed, params Space[] route)
        {
            int step = 0;
            while (step < route.Length)
            {
                if (isRed)
                {
                    red.transform.Translate((route[step].transform.position - red.transform.position).normalized * Time.fixedDeltaTime * speed, UnityEngine.Space.World);
                    red.transform.LookAt((route[step].transform.position));
                    if (Vector3.Distance(route[step].transform.position, red.transform.position) < 0.2f)
                    {
                        step++;
                    }
                }
                else
                {
                    yellow.transform.Translate((route[step].transform.position - yellow.transform.position).normalized * Time.fixedDeltaTime * speed, UnityEngine.Space.World);
                    yellow.transform.LookAt((route[step].transform.position));
                    if (Vector3.Distance(route[step].transform.position, yellow.transform.position) < 0.2f)
                    {
                        step++;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        Space FindOneWayRoute(Space from,  Space to)
        {
            float leastDistacne = float.MaxValue;
            Space result = null;

            for(int i = 0; i < from.Passable.Count; i++)
            {
                if (to.ImPassable.Contains(from.Passable[i]))
                {
                    float distacne = Vector3.Magnitude(from.Passable[i].transform.position - from.transform.position);
                    if (distacne < leastDistacne)
                    {
                        leastDistacne = distacne;
                        result = from.Passable[i];
                    }
                }
            }

            return result;
        }

        Space FindTwoWayRoute(Space from, Space to)
        {
            float leastDistacne = float.MaxValue;
            Space result = null;

            for (int i = 0; i < from.Passable.Count; i++)
            {
                if (to.Passable.Contains(from.Passable[i]))
                {
                    float distacne = Vector3.Magnitude(from.Passable[i].transform.position - from.transform.position) * 0.1f + Vector3.Magnitude(from.Passable[i].transform.position - to.transform.position) * 0.1f;
                    if (distacne < leastDistacne)
                    {
                        leastDistacne = distacne;
                        result = from.Passable[i];
                    }
                }
            }

            return result;
        }
    }
}