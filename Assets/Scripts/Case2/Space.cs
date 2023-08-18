using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class Space : MonoBehaviour
    {
        [SerializeField] List<Space> childrenSpaces;
        [SerializeField] Space parentSpace;
        [SerializeField] float size, minSize;
        [SerializeField] bool passable;
        [SerializeField] int depth;

        public List<Space> Children { get { return childrenSpaces; } } 
        public Space Parent { get { return parentSpace; } }

        public bool IsRoot { get { return parentSpace == null; } }
        public bool IsLeaf { get { return childrenSpaces.Count == 0; } }
        public float Size { get { return size; } }
        public bool Passable { get { return passable; } }
        public int Depth { get { return depth; } }

        void Awake()
        {
            childrenSpaces = new();
        }

        public void Initialize(float _size, float _minSize, int _depth, Space _parent = null)
        {
            size = _size;
            minSize = _minSize;
            depth = _depth;
            parentSpace = _parent;

            transform.localScale /= depth;
        }

        public void AddChild(Space space)
        {
            childrenSpaces.Add(space);
        }

        public bool CheckDivide()
        {
            if (Physics.CheckBox(transform.position, Vector3.one * size * 0.5f, Quaternion.identity, LayerMask.GetMask("Obstacle")))
            {
                passable = false;
                if (size <= minSize)
                    return false;
                return true;
            }
            passable = true;
            return false;
        }

        void OnDrawGizmos()
        {
            if(passable)
                Gizmos.color = Color.green * 0.2f;
            else
                Gizmos.color = Color.red * 0.2f;
            Gizmos.DrawWireCube(transform.position, Vector3.one * size);
        }
    }
}