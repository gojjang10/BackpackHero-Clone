using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("기본 스탯")]
    public int maxHp = 50;
    public int currentHp;

    [Header("전투 스탯")]
    public int maxEnergy = 3;
    public int currentEnergy;
    public int currentBlock; // 방어도

    [Header("UI 연결")]
    public PlayerUI playerUI;

    private void Start()
    {
        // 게임 시작 시 초기화
        currentHp = maxHp;
        currentEnergy = maxEnergy;
        currentBlock = 0;

        // UI 초기화
        UpdateUI();
    }

    // 행동력 변경 함수 (사용 시 음수, 회복 시 양수 넣기)
    public void ModifyEnergy(int amount)
    {
        currentEnergy += amount;

        // 최대치 넘지 않게, 0보다 작아지지 않게
        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        if (currentEnergy < 0) currentEnergy = 0;

        UpdateUI();
    }

    public void AddBlock(int amount)
    {
        currentBlock += amount;
        Debug.Log($" 방어도 증가! (+{amount}) -> 현재: {currentBlock}");
        UpdateUI();
    }

    // 턴 시작 시 리셋할 것들
    public void OnTurnStart()
    {
        currentEnergy = maxEnergy; // 행동력 풀충전
        currentBlock = 0;          // 방어도 초기화

        UpdateUI();
    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {

        int finalDamage = damage;

        // 1. 방어도가 있다면 먼저 차감
        if (currentBlock > 0)
        {
            if (currentBlock >= finalDamage)
            {
                // 방어도가 더 많으면 -> 데미지 0, 방어도만 깎임
                currentBlock -= finalDamage;
                finalDamage = 0;
            }
            else
            {
                // 방어도가 모자르면 -> 방어도 다 깎이고 남은 데미지 적용
                finalDamage -= currentBlock;
                currentBlock = 0;
            }
        }

        // 2. 남은 데미지로 체력 감소
        if (finalDamage > 0)
        {
            currentHp -= finalDamage;
            if (currentHp < 0) currentHp = 0;
            Debug.Log($"플레이어 피격! -{finalDamage} (남은 HP: {currentHp})");
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateHP(currentHp, maxHp);
            playerUI.UpdateStats(currentBlock, currentEnergy);
        }
    }

    [ContextMenu("Test Add Block 10")]
    public void TestAddBlock()
    {
        AddBlock(10);
    }
}
