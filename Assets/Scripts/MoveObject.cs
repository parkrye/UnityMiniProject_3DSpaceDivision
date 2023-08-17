using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    enum State { Idle, Find, Look, Hide, SeekAndHideA, SeekAndHideB }
    [SerializeField] State state;

    [SerializeField] SpaceManager spaceManager;
    [SerializeField] MoveObject otherObject;
    [SerializeField] List<int> exceptSpaceIndexList;

    [SerializeField] int placeSpaceindex, workCount, workLimit;
    public int PlacedSpaceIndex { get { return placeSpaceindex; } set { placeSpaceindex = value; } }
    [SerializeField] bool finding, looking, hiding;
    [SerializeField] Vector3 movePosition;
    [SerializeField] float speed, movePosMinDiff, movePosMaxDiff;
    public Vector3 Position { get { return transform.position + Vector3.up; } }

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

    public void OnSeekAndHideAbuttonClicked()
    {
        Idle();
        state = State.SeekAndHideA;
    }

    public void OnSeekAndHideBbuttonClicked()
    {
        Idle();
        state = State.SeekAndHideB;
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
            case State.SeekAndHideA:
                Find(true);
                break;
            case State.SeekAndHideB:
                Hide(true);
                break;
        }
    }

    void Idle()
    {
        exceptSpaceIndexList = new();
        finding = false;
        looking = false;
        hiding = false;
        workCount = 0;
        workLimit = spaceManager.WholeSpaces.Count;
    }

    void Find(bool isSeekAndHide = false)
    {
        if (spaceManager.WholeSpaces[PlacedSpaceIndex].PassableSpaces.Contains(otherObject.PlacedSpaceIndex) && Vector3.Distance(movePosition, Position) <= movePosMaxDiff)
        {
            //Debug.Log("Find Object");
            if (!isSeekAndHide)
                state = State.Idle;
            else
                workCount = 0;
            return;
        }

        if (!finding)
        {
            workCount++;
            if (workCount > workLimit)
            {
                if (!isSeekAndHide)
                    state = State.Idle;
                else
                    workCount = 0;
                return;
            }

            int nearIndex;
            if (PlacedSpaceIndex == otherObject.PlacedSpaceIndex)
            {
                nearIndex = spaceManager.GetNearestSpace(otherObject.PlacedSpaceIndex, spaceManager.WholeSpaces[PlacedSpaceIndex].PassableSpaces, exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }
            else
            {
                nearIndex = spaceManager.GetNearestSpace(otherObject.PlacedSpaceIndex, spaceManager.GetTwoWaySpaces(PlacedSpaceIndex, otherObject.PlacedSpaceIndex), exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }

            movePosition = spaceManager.WholeSpaces[nearIndex].Position;
            if (!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            finding = true;
        }
        else
        {
            if (Vector3.Distance(movePosition, Position) <= movePosMinDiff)
            {
                finding = false;
                return;
            }

            transform.Translate(speed * Time.deltaTime * (movePosition - Position).normalized, UnityEngine.Space.World);
        }
    }

    void Look()
    {
        if (!looking)
        {
            workCount++;
            if (workCount > workLimit)
            {
                state = State.Idle;
                return;
            }

            int nearIndex;
            if (PlacedSpaceIndex == otherObject.PlacedSpaceIndex)
            {
                nearIndex = spaceManager.GetNearestSpace(PlacedSpaceIndex, spaceManager.WholeSpaces[PlacedSpaceIndex].PassableSpaces, exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }
            else
            {
                nearIndex = spaceManager.GetNearestSpace(otherObject.PlacedSpaceIndex, spaceManager.GetTwoWaySpaces(PlacedSpaceIndex, otherObject.PlacedSpaceIndex), exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }

            movePosition = spaceManager.WholeSpaces[nearIndex].Position;
            if (!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            looking = true;
        }

        else
        {
            if (spaceManager.WholeSpaces[PlacedSpaceIndex].ImpassableSpaces.Contains(otherObject.PlacedSpaceIndex))
            {
                if (Vector3.Distance(movePosition, Position) < movePosMinDiff)
                {
                    looking = false;
                    return;
                }
            }
            else
            {
                //Debug.Log($"{name} Look at Object");
                //Debug.Log($"{name} Space {PlacedSpaceIndex} and {otherObject.PlacedSpaceIndex} is Passable? : {spaceManager.WholeSpaces[PlacedSpaceIndex].PassableSpaces.Contains(otherObject.placeSpaceindex)}");
                state = State.Idle;
                return;
            }

            transform.Translate(speed * Time.deltaTime * (movePosition - Position).normalized, UnityEngine.Space.World);
        }
    }

    void Hide(bool isSeekAndHide = false)
    {
        if (!hiding)
        {
            workCount++;
            if (workCount > workLimit)
            {
                if (!isSeekAndHide)
                    state = State.Idle;
                else
                    workCount = 0;
                return;
            }

            int nearIndex;
            if (PlacedSpaceIndex == otherObject.PlacedSpaceIndex)
            {
                nearIndex = spaceManager.GetNearestSpace(PlacedSpaceIndex, spaceManager.WholeSpaces[PlacedSpaceIndex].PassableSpaces, exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }
            else
            {
                nearIndex = spaceManager.GetNearestSpace(PlacedSpaceIndex, spaceManager.GetOneWaySpaces(PlacedSpaceIndex, otherObject.PlacedSpaceIndex), exceptSpaceIndexList);
                if (nearIndex < 0 || !spaceManager.WholeSpaces[nearIndex].Usable)
                {
                    if (!exceptSpaceIndexList.Contains(nearIndex))
                        exceptSpaceIndexList.Add(nearIndex);
                    return;
                }
            }

            movePosition = spaceManager.WholeSpaces[nearIndex].Position;
            if (!exceptSpaceIndexList.Contains(nearIndex))
                exceptSpaceIndexList.Add(nearIndex);
            hiding = true;
        }
        else
        {
            if (spaceManager.WholeSpaces[PlacedSpaceIndex].ImpassableSpaces.Contains(otherObject.PlacedSpaceIndex))
            {
                //Debug.Log($"{name} Hide from Object");
                //Debug.Log($"{name} Space {PlacedSpaceIndex} and {otherObject.PlacedSpaceIndex} is ImPassable? : {spaceManager.WholeSpaces[PlacedSpaceIndex].ImpassableSpaces.Contains(otherObject.placeSpaceindex)}");

                if (!isSeekAndHide)
                    state = State.Idle;
                else
                    workCount = 0;
                return;
            }
            else
            {
                if (Vector3.Distance(movePosition, Position) < movePosMinDiff)
                {
                    hiding = false;
                    return;
                }
            }

            transform.Translate(speed * Time.deltaTime * (movePosition - Position).normalized, UnityEngine.Space.World);
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
