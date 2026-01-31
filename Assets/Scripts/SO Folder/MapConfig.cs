using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapConfig", menuName = "Map/Map Config")]
public class MapConfig : ScriptableObject
{
    [Header("그리드 설정")]
    public int gridWidth = 12;  // 가로 길이 (진행 단계)
    public int gridHeight = 7;  // 세로 길이 (갈림길 폭)

    [Header("생성 규칙")]
    public int maxPathCount = 3; // 생성할 길(Path)의 개수
    [Range(0f, 1f)] public float branchProbability = 0.3f; // 곁가지 확률

    [Header("노드 생성 가중치 (확률)")]
    public int battleWeight = 50;
    public int shopWeight = 15;
    public int eventWeight = 20;
    public int restWeight = 10;
    public int treasureWeight = 5;
}
