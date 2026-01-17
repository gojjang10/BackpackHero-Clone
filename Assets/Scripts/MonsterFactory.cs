using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterFactory : MonoBehaviour
{
    [Header("기본 몬스터 프리팹 (껍데기)")]
    public GameObject monsterBasePrefab;

    // 외부(BattleManager)에서 호출하는 함수
    public Monster SpawnMonster(BaseMonsterData data, Vector3 position)
    {
        if (data == null)
        {
            Debug.LogError("생성할 몬스터 데이터가 없습니다!");
            return null;
        }

        // 1. 전달받은 위치(position)에 생성
        GameObject go = Instantiate(monsterBasePrefab, position, Quaternion.identity);

        // 2. 부모 설정 (하이어라키 창 정리를 위해 Factory 자식으로 넣거나, 필요 없으면 생략 가능)
        go.transform.SetParent(this.transform);

        Monster monsterScript = go.GetComponent<Monster>();
        monsterScript.Init(data); // 껍데기에 데이터 주입

        return monsterScript;
    }
}
