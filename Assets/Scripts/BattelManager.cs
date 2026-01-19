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
            // 스포너가 몬스터들을 생성해주고, 생성된 몬스터 명단을 리턴해줌
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

    // 아이템 사용 함수
    public void OnUseItem(InventoryItem item)
    {
        // 1. 사용 가능 여부 검증 
        if (!IsItemUsable(item)) return;

        // 2. 행동력 차감
        player.ModifyEnergy(-item.data.energyCost);

        // 3. 공격 로직 실행 
        if (item.currentAttack > 0)
        {
            ProcessAttack(item.currentAttack);
        }

        // 4. UI 갱신
        if (uiManager != null)
            uiManager.UpdateEnergy(player.currentEnergy, player.maxEnergy);
    }

    // 아이템 사용 가능 여부 체크
    private bool IsItemUsable(InventoryItem item)
    {
        if (state != BattleState.PlayerTurn) return false;
        if (item.data.itemType != ItemType.Active) return false;

        if (currentTarget == null)
        {
            Debug.LogWarning(" 타겟이 없습니다!");
            return false;
        }

        if (player.currentEnergy < item.data.energyCost)
        {
            Debug.LogWarning($" 행동력 부족! (필요: {item.data.energyCost})");
            return false;
        }

        return true;
    }

    // 공격 처리 및 결과 확인 (자동 타겟팅 포함)
    private void ProcessAttack(int damage)
    {
        if (currentTarget == null) return;

        currentTarget.TakeDamage(damage);

        // 몬스터가 죽었는지 확인
        if (currentTarget.currentHp <= 0)
        {
            HandleMonsterDeath(currentTarget);
        }
    }

    // 몬스터 사망 처리 로직
    private void HandleMonsterDeath(Monster deadMonster)
    {
        // 1. 리스트에서 제거 
        activeMonsters.Remove(deadMonster);

        // 2. 몬스터 오브젝트 퇴장 처리 (OnDie 호출)
        deadMonster.OnDie();

        // 3. 승리 체크
        if (activeMonsters.Count == 0)
        {
            currentTarget = null;
            StartCoroutine(WinBattle());
        }
        else
        {
            // 4. 아직 적이 남았으면 다음 타겟 자동 지정
            SelectNextTarget();
        }
    }

    // --- [적 턴] ---
    IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        if (uiManager != null) uiManager.UpdateTurnText("Enemy Turn");
        Debug.Log(" 적 턴 시작!");

        foreach (Monster monster in activeMonsters)
        {
            //  몬스터가 없거나 죽었으면 패스
            if (monster == null || monster.currentHp <= 0) continue;

            yield return new WaitForSeconds(0.5f);

            // 매니저가 직접 제어하지 않고 몬스터에게 위임
            yield return StartCoroutine(monster.AttackRoutine(player));
        }

        yield return new WaitForSeconds(1f);
        StartPlayerTurn();
    }

    // 다음 타겟 자동 선택
    void SelectNextTarget()
    {
        if (activeMonsters.Count > 0)
        {
            // 리스트의 첫 번째 몬스터를 자동으로 선택
            SelectTarget(activeMonsters[0]);
        }
        else
        {
            currentTarget = null;
        }
    }

    // 모든 몬스터가 죽었는지 검사
    bool CheckAllMonstersDead()
    {
        return activeMonsters.Count == 0;
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
