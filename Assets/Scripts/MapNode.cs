using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class MapNode
{
    // 1. 기본 정보
    public Vector2Int coordinate; // 좌표 (x, y)

    private NodeType _nodeType;
    public NodeType nodeType        // 방 타입
    {
        get { return _nodeType; }
        set
        {
            _nodeType = value;
            // ★ 타입이 바뀔 때마다 자동으로 클리어 여부 결정
            // 배틀이나 보스가 아니면 -> 자동으로 클리어(true) 처리
            if (_nodeType == NodeType.Battle || _nodeType == NodeType.Boss)
            {
                isCleared = false;
            }
            else
            {
                isCleared = true;
            }
        }
    }

    // 2. 연결 정보 
    public List<Vector2Int> nextNodes = new List<Vector2Int>(); // 다음으로 갈 수 있는 방들

    // 3. 상태 정보 (게임 진행용)
    public bool isVisited = false; // 방문 여부
    public bool isAccessible = false; // 접근 가능 여부 
    public bool isCleared = false;  // 클리어 여부

    // 생성자 (데이터 넣기 쉽게)
    public MapNode(Vector2Int coord, NodeType type)
    {
        this.coordinate = coord;
        this.nodeType = type;
    }
}
