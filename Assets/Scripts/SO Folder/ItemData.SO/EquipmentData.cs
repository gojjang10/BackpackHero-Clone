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
}
