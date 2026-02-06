using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeVisual : MonoBehaviour
{
    // 내가 무슨 방인지 알고 있어야 함
    private MapNode nodeData;

    // 초기화 함수 (생성될 때 데이터 받기)
    public void Setup(MapNode data)
    {
        this.nodeData = data;
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
