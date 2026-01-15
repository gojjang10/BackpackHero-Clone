using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("현재 데이터 (읽기 전용)")]
    public BaseMonsterData data; // 현재 몬스터의 원본 데이터

    [Header("실시간 상태")]
    public int currentHp;
    public int currentBlock; // 방어도

    [Header("연결 요소")]
    public SpriteRenderer spriteRenderer; // 이미지를 바꿔주기 위해 필요
    public GameObject selectionMark;      // 선택 화살표

    // 팩토리가 이 함수를 호출해서 몬스터를 세팅함 (초기화)
    public void Init(BaseMonsterData inputData)
    {
        // 1. 데이터 주입
        data = inputData;

        // 2. 스탯 초기화
        currentHp = data.maxHp;
        currentBlock = 0;

        // 3. 외형 변경 (가장 중요! 데이터에 있는 이미지로 갈아끼움)
        if (spriteRenderer != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }

        // 4. 이름 변경 (하이어라키 창에서 보기 편하게)
        gameObject.name = data.monsterName;

        Debug.Log($" 몬스터 생성 완료: {data.monsterName} (HP: {currentHp})");
    }

    private void OnMouseDown()
    {
        // 배틀 매니저가 있고, 현재 전투 중이라면 타겟 등록 요청
        if (BattleManager.instance != null && GameManager.instance.currentState == GameState.Battle)
        {
            BattleManager.instance.SelectTarget(this);
        }
    }

    public void SetSelection(bool isSelected)
    {
        if (selectionMark != null)
        {
            selectionMark.SetActive(isSelected);
        }

    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        Debug.Log($" {data.monsterName}에게 {damage} 데미지! (남은 HP: {currentHp}/{data.maxHp})");

        // 여기서 사망 처리 로직 등을 추가할 수 있음
        if (currentHp <= 0)
        {
            Debug.Log($" {data.monsterName} 처치 완료!");
            Destroy(gameObject);
        }
    }
}
