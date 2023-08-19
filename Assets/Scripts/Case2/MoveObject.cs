using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class MoveObject : MonoBehaviour
    {
        [SerializeField] SpaceDivider spaceDivider;
        [SerializeField] Space nowSpace;
        [SerializeField] MoveObject otherObject;
        [SerializeField] bool readyToDrawRoute;
        [SerializeField] List<Space> moveList;
        public Space NowSpace { get { return nowSpace; } }

        public void CreateRoute()
        {
            moveList = new();

            Stack<Space> myRoute = new();
            Space myCheckSpace = NowSpace;
            while (!myCheckSpace.IsRoot)
            {
                myRoute.Push(myCheckSpace);
                myCheckSpace = myCheckSpace.Parent;
            }

            Stack<Space> otRoute = new();
            Space otCheckSpace = otherObject.NowSpace;
            while (!otCheckSpace.IsRoot)
            {
                otRoute.Push(otCheckSpace);
                otCheckSpace = otCheckSpace.Parent;
            }

            Space myPop = myRoute.Pop();
            Space otPop = otRoute.Pop();
            while (myPop == otPop && (myRoute.Count > 0 && otRoute.Count > 0))
            {
                myPop = myRoute.Pop();
                otPop = otRoute.Pop();
            }
            myRoute.Push(myPop);
            otRoute.Push(otPop);

            while (myRoute.Count > 0)
            {
                moveList.Insert(0, myRoute.Pop());
            }
            while (otRoute.Count > 0)
            {
                moveList.Add(otRoute.Pop());
            }

            readyToDrawRoute = true;
        }

        void OnTriggerEnter(Collider other)
        {
            Space space = other.GetComponent<Space>();
            if(space != null)
            {
                if(nowSpace == null)
                    nowSpace = space;
                else
                {
                    if(space.Depth > nowSpace.Depth)
                        nowSpace = space;
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            if (readyToDrawRoute)
            {
                for (int i = 0; i < moveList.Count; i++)
                {
                    Gizmos.DrawWireSphere(moveList[i].transform.position, 1f);
                    if(i < moveList.Count - 1)
                        Gizmos.DrawLine(moveList[i].transform.position, moveList[i + 1].transform.position);
                }
            }
        }
    }
}
