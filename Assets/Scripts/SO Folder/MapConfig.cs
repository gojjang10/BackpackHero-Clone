using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapConfig", menuName = "Map/Map Config")]
public class MapConfig : ScriptableObject
{
    [Header("맵 크기")]
    public int gridWidth = 12;  // 가로 길이 (진행 단계)
    public int gridHeight = 7;  // 세로 길이 (폭)

    [Header("생성 규칙")]
    public int maxPathCount = 3; // 길을 몇 갈래로 뚫을지
    [Range(0f, 1f)] public float branchProbability = 0.3f; // 위아래로 튈 확률

    [Header("방 생성 확률 (가중치)")]
    public int battleWeight = 60;
    public int shopWeight = 20;
    public int eventWeight = 20;
}
