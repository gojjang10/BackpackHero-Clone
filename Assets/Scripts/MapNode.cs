using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    // 1. 기본 정보
    public Vector2Int coordinate; // 좌표 (x, y)
    public NodeType nodeType;     // 방 타입

    // 2. 연결 정보 
    public List<Vector2Int> nextNodes = new List<Vector2Int>(); // 다음으로 갈 수 있는 방들

    // 3. 상태 정보 (게임 진행용)
    public bool isVisited = false; // 방문 여부
    public bool isAccessible = false; // 접근 가능 여부 

    // 생성자 (데이터 넣기 쉽게)
    public MapNode(Vector2Int coord, NodeType type)
    {
        this.coordinate = coord;
        this.nodeType = type;
    }
}
