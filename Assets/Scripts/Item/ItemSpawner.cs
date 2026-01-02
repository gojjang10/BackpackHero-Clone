using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("연결 필요")]
    public InventoryGrid inventoryGrid; // 하이어라키의 그리드 패널 연결
    public GameObject itemPrefab;       // InventoryItem 스크립트가 붙은 프리팹

    [Header("테스트 데이터")]
    public BaseItemData[] itemsToSpawn; // 1x1, 2x1, 2x2 데이터를 여기에 드래그앤드롭

    void Update()
    {
        // 스페이스바를 누르면 랜덤 아이템 생성 시도
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        // 1. 데이터 랜덤 선택
        if (itemsToSpawn.Length == 0) return;
        int randomIndex = Random.Range(0, itemsToSpawn.Length);
        BaseItemData selectedData = itemsToSpawn[randomIndex];

        // 2. 아이템 객체(껍데기) 생성
        GameObject newItemObj = Instantiate(itemPrefab);
        InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();

        // 3. 데이터 주입 (Initialize 호출)
        newItem.Initialize(selectedData);

        if (inventoryGrid.AutoPlaceItem(newItem))
        {
            Debug.Log($"생성 성공: {selectedData.itemName}");
        }
        else
        {
            Debug.Log("인벤토리가 꽉 찼습니다!");
            Destroy(newItemObj);
        }
    }
}
    
