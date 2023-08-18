using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveObject : MonoBehaviour
{
    enum State { Idle, Find, Look, Hide, SeekAndHideA, SeekAndHideB }
    [SerializeField] State state;

    [SerializeField] SpaceManager spaceManager;
    [SerializeField] MoveObject otherObject;

    [SerializeField] int placeSpaceindex;
    public int PlacedSpaceIndex { get { return placeSpaceindex; } set { placeSpaceindex = value; } }
    [SerializeField] bool finding, looking, hiding, moving;
    [SerializeField] float speed;
    [SerializeField] int moveSpaceIndex;

    Stack<int> moveSpaceIndexStack;
    public Vector3 Position { get { return transform.position + Vector3.up; } }

    public void OnFindButtonClicked()
    {
        state = State.Find;
    }

    public void OnLookButtonClicked()
    {
        state = State.Look;
    }

    public void OnHideButtonClicked()
    {
        state = State.Hide;
    }

    public void OnSeekAndHideAbuttonClicked()
    {
        state = State.SeekAndHideA;
    }

    public void OnSeekAndHideBbuttonClicked()
    {
        state = State.SeekAndHideB;
    }

    void Awake()
    {
        moveSpaceIndexStack = new();
    }

    void Start()
    {
        StartCoroutine(URoutine());
    }

    IEnumerator URoutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
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
            yield return null;
        }
    }

    void Idle()
    {
        if(finding || looking || hiding || moving)
        {
            moveSpaceIndexStack = new();
            finding = false;
            looking = false;
            hiding = false;
            moving = false;
        }
    }

    void Find(bool isSeekAndHide = false)
    {
        if (finding && moveSpaceIndexStack.Count == 0)
        {
            if (!isSeekAndHide)
                state = State.Idle;
            finding = false;
            return;
        }

        if (!finding)
        {
            moveSpaceIndexStack = SpaceBaseAstar.PathFinding(spaceManager, PlacedSpaceIndex, otherObject.PlacedSpaceIndex);
            finding = true;
        }
        else
        {
            if (!moving)
            {
                moveSpaceIndex = moveSpaceIndexStack.Pop();
                moving = true;
            }
            else
            {
                if(PlacedSpaceIndex == moveSpaceIndex)
                {
                    moving = false;
                }
                else
                {
                    transform.Translate(speed * Time.deltaTime * (spaceManager.WholeSpaces[moveSpaceIndex].Position - Position));
                }
            }
        }
    }

    void Look()
    {
        if (looking && moveSpaceIndexStack.Count == 0)
        {
            state = State.Idle;
            looking = false;
            return;
        }

        if (!looking)
        {
            moveSpaceIndexStack = SpaceBaseAstar.PathFinding(spaceManager, PlacedSpaceIndex, otherObject.PlacedSpaceIndex);
            int prev = PlacedSpaceIndex;
            foreach (int index in moveSpaceIndexStack)
            {
                Debug.Log($"{prev} => {index} : {spaceManager.WholeSpaces[prev].PassableSpaces.Contains(index)}");
                prev = index;
            }
            looking = true;
        }
        else
        {
            if (!moving)
            {
                moveSpaceIndex = moveSpaceIndexStack.Pop();
                moving = true;
            }
            else
            {
                if (PlacedSpaceIndex == moveSpaceIndex)
                {
                    moving = false;
                }
                else
                {
                    transform.Translate(speed * Time.deltaTime * (spaceManager.WholeSpaces[moveSpaceIndex].Position - Position));
                }
            }
        }
    }

    void Hide(bool isSeekAndHide = false)
    {
        if (hiding && moveSpaceIndexStack.Count == 0)
        {
            if (!isSeekAndHide)
                state = State.Idle;
            hiding = true;
            return;
        }

        if (!hiding)
        {
            hiding = true;
        }
        else
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        Space space = other.GetComponent<Space>();
        if (space != null && space.Usable)
        {
            StartCoroutine(ChangePlacedSpaceIndexRoutine(space.SpaceIndex));
        }
    }

    IEnumerator ChangePlacedSpaceIndexRoutine(int value)
    {
        yield return new WaitForSeconds(0.5f);
        PlacedSpaceIndex = value;
    }
}
