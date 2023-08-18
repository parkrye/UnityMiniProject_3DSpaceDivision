using System.Collections.Generic;
using UnityEngine;

namespace Case1
{
    public class Space : MonoBehaviour
    {
        [SerializeField] int spaceIndex;
        [SerializeField] bool usable;
        [SerializeField] List<int> passableSpaces, impassableSpaces;
        public int SpaceIndex { get { return spaceIndex; } set { spaceIndex = value; } }
        public bool Usable { get { return usable; } }
        public List<int> PassableSpaces { get { return passableSpaces; } set { passableSpaces = value; } }
        public List<int> ImpassableSpaces { get { return impassableSpaces; } set { impassableSpaces = value; } }
        public Vector3 Position { get { return transform.position + Vector3.up; } }

        void Awake()
        {
            usable = true;
        }

        public void Initialize(int index)
        {
            SpaceIndex = index;
            PassableSpaces = new List<int>();
            ImpassableSpaces = new List<int>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (Usable)
            {
                if (1 << other.gameObject.layer == LayerMask.GetMask("Obstacle"))
                {
                    usable = false;
                }
            }
        }
    }

}
