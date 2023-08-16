using System.Collections.Generic;
using UnityEngine;

public class SpaceDivinder : MonoBehaviour
{
    [SerializeField] List<Space> wholeSpace;
    public List<Space> WholeSpaces { get { return wholeSpace; } set { wholeSpace = value; } }

    [SerializeField][Range(1f, 5f)] float spaceSize;
    [SerializeField][Range(2, 20)] int spaceLine;
    [SerializeField] GameObject spacePrefab;
    [SerializeField] int state;

    void Awake()
    {
        WholeSpaces = new List<Space>();

        state = 0;
    }


    void Start()
    {
        SettingSpaces();
        SettingRoutes();
    }

    void SettingSpaces()
    {
        int index = 0;
        for(int i = (int)(-spaceLine * 0.5f); i <= (int)(spaceLine * 0.5f); i++)
        {
            for (int j = (int)(-spaceLine * 0.5f); j <= (int)(spaceLine * 0.5f); j++)
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

    void SettingRoutes()
    {
        for (int i = 0; i < WholeSpaces.Count; i++)
        {
            if (!WholeSpaces[i].Usable)
                continue;

            for (int j = i + 1; j < WholeSpaces.Count; j++)
            {
                if (!WholeSpaces[j].Usable)
                    continue;

                if (Physics.Raycast(WholeSpaces[i].Position, WholeSpaces[j].Position - WholeSpaces[i].Position, Vector3.Distance(WholeSpaces[j].Position,  WholeSpaces[i].Position)))
                {
                    WholeSpaces[i].ImpassableSpaces.Add(WholeSpaces[j]);
                    WholeSpaces[j].ImpassableSpaces.Add(WholeSpaces[i]);
                }
                else
                {
                    WholeSpaces[i].PassableSpaces.Add(WholeSpaces[j]);
                    WholeSpaces[j].PassableSpaces.Add(WholeSpaces[i]);
                }
            }
        }
    }
}
