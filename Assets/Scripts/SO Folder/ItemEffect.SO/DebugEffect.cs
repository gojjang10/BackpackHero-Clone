using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effect/Debug Effect")]
public class DebugEffect : BaseItemEffect
{
    [Header("효과 설정")]
    public int bonusAttack = 5; // 올려줄 공격력 수치 (현재 사용 안 함, 예시용)

    public override void Execute(InventoryItem user, InventoryGrid grid)
    {
        // 1. 필터링된 타겟 가져오기 (실제 배치된 위치 기준)
        List<InventoryItem> targets = GetTargetItems(user, grid, user.onGridX, user.onGridY);

        if (targets.Count == 0)
        {
            Debug.Log($"[{user.data.itemName}] 효과 발동 실패: 범위 내에 조건에 맞는 대상이 없습니다.");
            return;
        }

        Debug.Log($" [{user.data.itemName}] 효과 발동! (대상 수: {targets.Count}개)");

        foreach (var target in targets)
        {
            // 2. 타겟의 실시간 공격력(InventoryItem 변수) 증가
            int oldAttack = target.currentAttack;
            target.currentAttack += bonusAttack;

            Debug.Log($"      타겟: {target.data.itemName} (태그: {string.Join(",", target.data.itemTags)})");
            Debug.Log($"      공격력 변화: {oldAttack} -> {target.currentAttack} (+{bonusAttack})");
        }
    }
}
