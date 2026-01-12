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
        List<InventoryItem> targets = GetTargetItems(user, grid, user.onGridX, user.onGridY);

        // 연결된 타겟 수만큼 내 공격력 증가
        int totalBonus = targets.Count * bonusAttack;

        user.currentAttack += totalBonus; // target이 아니라 user(나)를 수정

        Debug.Log($"[{user.data.itemName}] 주변 {targets.Count}개와 연결되어 공격력이 {totalBonus}만큼 상승했습니다!");
    }
}
