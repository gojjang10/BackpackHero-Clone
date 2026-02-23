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
    public int neutralWeight = 60;
    public int battleWeight = 20;
    public int shopWeight = 20;

    [Header("전투 설정 (몬스터 풀)")]
    public List<BaseMonsterData> possibleMonsterDatas;  // 이 맵에서 나올 수 있는 몬스터 데이터 리스트
    public int minMonsterCount = 1; // 최소 등장 마릿수
    public int maxMonsterCount = 3; // 최대 등장 마릿수

    [Header("보스 설정")]
    public bool isBossStage = false;    // 보스 스테이지 여부
    public BaseMonsterData bossMonsterData; // isBossStage가 true일 때 소환될 보스 데이터
}
