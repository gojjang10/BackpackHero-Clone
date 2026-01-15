using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // 편의상 우선 싱글톤 (추후 사용여부 재판단)

    [Header("연결된 시스템")]
    public MonsterFactory monsterFactory;

    [Header("테스트 데이터")]
    public BaseMonsterData testMonsterData; // 소환할 몬스터 데이터 (슬라임 등)

    [Header("현재 타겟")]
    public Monster currentTarget; // 지금 플레이어가 보고 있는 놈


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 팩토리와 데이터가 연결되어 있다면 소환 시도
        if (monsterFactory != null && testMonsterData != null)
        {
            // 1. 공장에게 주문 넣기
            Monster newMonster = monsterFactory.SpawnMonster(testMonsterData);

            // 2. 소환된 몬스터를 자동으로 타겟팅 (테스트용)
            if (newMonster != null)
            {
                SelectTarget(newMonster);
            }
        }
        else
        {
            Debug.LogWarning("BattleManager: 몬스터 팩토리나 테스트 데이터가 연결되지 않았습니다.");
        }
    }

    // 몬스터가 호출하는 함수: "나를 타겟으로 해줘"
    public void SelectTarget(Monster monster)
    {
        // 1. 기존에 선택된 애가 있었다면, 표시 끄기
        if (currentTarget != null)
        {
            currentTarget.SetSelection(false);
        }

        // 2. 새로운 타겟 등록하고 표시 켜기
        currentTarget = monster;
        currentTarget.SetSelection(true);

        Debug.Log($" 타겟 변경됨: {monster.data.monsterName}");
    }

    // 아이템 사용 로직 (수정됨)
    public void OnUseItem(InventoryItem item)
    {
        // 아이템 타입 체크 (패시브 등 제외)
        if (item.data.itemType != ItemType.Active) return;

        // 공격 아이템인데 타겟이 없다? -> 경고
        // (나중에 회복 아이템 등은 타겟이 없어도 되게 분기 처리 필요)
        if (currentTarget == null)
        {
            Debug.LogWarning(" 공격할 대상을 먼저 선택해주세요!");
            return;
        }

        // 타겟에게 데미지 적용
        if (item.currentAttack > 0)
        {
            currentTarget.TakeDamage(item.currentAttack);
        }
    }
}
