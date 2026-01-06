using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemEffect : ScriptableObject
{
    public EffectTriggerType triggerType;

    [Header("탐색 설정")]
    public SearchDirection searchDirection; // 탐색 방향

    [Header("CustomPattern일 때만 사용")]    // * 불규칙한 고정 범위(예: 대각선, L자 모양 등)를 탐색할 때 사용
    public List<Vector2Int> customPattern;

    // 실행 함수 (자식들이 구현)
    public abstract void Execute(InventoryItem sourceItem, InventoryGrid grid);

    // 타겟 찾는 공통 함수 (자식들이 편하게 쓰라고 만듦)
    protected List<InventoryItem> GetTargetItems(InventoryItem sourceItem, InventoryGrid grid)
    {
       List<Vector2Int> coords;

        // 1. 방향 설정에 따라 좌표 구하기 방식 분기
        if (searchDirection == SearchDirection.CustomPattern)
        {
            // [기존 방식] 수동 패턴. 현재 아이템의 좌표를 기준으로 customPattern이 유효한지 판단. 
            coords = GridSearcher.GetValidCoordinates(sourceItem.onGridX, sourceItem.onGridY, customPattern, grid);
        }
        else
        {
            // [신규 방식] 크기에 따른 동적 계산
            coords = GridSearcher.GetDirectionTargets(
                sourceItem.onGridX,
                sourceItem.onGridY,
                sourceItem.Width,
                sourceItem.Height,
                searchDirection,
                grid
            );
        }

        // 2. 좌표에 있는 아이템 가져오기 (중복 및 자기자신 제외)
        List<InventoryItem> targets = new List<InventoryItem>();
        
        foreach (var coord in coords)
        {
            InventoryItem foundItem = grid.GetItem(coord.x, coord.y);

            // 빈칸이 아니고, 나 자신(sourceItem)이 아닐 때만
            if (foundItem != null && foundItem != sourceItem)
            {
                // 이미 리스트에 들어있는 아이템인지 확인 (큰 아이템 중복 방지)
                if (!targets.Contains(foundItem))
                {
                    targets.Add(foundItem);
                }
            }
        }

        return targets;
    }
}
