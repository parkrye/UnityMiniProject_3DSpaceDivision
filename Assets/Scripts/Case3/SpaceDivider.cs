using System.Collections;
using UnityEngine;

namespace Case3
{
    public class SpaceDivider : MonoBehaviour
    {
        [SerializeField] float maximumSize, minimumSize;
        [SerializeField] Space spacePrefab, rootSpace;
        [SerializeField] int drawGizmoLevel;
        [SerializeField] GameObject[] obstacles;
        public int DrawGizmoLevel { get { return drawGizmoLevel;} }
        public Space RootSpace { get { return spacePrefab; } }
        public GameObject[] Obstacles { get { return obstacles; } }

        void Awake()
        {
            obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            rootSpace = Instantiate(spacePrefab, transform.position, Quaternion.identity, transform);
            rootSpace.Initialize(this, maximumSize, minimumSize, 1);

            StartCoroutine(DivideRoutine(rootSpace));
        }

        IEnumerator DivideRoutine(Space parentSpace)
        {
            float size = parentSpace.Size * 0.5f;
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Space space = Instantiate(spacePrefab, parentSpace.transform.position + (Vector3.right * x + Vector3.up * y + Vector3.forward * z) * size * 0.5f, Quaternion.identity, transform);
                        space.Initialize(this, size, minimumSize, parentSpace.Depth + 1, parentSpace);
                        if (space.CheckDivide())
                        {
                            if (size <= minimumSize)
                                continue;
                            StartCoroutine(DivideRoutine(space));
                        }
                        parentSpace.AddChild(space);
                    }
                }
            }
            yield return null;
        }

        public void OnDrawGizmoButton(int level)
        {
            drawGizmoLevel += level;
            if(drawGizmoLevel < 0)
                drawGizmoLevel = 0;
            if (drawGizmoLevel > maximumSize / minimumSize)
                drawGizmoLevel = (int)(maximumSize / minimumSize);
        }
    }
}
