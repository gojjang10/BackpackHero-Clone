using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 스탯")]
    public string monsterName = "허수아비";
    public int maxHp = 100;
    public int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        Debug.Log($" {monsterName}에게 {damage} 데미지! (남은 HP: {currentHp}/{maxHp})");

        // 여기서 사망 처리 로직 등을 추가할 수 있음
        if (currentHp <= 0)
        {
            Debug.Log($" {monsterName} 처치 완료!");
        }
    }
}
