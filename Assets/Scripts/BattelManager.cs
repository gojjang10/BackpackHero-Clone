using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // 편의상 우선 싱글톤 (추후 사용여부 재판단)

    [Header("테스트용 연결")]
    public Monster dummyMonster; // 인스펙터에서 하이어라키에 있는 몬스터 오브젝트 연결

    private void Awake()
    {
        instance = this;
    }

    // GridInteract에서 호출할 함수
    public void OnUseItem(InventoryItem item)
    {
        // 1. 아이템 타입 체크 (패시브는 사용 불가)
        if (item.data.itemType == ItemType.Passive)
        {
            Debug.Log(" 패시브 아이템은 클릭해서 사용할 수 없습니다.");
            return;
        }

        // 2. (나중에 구현) 행동력(Energy) 체크 로직 들어갈 자리
        // if (currentEnergy < item.data.energyCost) return;

        // 3. 효과 실행 (지금은 더미 몬스터 때리기만 구현)
        if (item.currentAttack > 0)
        {
            if (dummyMonster != null)
            {
                dummyMonster.TakeDamage(item.currentAttack);
            }
        }
        else
        {
            Debug.Log($" {item.data.itemName}은(는) 공격력이 없습니다.");
        }

        // 4. (나중에 구현) BaseItemEffect의 Active 효과들 실행
        // foreach (var effect in item.data.effects) { if(Active) effect.Execute(...); }
    }
}
