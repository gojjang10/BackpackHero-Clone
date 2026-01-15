using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory : MonoBehaviour
{
    [Header("기본 몬스터 프리팹 (껍데기)")]
    public GameObject monsterBasePrefab;

    [Header("생성 위치")]
    public Transform spawnPoint;

    // 외부(BattleManager)에서 호출하는 함수
    public Monster SpawnMonster(BaseMonsterData data)
    {
        if (data == null)
        {
            Debug.LogError("생성할 몬스터 데이터가 없습니다!");
            return null;
        }

        GameObject go = Instantiate(monsterBasePrefab, spawnPoint.position, Quaternion.identity);
        go.transform.SetParent(spawnPoint);

        Monster monsterScript = go.GetComponent<Monster>();
        monsterScript.Init(data); // 껍데기에 데이터 주입

        return monsterScript;
    }
}
