using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Case2
{
    public class MoveObject : MonoBehaviour
    {
        [SerializeField] SpaceDivider spaceDivider;
        [SerializeField] Space nowSpace;
        [SerializeField] MoveObject otherObject;
        Stack<Space> moveStack;
        public Space NowSpace { get { return nowSpace; } }

        public void StartWork()
        {
            Chase();
        }

        void Chase()
        {
            moveStack = new();

            Stack<Space> myRoute = new();
            Space myCheckSpace = NowSpace;
            while (!myCheckSpace.IsRoot)
            {
                myRoute.Push(myCheckSpace);
                myCheckSpace = myCheckSpace.Parent;
            }

            Queue<Space> otRoute = new();
            Space otCheckSpace = otherObject.NowSpace;
            while (!otCheckSpace.IsRoot)
            {
                otRoute.Enqueue(otCheckSpace);
                otCheckSpace = otCheckSpace.Parent;
            }

            while (otRoute.Count > 0)
                moveStack.Push(otRoute.Dequeue());
            while(myRoute.Count > 0)
                moveStack.Push(myRoute.Pop());

            StartCoroutine(MoveRoutine());
        }

        IEnumerator MoveRoutine()
        {
            while (moveStack.Count > 0)
            {
                transform.Translate((moveStack.Peek().transform.position - transform.position) * Time.deltaTime);
                if (Vector3.Distance(moveStack.Peek().transform.position, transform.position) < 0.1f)
                    moveStack.Pop();
                yield return null;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Space space = other.GetComponent<Space>();
            if(space != null)
            {
                nowSpace = space;
            }
        }
    }
}
