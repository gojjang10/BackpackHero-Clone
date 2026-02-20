using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("기본 스탯")]
    public int maxHp = 50;
    public int currentHp;
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 10;           // 다음 레벨업에 필요한 경험치
    [SerializeField] int _expandPoints = 0;      // 가방을 확장할 수 있는 포인트 (레벨업 당 +3)

    public int expandPoints
    {
        get { return _expandPoints; }
        set
        {
            if (_expandPoints != value) // 값이 실제로 변했을 때만
            {
                _expandPoints = value;

                // 포인트가 바뀌었다고 구독자들에게 방송을 켭니다 (새로운 포인트 값을 전달)
                OnExpandPointsChanged?.Invoke(_expandPoints);
            }
        }
    }

    [Header("전투 스탯")]
    public int maxEnergy = 3;
    public int currentEnergy;
    public int currentBlock; // 방어도

    [Header("UI 연결")]
    public PlayerUI playerUI;

    // 포인트가 변경될 때마다 알림을 보낼 이벤트 (방송국 역할)
    public event Action<int> OnExpandPointsChanged;
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

    // 방어도 추가 함수
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
        UpdateUI();
    }

    public void OnTurnEnd()
    {
        // 턴이 끝날 때 방어도가 사라지는 경우
        currentBlock = 0;
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

    // 경험치 획득 함수
    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($" 경험치 획득: +{amount} (현재: {currentExp} / {maxExp})");

        // 경험치가 다 찼는지 확인 (연속 레벨업 가능성 고려하여 while문 사용)
        while (currentExp >= maxExp)
        {
            LevelUp();
        }

        UpdateUI(); // EXP 바 갱신을 위해 UI 업데이트 호출
    }

    // 레벨업 처리 함수
    private void LevelUp()
    {
        currentExp -= maxExp; // 남은 경험치 이월
        level++;

        // 다음 레벨업 요구치 증가 (예: 10 -> 15 -> 20)
        maxExp += 5;

        // 가방 확장 포인트 3점 지급
        expandPoints += 3;

        Debug.Log($" 레벨 업! 현재 레벨: {level} / 가방 확장 포인트: {expandPoints}");

        // TODO: 화면에 "레벨업! 잠긴 가방을 클릭해 확장하세요!" 같은 팝업을 띄우기
    }

    public void UpdateUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateHP(currentHp, maxHp);
            playerUI.UpdateStats(currentBlock, currentEnergy);
            playerUI.UpdateExp(currentExp, maxExp, level);
        }
    }
}
