using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // 편의상 우선 싱글톤 (추후 사용여부 재판단)

    [Header("시스템 연결")]
    public MonsterFactory monsterFactory;
    public BaseMonsterData testMonsterData; // 소환할 몬스터 데이터 (슬라임 등)
    public Player player; // 플레이어 참조

    [Header("UI 연결")]
    public TextMeshProUGUI energyText;   // "Energy: 3/3"
    public TextMeshProUGUI turnInfoText; // "나의 턴" / "적의 턴" 표시용

    [Header("결과 UI")]
    public GameObject endGamePanel;      // 아까 만든 검은 패널
    public TextMeshProUGUI resultText;   // 결과 텍스트

    [Header("전투 상태")]
    public BattleState state; // 현재 전투 상태
    public Monster currentTarget; // 지금 플레이어가 보고 있는 놈


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 몬스터 소환
        if (monsterFactory != null && testMonsterData != null)
        {
            Monster newMonster = monsterFactory.SpawnMonster(testMonsterData);
            if (newMonster != null) SelectTarget(newMonster);
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

        UpdateUI();
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
            // [추가된 부분] 몬스터가 죽었나 확인?
            if (currentTarget.currentHp <= 0)
            {
                StartCoroutine(WinBattle());
                return; // 승리했으면 아래 로직(행동력 차감 등) 실행 안 하고 종료
            }
        }

        // 행동력 차감
        player.ModifyEnergy(-item.data.energyCost);
        UpdateUI(); // UI 갱신
    }

    // --- [적 턴] ---
    IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        if (turnInfoText != null) turnInfoText.text = "Enemy Turn";

        Debug.Log(" 적 턴 시작!");

        yield return new WaitForSeconds(1.5f); // 적이 공격하는 척 뜸 들이기 (추후에 다른 방식 고민)

        // 적이 살아있으면 플레이어 공격
        if (currentTarget != null && currentTarget.currentHp > 0)
        {
            int damage = currentTarget.data.attackDamage;
            if (player != null) 
            {
                player.TakeDamage(damage);
                if (player.currentHp <= 0)
                {
                    StartCoroutine(LoseBattle());
                    yield break; // 코루틴 즉시 종료
                }
            } 
        }

        yield return new WaitForSeconds(1f); // 잠시 대기

        // 다시 플레이어 턴으로
        StartPlayerTurn();
    }

    // UI 업데이트
    void UpdateUI()
    {
        if (player != null && energyText != null)
        {
            energyText.text = $"Energy: {player.currentEnergy} / {player.maxEnergy}";
        }

        if (turnInfoText != null && state == BattleState.PlayerTurn)
        {
            turnInfoText.text = "Player Turn";
        }
    }

    // 몬스터가 호출하는 함수
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

    IEnumerator WinBattle()
    {
        state = BattleState.Win; // 상태 변경 
        Debug.Log("승리했습니다!");

        yield return new WaitForSeconds(1f); // 잠시 여운을 즐기고

        // UI 띄우기
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            if (resultText != null) resultText.text = "VICTORY!";
        }
    }

    IEnumerator LoseBattle()
    {
        state = BattleState.Lose;
        Debug.Log("패배했습니다...");

        yield return new WaitForSeconds(1f);

        // UI 띄우기
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            if (resultText != null) resultText.text = "GAME OVER...";
        }
    }
}
