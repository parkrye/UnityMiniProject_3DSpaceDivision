using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    [SerializeField] List<Space> wholeSpace;
    public List<Space> WholeSpaces { get { return wholeSpace; } set { wholeSpace = value; } }

    [SerializeField][Range(1f, 5f)] float spaceSize;
    [SerializeField][Range(2, 30)] int spaceLine;
    [SerializeField] GameObject spacePrefab;

    public int SpaceLine { get { return spaceLine; } }

    void Awake()
    {
        WholeSpaces = new List<Space>();
    }

    public void SettingSpaces()
    {
        int index = 0;
        for (int i = -spaceLine / 2; i < spaceLine / 2; i++)
        {
            for (int j = -spaceLine / 2; j < spaceLine / 2; j++)
            {
                Space space = Instantiate(spacePrefab, transform).GetComponent<Space>();
                space.transform.Translate((Vector3.forward * i + Vector3.right * j + Vector3.up * 0.5f) * spaceSize);
                space.transform.localScale = Vector3.one * spaceSize;
                space.Initialize(index);
                index++;
                WholeSpaces.Add(space);
            }
        }
    }

    // 시야를 구현하려면 raycast 필요?

    // 발상 1.
    // 각 위치로부터 외곽 지역에 raycast(obstacle)
    // hit1로부터 원 위치로 raycastAll(space)
    // hit2들 사이는 Passable
    // 남은 Space는 Impassable

    // 발상 2. => 가장 적은 raycast
    // 각 위치로부터 외곽 지역에 raycast(obstacle)
    // hit Space 번호로 브레즌햄
    // 남은 Space는 Impassable

    // 발상 3. => A* 기반 이동만 가능
    // 인접한 Space가 Usable한지 검사
    // Usable하다면 서로 Passable
    // 아니라면 서로 Impassable

    // i-n-1  i-1   i+n-1
    //  i-n    i     i+n
    // i-n+1  i+1   i+n+1

    public void SettingRoutes()
    {
        for (int i = 0; i < WholeSpaces.Count; i++)
        {

        }
    }

    void AddPassableOrNot(int from, int to)
    {
        if (WholeSpaces[from].Usable && WholeSpaces[to].Usable)
        {
            WholeSpaces[from].PassableSpaces.Add(WholeSpaces[to].SpaceIndex);
            WholeSpaces[to].PassableSpaces.Add(WholeSpaces[from].SpaceIndex);
        }
        else
        {
            WholeSpaces[from].ImpassableSpaces.Add(WholeSpaces[to].SpaceIndex);
            WholeSpaces[to].ImpassableSpaces.Add(WholeSpaces[from].SpaceIndex);
        }
    }

    /// <summary>
    /// 일방향 통행 공간 리스트를 반환
    /// </summary>
    /// <param name="passableSpaceIndex">이동 가능해야 하는 공간 인덱스</param>
    /// <param name="impassableSpaceIndex">이동 불가능해야 하는 공간 인덱스</param>
    /// <returns>일방향 통행 공간 리스트</returns>
    public List<int> GetOneWaySpaces(int passableSpaceIndex, int impassableSpaceIndex)
    {
        List<int> spaces = new();

        foreach (Space space in WholeSpaces)
        {
            if (!space.Usable)
                continue;
            if (WholeSpaces[passableSpaceIndex].ImpassableSpaces.Contains(space.SpaceIndex))
                continue;
            if (WholeSpaces[impassableSpaceIndex].PassableSpaces.Contains(space.SpaceIndex))
                continue;
            spaces.Add(space.SpaceIndex);
        }

        return spaces;
    }

    /// <summary>
    /// 쌍방향 통행 공간 인덱스 리스트를 반환
    /// </summary>
    /// <param name="passableSpaceAIndex">이동 가능해야 하는 공간1 인덱스</param>
    /// <param name="passableSpaceBIndex">이동 가능해야 하는 공간2 인덱스</param>
    /// <returns>쌍방향 통행 공간 인덱스 리스트</returns>
    public List<int> GetTwoWaySpaces(int passableSpaceAIndex, int passableSpaceBIndex)
    {
        List<int> spaces = new();

        foreach (Space space in WholeSpaces)
        {
            if (!space.Usable)
                continue;
            if (WholeSpaces[passableSpaceAIndex].ImpassableSpaces.Contains(space.SpaceIndex))
                continue;
            if (WholeSpaces[passableSpaceBIndex].ImpassableSpaces.Contains(space.SpaceIndex))
                continue;
            spaces.Add(space.SpaceIndex);
        }

        return spaces;
    }

    /// <summary>
    /// 특정 위치로부터 가장 가까운 공간 인덱스를 반환
    /// </summary>
    /// <param name="targetTransform">거리 측정 위치</param>
    /// <param name="searchSpaces">탐색 공간 리스트</param>
    /// <param name="exceptList">제외 공간 인덱스 리스트</param>
    /// <returns>특정 위치로부터 가장 가까운 공간 인덱스</returns>
    public int GetNearestSpace(int targetSpaceIndex, List<int> searchSpaces, List<int> exceptList)
    {
        if (searchSpaces.Count == 0)
            return -1;
        int nearestIndex = searchSpaces[0];
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < searchSpaces.Count; i++)
        {
            if (exceptList.Contains(searchSpaces[i]))
                continue;
            float distanceDiff = Vector3.Distance(WholeSpaces[targetSpaceIndex].Position, WholeSpaces[searchSpaces[i]].Position);
            if (distanceDiff < nearestDistance)
            {
                nearestIndex = searchSpaces[i];
                nearestDistance = distanceDiff;
            }
        }

        return nearestIndex;
    }
}
