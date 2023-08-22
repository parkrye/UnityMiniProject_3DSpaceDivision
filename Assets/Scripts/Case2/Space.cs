using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class Space : MonoBehaviour
    {
        [SerializeField] List<Space> childrenSpaces;
        [SerializeField] Space parentSpace;
        [SerializeField] float size, colorModifier;
        [SerializeField] int depth;
        [SerializeField] bool visible, usable;
        [SerializeField] int drawGizmoLevel;
        [SerializeField] Color gizmoColor;
        [SerializeField] SpaceDivider divider;
        [SerializeField] bool[] directions; // right, up, foward

        [SerializeField] List<Space> passable, imPassable;

        public List<Space> Children { get { return childrenSpaces; } } 
        public Space Parent { get { return parentSpace; } }
        public List<Space> Brothers { get { return Parent.Children; } }

        public bool IsRoot { get { return parentSpace == null; } }
        public bool IsLeaf { get { return childrenSpaces.Count == 0; } }
        public bool Usable { get { return usable; } }
        public float Size { get { return size; } }
        public int Depth { get { return depth; } }
        public bool[] Direction { get { return directions; } }
        public List<Space> Passable { get { return passable; } set { passable = value; } }
        public List<Space> ImPassable {  get { return imPassable; } set { imPassable = value; } }

        void Awake()
        {
            childrenSpaces = new();
        }

        public void Initialize(SpaceDivider spaceDivider, float _size, int _depth, Space _parent = null, bool right = false, bool up = false, bool foward = false)
        {
            divider = spaceDivider;
            size = _size;
            depth = _depth;
            parentSpace = _parent;
            colorModifier = 1 / Mathf.Log(Depth, 2);

            transform.localScale = Vector3.one * _size;

            directions = new bool[3];
            directions[0] = right;
            directions[1] = up;
            directions[2] = foward;

            passable = new List<Space>();
            imPassable = new List<Space>();
        }

        public void AddChild(Space space)
        {
            childrenSpaces.Add(space);
        }

        public bool CheckDivide()
        {
            usable = true;
            if (Physics.CheckBox(transform.position, 0.5f * size * Vector3.one, Quaternion.identity, LayerMask.GetMask("Obstacle")))
            {
                usable = false;
            }
            return !usable;
        }

        void OnDrawGizmos()
        {
            if (divider.DrawGizmoLevel != depth)
                return;
            if (Usable)
                gizmoColor = Color.green;
            else
                gizmoColor = Color.red;
            gizmoColor *= colorModifier;

            Gizmos.color = gizmoColor;
            
            Gizmos.DrawWireCube(transform.position, Vector3.one * size);
        }
    }
}