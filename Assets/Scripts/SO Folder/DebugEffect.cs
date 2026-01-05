using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effect/Debug Effect")]
public class DebugEffect : BaseItemEffect
{
    public override void Execute(InventoryItem user, InventoryGrid grid)
    {
        // 내 패턴 안에 있는 아이템들을 찾아서 로그 찍기
        List<InventoryItem> targets = GetTargetItems(user, grid);

        if (targets.Count == 0)
        {
            Debug.Log("범위 내에 아무것도 없습니다.");
        }
        else
        {
            foreach (var target in targets)
            {
                Debug.Log($"탐색 성공! 발견된 아이템: {target.data.itemName}");
            }
        }
    }
}
