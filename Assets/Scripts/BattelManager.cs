using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // 편의상 우선 싱글톤 (추후 사용여부 재판단)

    [Header("하위 시스템 연결 (외주 업체들)")]
    public MonsterSpawner spawner;      // 배치 담당
    public BattleUIManager uiManager;   // 화면 담당

    [Header("시스템 연결")]
    public List<BaseMonsterData> testMonsterList;
    public Player player; // 플레이어 참조

    [Header("전투 상태")]
    public BattleState state; // 현재 전투 상태
    public List<Monster> activeMonsters = new List<Monster>();  // 현재 전투에 참여 중인 몬스터들
    public Monster currentTarget; // 지금 플레이어가 보고 있는 몬스터


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 몬스터 리스트를 넘겨서 여러 마리 소환 요청
        if (spawner != null && testMonsterList != null && testMonsterList.Count > 0)
        {
            // 스포너가 몬스터들을 쫙 깔아주고, 생성된 애들 명단을 리턴해줌
            activeMonsters = spawner.SpawnWave(testMonsterList);

            // 첫 번째 몬스터를 자동으로 타겟팅 (편의성)
            if (activeMonsters.Count > 0)
            {
                SelectTarget(activeMonsters[0]);
            }
        }

        // 전투 시작
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        yield return new WaitForSeconds(0.5f);
        StartPlayerTurn();
    }

    // --- [플레이어 턴] ---
    public void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;

        // 플레이어 상태 리셋 (행동력 충전 등)
        if (player != null) player.OnTurnStart();

        // UI 매니저에게 텍스트 갱신 명령
        if (uiManager != null)
        {
            uiManager.UpdateEnergy(player.currentEnergy, player.maxEnergy);
            uiManager.UpdateTurnText("Player Turn");
        }
        Debug.Log(" 플레이어 턴 시작!");
    }

    // '턴 종료' 버튼과 연결
    public void OnEndTurnButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(EnemyTurn());
    }

    // 아이템 사용
    public void OnUseItem(InventoryItem item)
    {
        if (state != BattleState.PlayerTurn) return;
        if (item.data.itemType != ItemType.Active) return;

        if (currentTarget == null)
        {
            Debug.LogWarning(" 타겟이 없습니다!");
            return;
        }

        // Player의 행동력 체크
        if (player.currentEnergy < item.data.energyCost)
        {
            Debug.LogWarning($"행동력 부족! (필요: {item.data.energyCost}, 보유: {player.currentEnergy})");
            return;
        }

        // 공격 실행
        if (item.currentAttack > 0)
        {
            currentTarget.TakeDamage(item.currentAttack);
            // 몬스터가 죽었는지 체크
            if (CheckAllMonstersDead())
            {
                StartCoroutine(WinBattle());
                return;
            }
        }

        // 행동력 차감
        player.ModifyEnergy(-item.data.energyCost);
        if (uiManager != null)
            uiManager.UpdateEnergy(player.currentEnergy, player.maxEnergy);
    }

    // --- [적 턴] ---
    IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        if (uiManager != null) uiManager.UpdateTurnText("Enemy Turn");

        Debug.Log(" 적 턴 시작!");

        // 몬스터 리스트를 하나씩 돌면서 행동시킴
        foreach (Monster monster in activeMonsters)
        {
            // 죽은 몬스터는 공격 못 함
            if (monster.currentHp <= 0) continue;

            yield return new WaitForSeconds(0.5f); // 몬스터 간 텀을 줌

            // 공격 연출 (나중에 애니메이션 대기 시간으로 대체될 곳)
            monster.transform.localScale = Vector3.one * 1.2f; // 커지는 연출 (임시)
            yield return new WaitForSeconds(0.2f);
            monster.transform.localScale = Vector3.one;        // 원상복구

            // 플레이어 피격
            int damage = monster.data.attackDamage;
            if (player != null)
            {
                player.TakeDamage(damage);
                if (player.currentHp <= 0)
                {
                    StartCoroutine(LoseBattle());
                    yield break; // 플레이어 죽으면 즉시 종료
                }
            }
        }

        yield return new WaitForSeconds(1f); // 모든 공격 끝나고 잠시 대기
        StartPlayerTurn();
    }

    // 모든 몬스터가 죽었는지 검사
    bool CheckAllMonstersDead()
    {
        foreach (Monster monster in activeMonsters)
        {
            if (monster.currentHp > 0)
            {
                return false; // 살아있는 몬스터가 하나라도 있으면 false 반환  
            }
        }
        return true; // 모든 몬스터가 죽었음
    }

    // 몬스터가 호출하는 함수
    public void SelectTarget(Monster monster)
    {
        // 1. 기존에 선택된 몬스터가 있었다면, 표시 끄기
        if (currentTarget != null)
        {
            currentTarget.SetSelection(false);
        }

        // 2. 새로운 타겟 등록하고 표시 켜기
        currentTarget = monster;
        currentTarget.SetSelection(true);

        Debug.Log($" 타겟 변경됨: {monster.data.monsterName}");
    }

    IEnumerator WinBattle()
    {
        state = BattleState.Win; // 상태 변경 
        Debug.Log("승리했습니다!");

        yield return new WaitForSeconds(1f);    // 잠시 대기

        if (uiManager != null) uiManager.ShowWinUI();
    }

    IEnumerator LoseBattle()
    {
        state = BattleState.Lose;
        Debug.Log("패배했습니다...");

        yield return new WaitForSeconds(1f);    // 잠시 대기

        if (uiManager != null) uiManager.ShowLoseUI();
    }
}
