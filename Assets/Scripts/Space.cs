using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] int spaceIndex;
    [SerializeField] bool usable;
    [SerializeField] List<Space> passableSpaces, impassableSpaces;
    public int SpaceIndex { get { return spaceIndex; } set { spaceIndex = value; } }
    public bool Usable { get { return usable; } set { usable = value; } }
    public List<Space> PassableSpaces { get { return passableSpaces; } set { passableSpaces = value; } }
    public List<Space> ImpassableSpaces { get { return impassableSpaces; } set { impassableSpaces = value; } }
    public Vector3 Position { get { return transform.position; } }

    public void Initialize(int index)
    {
        SpaceIndex = index;
        Usable = true;
        PassableSpaces = new List<Space>();
        ImpassableSpaces = new List<Space>();
    }

    public int GetNearestPassableSpace(Vector3 targetTransform, List<int> exceptList = null)
    {
        int nearestIndex = SpaceIndex;
        float nearestDistance = float.MaxValue;

        for(int i = 0; i < PassableSpaces.Count; i++)
        {
            if (i == SpaceIndex)
                continue;
            if (exceptList != null && exceptList.Contains(PassableSpaces[i].SpaceIndex))
                continue;
            float distanceDiff = Vector3.SqrMagnitude(targetTransform - PassableSpaces[i].Position);
            if (distanceDiff > 0.1f && distanceDiff < nearestDistance)
            {
                nearestIndex = PassableSpaces[i].SpaceIndex;
                nearestDistance = distanceDiff;
            }
        }

        return nearestIndex;
    }

    public int GetNearestImpassableSpace(Vector3 targetTransform, List<int> exceptList = null)
    {
        int nearestIndex = SpaceIndex;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < ImpassableSpaces.Count; i++)
        {
            if (i == SpaceIndex)
                continue;
            if (exceptList != null && exceptList.Contains(ImpassableSpaces[i].SpaceIndex))
                continue;
            float distanceDiff = Vector3.SqrMagnitude(targetTransform - ImpassableSpaces[i].Position);
            if (distanceDiff > 0.1f && distanceDiff < nearestDistance)
            {
                nearestIndex = ImpassableSpaces[i].SpaceIndex;
                nearestDistance = distanceDiff;
            }
        }

        return nearestIndex;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Usable)
        {
            if (1 << other.gameObject.layer == LayerMask.GetMask("Obstacle"))
            {
                Usable = false;
            }
        }
    }
}
