using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monster Data/Base Monster")]
public class BaseMonsterData : ScriptableObject
{
    [Header("기본 정보")]
    public string monsterName;      // 이름 (예: 슬라임)
    public Sprite icon;             // 몬스터 이미지 (Sprite)

    [Header("전투 스탯")]
    public int maxHp;               // 최대 체력
    public int attackDamage;        // 기본 공격력
    public int defense;             // 기본 방어력 

    [Header("몬스터 보상")]
    public int xpReward;           // 처치 시 획득 경험치

    [Header("AI 패턴")]
    public MonsterPatternSO defaultPattern;
}

// 몬스터의 행동 데이터 구조체
[System.Serializable]
public struct MonsterIntent
{
    public MonsterMoveType type;
    public int value;

    public MonsterIntent(MonsterMoveType type, int value)
    {
        this.type = type;
        this.value = value;
    }
}
