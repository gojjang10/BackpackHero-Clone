using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("연결")]
    public MonsterFactory factory;      // 팩토리 참조
    public Transform spawnCenter;       // 배치 기준점 (빈 오브젝트)

    [Header("배치 설정")]
    public float spacing = 3.0f;        // 몬스터 간격

    // 단일 몬스터 소환 (테스트용)
    public Monster SpawnSingle(BaseMonsterData data)
    {
        // 중앙에 딱 하나 소환
        return factory.SpawnMonster(data, spawnCenter.position);
    }

    // 다수 몬스터 소환 & 자동 줄세우기 로직
    public List<Monster> SpawnWave(List<BaseMonsterData> datas)
    {
        List<Monster> spawnedList = new List<Monster>();

        for (int i = 0; i < datas.Count; i++)
        {
            // 중앙 기준 좌우 정렬 좌표 계산
            float xPos = (i - (datas.Count - 1) / 2f) * spacing;
            Vector3 spawnPos = spawnCenter.position + new Vector3(xPos, 0, 0);

            Monster m = factory.SpawnMonster(datas[i], spawnPos);
            spawnedList.Add(m);
        }
        return spawnedList;
    }
}
