using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Item Data/Equipment")]
public class EquipmentData : BaseItemData
{
    [Header("장비 스탯 (원본)")]
    public int baseAttack;
    public int baseBlock;
    // 필요한 스탯 더 추가 가능

    public override void ApplyBaseStats(InventoryItem item)
    {
        // 부모 거 실행(혹시 모르니)
        base.ApplyBaseStats(item);

        // 내 데이터(원본)에 있는 값을 아이템(InventoryItem)에 덮어씌움
        item.currentAttack = baseAttack;
        item.currentBlock = baseBlock;
    }

    public override void OnUse(InventoryItem item, Player player, Monster target)
    {
        // A. 공격력이 있다면? -> 때린다
        if (item.currentAttack > 0)
        {
            if (target != null)
            {
                target.TakeDamage(item.currentAttack);
                Debug.Log($" 공격! {target.name}에게 {item.currentAttack} 데미지");
            }
        }

        // B. 방어도가 있다면? -> 막는다
        if (item.currentBlock > 0) // InventoryItem에 currentBlock 변수 추가 필요
        {
            if (player != null)
            {
                player.AddBlock(item.currentBlock);
                Debug.Log($" 방어! 방어도 +{item.currentBlock}");
            }
        }
    }
}
