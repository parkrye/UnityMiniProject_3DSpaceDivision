using System;
using System.Collections.Generic;

public static class SpaceBaseAstar
{
    public static Stack<int> PathFinding(SpaceManager spaceManager, int startSpaceIndex, int endSpaceIndex)
    {
        Stack<int> answer = new();

        Dictionary<int, bool> visited = new();
        Dictionary<int, Node> nodes = new();
        PriorityQueue<Node, float> pq = new();     

        int counter = 0;
        int maxCount = 10000;

        Node startNode = new(startSpaceIndex, -1, 0, 0);
        pq.Enqueue(startNode, 0);
        nodes.Add(startSpaceIndex, startNode);

        while (pq.Count > 0 && ++counter < maxCount)
        {
            Node nowNode = pq.Dequeue();
            if (visited.ContainsKey(nowNode.nowSpaceIndex))
                continue;
            visited.Add(nowNode.nowSpaceIndex, true);

            if (spaceManager.WholeSpaces[nowNode.nowSpaceIndex].PassableSpaces.Contains(endSpaceIndex))
            {
                while (nowNode.parentSpaceIndex != -1)
                {
                    answer.Push(nowNode.nowSpaceIndex);
                    nowNode = nodes[nowNode.parentSpaceIndex];
                }
                return answer;
            }

            for(int i = 0; i < spaceManager.WholeSpaces[nowNode.nowSpaceIndex].PassableSpaces.Count; i++)
            {
                int searchSpaceIndex = spaceManager.WholeSpaces[nowNode.nowSpaceIndex].PassableSpaces[i];

                if (visited.ContainsKey(searchSpaceIndex))
                    continue;

                // i-n-1  i-1   i+n-1
                //  i-n    i     i+n
                // i-n+1  i+1   i+n+1

                // 가로줄 위치는 k / n
                // 세로줄 위치는 k % n
                int rowDiff = Math.Abs((nowNode.nowSpaceIndex / spaceManager.SpaceLine) - (searchSpaceIndex / spaceManager.SpaceLine));
                int colDiff = Math.Abs((nowNode.nowSpaceIndex % spaceManager.SpaceLine) - (searchSpaceIndex % spaceManager.SpaceLine));
                int g = nowNode.g + rowDiff + colDiff;

                int rowDiffEnd = Math.Abs((searchSpaceIndex / spaceManager.SpaceLine) - (endSpaceIndex / spaceManager.SpaceLine));
                int colDiffEnd = Math.Abs((searchSpaceIndex % spaceManager.SpaceLine) - (endSpaceIndex % spaceManager.SpaceLine));
                int h = rowDiffEnd + colDiffEnd;

                Node findNode = new(searchSpaceIndex, nowNode.nowSpaceIndex, g, h);
                if (!nodes.ContainsKey(searchSpaceIndex))
                {
                    nodes.Add(searchSpaceIndex, findNode);
                    pq.Enqueue(findNode, findNode.f);
                }
                else if (nodes[searchSpaceIndex].f > findNode.f)
                {
                    nodes[searchSpaceIndex] = findNode;
                    pq.Enqueue(findNode, findNode.f);
                }
            }
        }

        return answer;
    }

    struct Node
    {
        public int nowSpaceIndex;
        public int parentSpaceIndex;

        public int g;
        public int h;
        public int f;

        public Node(int _nowSpaceIndex, int _parentSpaceIndex, int _g, int _h)
        {
            nowSpaceIndex = _nowSpaceIndex;
            parentSpaceIndex = _parentSpaceIndex;
            g = _g;
            h = _h;
            f = g + h;
        }
    }
}