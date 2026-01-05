using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemEffect : ScriptableObject
{
    public EffectTriggerType triggerType;

    [Header("탐색할 범위 (상대 좌표)")]
    // 예: (1,0)은 오른쪽 한 칸
    public List<Vector2Int> searchPattern;

    // 실행 함수 (자식들이 구현)
    public abstract void Execute(InventoryItem sourceItem, InventoryGrid grid);

    // 타겟 찾는 공통 함수 (자식들이 편하게 쓰라고 만듦)
    protected List<InventoryItem> GetTargetItems(InventoryItem sourceItem, InventoryGrid grid)
    {
        // 1. GridSearcher 써서 좌표 구하기
        List<Vector2Int> coords = GridSearcher.GetValidCoordinates(sourceItem.onGridX, sourceItem.onGridY, searchPattern, grid);

        // 2. 좌표에 있는 아이템 가져오기
        List<InventoryItem> targets = new List<InventoryItem>();
        foreach (var coord in coords)
        {
            InventoryItem item = grid.GetItem(coord.x, coord.y);
            if (item != null) targets.Add(item);
        }

        // 3. 타겟 리스트 반환
        return targets;
    }
}
