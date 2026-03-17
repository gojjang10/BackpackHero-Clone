using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeVisual : MonoBehaviour
{
    // 내가 무슨 방인지 알고 있어야 함
    private MapNode nodeData;
    private SpriteRenderer sr; // 추가: 색상을 칠할 렌더러

    private void Awake()
    {
        // 시작할 때 내 오브젝트에 붙어있는 렌더러를 가져옵니다.
        sr = GetComponent<SpriteRenderer>();
    }

    // 초기화 함수 (생성될 때 데이터 받기)
    public void Setup(MapNode data)
    {
        this.nodeData = data;
        UpdateVisualState(); // 초기 상태에 맞게 색상 업데이트
    }

    // 노드의 상태에 따라 색상을 업데이트하는 전담 함수
    public void UpdateVisualState()
    {
        if (nodeData == null || sr == null) return;
        
        // 방이 이미 깬 방이면 파란색으로 표시 (어떤 타입이든)
        if (nodeData.isCleared)
        {
            if(nodeData.nodeType == NodeType.NextStair)
            {
                sr.color = Color.green;
                return;
            }
            sr.color = Color.blue;
            return;
        }

        // 아직 안 깬 방이면 고유의 타입 색상 부여
        switch (nodeData.nodeType)
        {
            case NodeType.Battle: sr.color = Color.red; break;
            case NodeType.Shop: sr.color = Color.yellow; break;
            case NodeType.Neutral: sr.color = Color.blue; break;
            case NodeType.Boss: sr.color = Color.black; break;
            case NodeType.NextStair: sr.color = Color.green; break;
        }
    }

    // 마우스 클릭 이벤트 (Collider 필수!)
    private void OnMouseDown()
    {
        // 1. 데이터가 없으면 무시
        if (nodeData == null) return;

        // 2. (추후 구현 예정) 접근 가능한 노드인지 체크
        // if (!nodeData.isAccessible) return;

        // 3. StageManager에게 "나 이 방 들어갈래" 요청
        StageManager.Instance.TryMoveToNode(nodeData);
    }
}
