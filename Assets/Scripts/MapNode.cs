using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{ 
    public Vector2Int coordinates; // 그리드 좌표 (x, y)
    public NodeType nodeType;      // 방 종류
    public int difficultyLevel;    // 난이도 

    // 연결 정보 (그래프 구조)
    public List<Vector2Int> outgoingConnections = new List<Vector2Int>();
    public List<Vector2Int> incomingConnections = new List<Vector2Int>();

    public MapNode(Vector2Int coords, NodeType type, int difficulty)
    {
        this.coordinates = coords;
        this.nodeType = type;
        this.difficultyLevel = difficulty;
    }
}
