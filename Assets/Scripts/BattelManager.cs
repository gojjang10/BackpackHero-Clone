using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // 편의상 우선 싱글톤 (추후 사용여부 재판단)

    [Header("시스템 연결")]
    public MonsterSpawner spawner;      // 배치 담당
    public BattleUIManager uiManager;   // 화면 담당

    [Header("시스템 연결")]
    public List<BaseMonsterData> testMonsterList;
    public Player player; // 플레이어 참조

    [Header("전투 상태")]
    public BattleState state; // 현재 전투 상태
    public List<Monster> activeMonsters = new List<Monster>();  // 현재 전투에 참여 중인 몬스터들
    public Monster currentTarget; // 지금 플레이어가 보고 있는 몬스터

    [Header("보상 시스템")]
    public GameObject rewardUIObject; // RewardPanel 오브젝트 (켜고 끄기용)
    public ItemSpawner itemSpawner;   // 아이템 스포너


    private void Awake()
    {
        instance = this;
    }

    // 외부(StageManager)에서 호출할 초기화 함수
    public void StartBattle()
    {
        Debug.Log("새로운 전투 시작! 몬스터를 배치합니다.");

        // 1. [청소] 기존에 살아남거나 죽은 몬스터 찌꺼기 제거
        // 리스트에 있는 애들뿐만 아니라, 화면에 남아있는 몬스터 오브젝트를 싹 지워야 함
        if (activeMonsters != null)
        {
            foreach (var monster in activeMonsters)
            {
                if (monster != null) Destroy(monster.gameObject);
            }
            activeMonsters.Clear();
        }

        // 2. [생성] 스포너에게 몬스터 생성 요청 (기존 로직)
        if (spawner != null && testMonsterList != null && testMonsterList.Count > 0)
        {
            activeMonsters = spawner.SpawnWave(testMonsterList);

            // 첫 번째 타겟 자동 지정
            if (activeMonsters.Count > 0)
            {
                SelectTarget(activeMonsters[0]);
            }
        }

        // 3. [상태 초기화]
        state = BattleState.Start;
        GameManager.instance.SetState(GameState.Battle);    // 배틀 시작 시 게임 상태 변경

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
        Debug.Log("--- [적 턴 시작] 행동 실행 ---");

        // 1. 모든 몬스터가 큐에 쌓아둔 행동을 실행 (Perform)
        foreach (Monster monster in activeMonsters)
        {
            if (monster == null || monster.currentHp <= 0) continue;

            // ★ [수정] AttackRoutine -> PerformTurnRoutine 변경
            yield return StartCoroutine(monster.PerformTurnRoutine(player));
        }

        yield return new WaitForSeconds(0.5f);

        Debug.Log("--- [적 턴 종료] 다음 행동 계획 ---");

        // 2. ★ [추가] 턴 넘기기 전에 다음 행동 미리 계획 (Telegraphing)
        foreach (Monster monster in activeMonsters)
        {
            if (monster != null && monster.currentHp > 0)
            {
                monster.PlanNextTurn();
                // (추후 여기에 UI 아이콘 갱신 코드 추가)
            }
        }

        // 플레이어 생존 체크 및 턴 넘기기
        if (player.currentHp == 0)
        {
            StartCoroutine(LoseBattle());
        }
        else
        {
            StartPlayerTurn();
        }
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

        uiManager.OnDisable();  // 전투 UI 비활성화
        GameManager.instance.SetState(GameState.Exploration); // 탐험 모드로 전환

        if (StageManager.Instance != null && StageManager.Instance.currentNode != null)
        {
            StageManager.Instance.currentNode.isCleared = true;
            Debug.Log($" 노드 클리어 완료! ({StageManager.Instance.currentNode.coordinate})");
        }

        // 1. 기존 승리 UI (VICTORY 텍스트) 대신 보상 창을 띄움
        if (rewardUIObject != null)
        {
            rewardUIObject.SetActive(true);

            // 2. 보상 아이템 생성 요청
            if (itemSpawner != null)
            {
                itemSpawner.SpawnRewardItems(5);
            }
        }
    }

    IEnumerator LoseBattle()
    {
        state = BattleState.Lose;
        Debug.Log("패배했습니다...");

        yield return new WaitForSeconds(1f);    // 잠시 대기

        uiManager.OnDisable();  // 전투 UI 비활성화

        if (uiManager != null) uiManager.ShowLoseUI();
    }
}
