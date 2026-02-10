using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 개별 행동 패턴 정의
[System.Serializable]
public class PatternDefinition
{
    public MonsterMoveType moveType;
    public int minValue; // 최소 위력
    public int maxValue; // 최대 위력
    [Range(1, 100)] public int weight; // 확률 가중치
}

// AI 패턴 데이터 (SO)
[CreateAssetMenu(fileName = "NewMonsterPattern", menuName = "Monster/Pattern")]
public class MonsterPatternSO : ScriptableObject
{
    [Header("가능한 행동 목록")]
    public List<PatternDefinition> patterns;

    // 가중치 랜덤으로 행동 하나를 뽑아주는 함수
    public MonsterIntent GetNextIntent()
    {
        if (patterns == null || patterns.Count == 0)
            return new MonsterIntent(MonsterMoveType.Wait, 0);

        // 1. 가중치 합 계산
        int totalWeight = 0;
        foreach (var p in patterns) totalWeight += p.weight;

        // 2. 룰렛 돌리기
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var p in patterns)
        {
            currentWeight += p.weight;
            if (randomValue < currentWeight)
            {
                int val = Random.Range(p.minValue, p.maxValue + 1);
                return new MonsterIntent(p.moveType, val);
            }
        }

        return new MonsterIntent(MonsterMoveType.Wait, 0);
    }
}
