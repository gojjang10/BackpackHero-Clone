using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHp = 50;
    public int currentHp;

    [Header("전투 스탯")]
    public int maxEnergy = 3;
    public int currentEnergy;
    public int currentBlock; // 방어도

    private void Start()
    {
        // 게임 시작 시 초기화
        currentHp = maxHp;
        currentEnergy = maxEnergy;
        currentBlock = 0;
    }

    // 행동력 변경 함수 (사용 시 음수, 회복 시 양수 넣기)
    public void ModifyEnergy(int amount)
    {
        currentEnergy += amount;

        // 최대치 넘지 않게, 0보다 작아지지 않게
        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        if (currentEnergy < 0) currentEnergy = 0;
    }

    // 턴 시작 시 리셋할 것들
    public void OnTurnStart()
    {
        currentEnergy = maxEnergy; // 행동력 풀충전
        currentBlock = 0;          // 방어도 초기화
    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        // 방어도가 있다면 방어도 먼저 차감 (나중에 구현)
        int finalDamage = damage;

        currentHp -= finalDamage;
        if (currentHp < 0) currentHp = 0;

        Debug.Log($" 플레이어 피격! {damage} 데미지 (남은 HP: {currentHp})");
    }
}
