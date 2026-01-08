using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemEffect : ScriptableObject
{
    public EffectTriggerType triggerType;

    [Header("탐색 전략 (드래그 앤 드롭)")]
    public SearchStrategy searchStrategy;

    [Header("필터링 설정 (선택)")]
    [Tooltip("비워두면 모든 아이템 허용. 설정하면 해당 태그 중 하나라도 가진 아이템만 허용.")]
    public List<ItemTag> targetTags;

    // 실행 함수 (자식들이 구현)
    public abstract void Execute(InventoryItem sourceItem, InventoryGrid grid);

    /// <summary>
    /// 설정된 전략(SearchStrategy)을 사용하여, 지정된 기준 좌표(x, y)를 중심으로 영향을 받는 타겟 아이템들을 반환합니다.
    /// <para>
    /// * 아이템이 이미 배치된 경우: <c>sourceItem.onGridX</c>, <c>sourceItem.onGridY</c>를 전달.<br/>
    /// * 배치 전 미리보기(Preview)인 경우: 마우스가 위치한 <c>임시 좌표</c>를 전달.
    /// </para>
    /// </summary>
    /// <param name="sourceItem">효과를 발동시키는 주체 아이템 (자기 자신은 탐색에서 제외됩니다)</param>
    /// <param name="grid">탐색을 수행할 인벤토리 그리드</param>
    /// <param name="x">탐색의 기준이 될 X 좌표 (Center/Origin)</param>
    /// <param name="y">탐색의 기준이 될 Y 좌표 (Center/Origin)</param>
    /// <returns>탐색 범위 내에 존재하며 필터링 조건을 통과한 아이템 리스트</returns>
    public List<InventoryItem> GetTargetItems(InventoryItem sourceItem, InventoryGrid grid, int x, int y)
    {
        // 1. 전략이 없으면 빈 리스트 반환 (안전장치)
        if (searchStrategy == null) return new List<InventoryItem>();

        // 2. 탐색 전략 클래스에게 좌표 계산 위임
        List<Vector2Int> coords = searchStrategy.GetCoords(sourceItem, grid, x, y);

        // 3. 좌표 -> 아이템 변환 및 필터링 (기존 로직 유지)
        List<InventoryItem> targets = new List<InventoryItem>();
        foreach (var coord in coords)
        {
            InventoryItem foundItem = grid.GetItem(coord.x, coord.y);

            // 4. 기본 유효성 검사 (빈칸 X, 나 자신 X, 중복 X)
            if (foundItem != null && foundItem != sourceItem && !targets.Contains(foundItem))
            {
                // 5. 태그 필터링 검사
                if (IsTagMatched(foundItem.data.itemTags))
                {
                    targets.Add(foundItem);
                }
            }
        }

        return targets;
    }

    // 태그 매칭 로직 (교집합 확인)
    private bool IsTagMatched(List<ItemTag> itemTags)
    {
        // 조건(targetTags)이 아예 없으면 "모두" (True)
        if (targetTags == null || targetTags.Count == 0) return true;

        // 아이템한테 태그가 하나도 없으면 "X" (False)
        if (itemTags == null || itemTags.Count == 0) return false;

        // 하나라도 겹치는 게 있는지 확인
        foreach (var tag in targetTags)
        {
            if (itemTags.Contains(tag)) return true;
        }
        return false;
    }
}
