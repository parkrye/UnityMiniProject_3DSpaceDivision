using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    enum State { Idle, Find, Look, Hide }
    [SerializeField] State state;

    [SerializeField] SpaceDivinder spaceDivinder;
    [SerializeField] MoveObject otherObject;
    [SerializeField] List<int> exceptSpaceIndexList;

    public int PlacedSpaceIndex {  get; private set; }
    [SerializeField] bool finding, looking, hiding;
    [SerializeField] Vector3 movePosition;

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
        if (Vector3.SqrMagnitude(otherObject.transform.position - transform.position) < 1f)
        {
            state = State.Idle;
            return;
        }

        if (!finding)
        {
            int nearIndex = spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].GetNearestPassableSpace(spaceDivinder.WholeSpaces[PlacedSpaceIndex].Position, exceptSpaceIndexList);
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            exceptSpaceIndexList.Add(nearIndex);
            finding = true;
        }
        else
        {
            if (Vector3.SqrMagnitude(movePosition - transform.position) < 0.1f)
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
            int nearIndex = spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].GetNearestPassableSpace(spaceDivinder.WholeSpaces[PlacedSpaceIndex].Position, exceptSpaceIndexList);
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            exceptSpaceIndexList.Add(nearIndex);
            looking = true;
        }
        else
        {
            if(Vector3.SqrMagnitude(movePosition - transform.position) < 0.1f)
            {
                if(Physics.Raycast(transform.position, otherObject.transform.position - transform.position, Vector3.Distance(otherObject.transform.position, transform.position), LayerMask.GetMask("Obstacle")))
                    looking = false;
                else
                    state = State.Idle;
                return;
            }

            transform.Translate((movePosition - transform.position) * Time.deltaTime, UnityEngine.Space.World);
        }
    }

    void Hide()
    {
        if (!hiding)
        {
            int nearIndex = spaceDivinder.WholeSpaces[otherObject.PlacedSpaceIndex].GetNearestImpassableSpace(spaceDivinder.WholeSpaces[PlacedSpaceIndex].Position, exceptSpaceIndexList);
            movePosition = spaceDivinder.WholeSpaces[nearIndex].Position;
            exceptSpaceIndexList.Add(nearIndex);
            hiding = true;
        }
        else
        {
            if (Vector3.SqrMagnitude(movePosition - transform.position) < 0.1f)
            {
                if (Physics.Raycast(transform.position, otherObject.transform.position - transform.position, Vector3.Distance(otherObject.transform.position, transform.position), LayerMask.GetMask("Obstacle")))
                    state = State.Idle;
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
