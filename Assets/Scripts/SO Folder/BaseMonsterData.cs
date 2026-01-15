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
}
