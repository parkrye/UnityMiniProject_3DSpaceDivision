using UnityEngine;

namespace Case2
{
    public class MoveObject : MonoBehaviour
    {
        [SerializeField] Space curSpace;
        public Space CurSpace { get { return curSpace; } }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Space>() != null)
            {
                Space nowSpace = curSpace = other.GetComponent<Space>();
                if (curSpace != null)
                {
                    if(curSpace.Depth < nowSpace.Depth)
                        curSpace = nowSpace;
                }
                else
                    curSpace = nowSpace;
            }
        }
    }

}