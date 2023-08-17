using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    enum State { Idle, Find, Look, Hide }
    [SerializeField] State state;

    [SerializeField] SpaceDivinder spaceDivinder;
    [SerializeField] MoveObject otherObject;
    [SerializeField] List<int> exceptSpaceIndexList;

    [SerializeField] int placeSpaceindex;
    public int PlacedSpaceIndex { get { return placeSpaceindex; } set { placeSpaceindex = value; } }
    [SerializeField] bool finding, looking, hiding;
    [SerializeField] Vector3 movePosition;

    public void OnFindButtonClicked()
    {
        Idle();
        state = State.Find;
    }

    public void OnLookButtonClicked()
    {
        Idle();
        state = State.Look;
    }

    public void OnHideButtonClicked()
    {
        Idle();
        state = State.Hide;
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.Idle:
                Idle();
                break;
            case State.Find:
                Find();
                break;
            case State.Look:
                Look();
                break;
            case State.Hide:
                Hide();
                break;
        }
    }

    void Idle()
    {
        exceptSpaceIndexList = new();
        finding = false;
        looking = false;
        hiding = false;
    }

    void Find()
    {
        if (Vector3.Distance(otherObject.transform.position, transform.position) < 1f)
        {
            Debug.Log("Find Object");
            state = State.Idle;
            return;
        }

        if (!finding)
        {
            int nearIndex = spaceDivinder.WholeSpaces[PlacedSpaceIndex].GetNearestPassableSpace(spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].Position, exceptSpaceIndexList);
            if (nearIndex < 0)
            {
                if (!exceptSpaceIndexList.Contains(PlacedSpaceIndex))
                    exceptSpaceIndexList.Add(PlacedSpaceIndex);
                return;
            }
            if (!spaceDivinder.WholeSpaces[nearIndex].Usable)
            {
                if (!exceptSpaceIndexList.Contains(nearIndex))
                    exceptSpaceIndexList.Add(nearIndex);
                return;
            }
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            if(!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            finding = true;
        }
        else
        {
            if (Vector3.Distance(otherObject.movePosition, transform.position) < 1f)
            {
                finding = false;
                return;
            }

            transform.Translate((movePosition - transform.position) * Time.deltaTime, UnityEngine.Space.World);
        }
    }

    void Look()
    {
        if (!looking)
        {
            int nearIndex = spaceDivinder.WholeSpaces[PlacedSpaceIndex].GetNearestPassableSpace(spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].Position, exceptSpaceIndexList);
            if(nearIndex < 0)
            {
                if (!exceptSpaceIndexList.Contains(PlacedSpaceIndex))
                    exceptSpaceIndexList.Add(PlacedSpaceIndex);
                return;
            }
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            if (!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            if (!Physics.Raycast(transform.position, (movePosition - transform.position).normalized, Vector3.Distance(movePosition, transform.position), LayerMask.GetMask("Obstacle")))
                looking = true;
        }
        else
        {
            if(Vector3.Distance(movePosition, transform.position) < 0.1f)
            {
                if(Physics.Raycast(transform.position, otherObject.transform.position - transform.position, Vector3.Distance(otherObject.transform.position, transform.position), LayerMask.GetMask("Obstacle")))
                    looking = false;
                else
                {
                    Debug.Log("Look at Object");
                    state = State.Idle;
                }
                return;
            }

            transform.Translate((movePosition - transform.position) * Time.deltaTime, UnityEngine.Space.World);
        }
    }

    void Hide()
    {
        if (!hiding)
        {
            int nearIndex = spaceDivinder.WholeSpaces[PlacedSpaceIndex].GetNearestImpassableSpace(spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].Position, exceptSpaceIndexList);
            if (nearIndex < 0)
            {
                if (!exceptSpaceIndexList.Contains(PlacedSpaceIndex))
                    exceptSpaceIndexList.Add(PlacedSpaceIndex);
                return;
            }
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            if (!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            if (!Physics.Raycast(transform.position, (movePosition - transform.position).normalized, Vector3.Distance(movePosition, transform.position), LayerMask.GetMask("Obstacle")))
                hiding = true;
        }
        else
        {
            if (Vector3.Distance(movePosition, transform.position) < 0.1f)
            {
                if (Physics.Raycast(transform.position, otherObject.transform.position - transform.position, Vector3.Distance(otherObject.transform.position, transform.position), LayerMask.GetMask("Obstacle")))
                {
                    Debug.Log("Hide from Object");
                    state = State.Idle;
                }
                else
                    hiding = false;
                return;
            }

            transform.Translate((movePosition - transform.position) * Time.deltaTime, UnityEngine.Space.World);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Space space = other.GetComponent<Space>();
        if (space != null)
        {
            PlacedSpaceIndex = space.SpaceIndex;
        }
    }
}
