using System.Collections.Generic;
using UnityEngine;

namespace Case3
{
    public class Space : MonoBehaviour
    {
        [SerializeField] List<Space> childrenSpaces;
        [SerializeField] Space parentSpace;
        [SerializeField] float size, minSize, colorModifier;
        [SerializeField] int depth;
        [SerializeField] bool visible;
        [SerializeField] int drawGizmoLevel;
        [SerializeField] Color gizmoColor;
        [SerializeField] SpaceDivider divider;

        public List<Space> Children { get { return childrenSpaces; } } 
        public Space Parent { get { return parentSpace; } }

        public bool IsRoot { get { return parentSpace == null; } }
        public bool IsLeaf { get { return childrenSpaces.Count == 0; } }
        public float Size { get { return size; } }
        public int Depth { get { return depth; } }

        void Awake()
        {
            childrenSpaces = new();
        }

        public void Initialize(SpaceDivider spaceDivider, float _size, float _minSize, int _depth, Space _parent = null)
        {
            name += GetHashCode();
            divider = spaceDivider;
            size = _size;
            minSize = _minSize;
            depth = _depth;
            parentSpace = _parent;
            colorModifier = 1 / Mathf.Log(Depth, 2);

            transform.localScale = Vector3.one * _size;
        }

        public void AddChild(Space space)
        {
            childrenSpaces.Add(space);
        }

        public bool CheckDivide()
        {
            for(int i = 0; i < divider.Obstacles.Length; i++)
            {
                Vector3 position = divider.Obstacles[i].transform.position;

                if (Mathf.Abs(position.x) <= Mathf.Abs(transform.position.x + transform.lossyScale.x * 0.5f) &&
                    Mathf.Abs(position.y) <= Mathf.Abs(transform.position.y + transform.lossyScale.y * 0.5f) &&
                    Mathf.Abs(position.z) <= Mathf.Abs(transform.position.z + transform.lossyScale.z * 0.5f)    )
                {
                    Debug.Log($"{divider.Obstacles[i].name} is in {name}");
                    return true;
                }
            }

            return false;
        }

        void OnDrawGizmos()
        {
            if (divider.DrawGizmoLevel != depth)
                return;
            if (IsLeaf)
                gizmoColor = Color.green;
            else
                gizmoColor = Color.red;
            gizmoColor *= colorModifier;

            Gizmos.color = gizmoColor;
            
            Gizmos.DrawWireCube(transform.position, Vector3.one * size);
        }
    }
}