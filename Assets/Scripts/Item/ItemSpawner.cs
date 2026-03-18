using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("연결 필요")]
    public InventoryGrid inventoryGrid; // 하이어라키의 그리드 패널 연결
    public GameObject itemPrefab;       // InventoryItem 스크립트가 붙은 프리팹
    public GridInteract gridInteract;

    [Header("보상 시스템 연결")]
    public Transform rewardPanel;

    [Header("테스트 데이터")]
    public BaseItemData[] itemsToSpawn; // 1x1, 2x1, 2x2 데이터를 여기에 드래그앤드롭

    void Update()
    {
        // 스페이스바를 누르면 랜덤 아이템 생성 시도
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnRandomItem();
            inventoryGrid.RecalculateAllStats();    // 생성 후 스탯 재계산
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // 테스트: 5개 생성해보기
            SpawnRewardItems(5);
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
        // 임의로 tileSize를 100으로 지정
        newItem.Initialize(selectedData, 100, gridInteract);

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

    // 보상 아이템들을 보상 패널에 생성하는 함수
    public void SpawnRewardItems(int count)
    {
        // 1. 기존에 보상 패널에 있던 아이템들 청소 (재시작 시 필요)
        foreach (Transform child in rewardPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. 개수만큼 생성
        for (int i = 0; i < count; i++)
        {
            if (itemsToSpawn.Length == 0) return;

            // 랜덤 데이터 선택
            int randomIndex = Random.Range(0, itemsToSpawn.Length);
            BaseItemData selectedData = itemsToSpawn[randomIndex];

            // 생성
            GameObject newItemObj = Instantiate(itemPrefab);
            InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();

            // 3. 부모를 '보상 패널'로 설정
            newItemObj.transform.SetParent(rewardPanel, false);

            // 4. 데이터 초기화 
            newItem.Initialize(selectedData, 100, gridInteract); // 100은 타일 사이즈

            // 아이템이 인벤토리 밖(보상 패널)에 있으므로,
            // GridInteract가 집을 때 "그리드에 없는 상태"임을 알아야 함.
            newItem.onGridX = -1;
            newItem.onGridY = -1;

            Debug.Log($"보상 생성 완료: {selectedData.itemName}");
        }
    }

    public void SpawnShopItems(int count, Transform shopContainer)
    {
        // 1. 진열장 청소
        foreach (Transform child in shopContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 개수만큼 생성
        for (int i = 0; i < count; i++)
        {
            if (itemsToSpawn.Length == 0) return;

            // 랜덤 데이터 선택
            int randomIndex = Random.Range(0, itemsToSpawn.Length);
            BaseItemData selectedData = itemsToSpawn[randomIndex];

            // 껍데기 생성
            GameObject newItemObj = Instantiate(itemPrefab);

            // ★ 부모를 매개변수로 받은 shopContainer로 설정 (UI 스케일 꼬임 방지)
            newItemObj.transform.SetParent(shopContainer, false);

            InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();

            // 데이터 초기화
            newItem.Initialize(selectedData, 100, gridInteract);
            newItem.onGridX = -1;
            newItem.onGridY = -1;

            // ★ 상점 모드로 세팅 (가격표 켜기)
            newItem.SetupAsShopItem();

            Debug.Log($"상점 진열 완료: {selectedData.itemName}");
        }
    }

    // 게임 시작 시 확정적으로 무기 1개, 방어구 1개, 랜덤 1개를 주는 함수
    public void SpawnOpeningItems()
    {
        // 1. 기존에 보상 패널에 있던 아이템들 청소
        foreach (Transform child in rewardPanel)
        {
            Destroy(child.gameObject);
        }

        if (itemsToSpawn.Length == 0) return;

        // 2. 전체 아이템 풀에서 무기와 방어구 분리하기
        List<BaseItemData> weapons = new List<BaseItemData>();
        List<BaseItemData> armors = new List<BaseItemData>();

        foreach (var item in itemsToSpawn)
        {
            // 아이템 태그에 Weapon이 있으면 무기 리스트에, Armor가 있으면 방어구 리스트에 추가
            if (item.itemTags.Contains(ItemTag.Weapon))
            {
                weapons.Add(item);
            }
            else if (item.itemTags.Contains(ItemTag.Armor))
            {
                armors.Add(item);
            }
        }

        // 3. 스폰할 아이템 3개를 담을 리스트 준비
        List<BaseItemData> selectedItems = new List<BaseItemData>();

        // 무기 1개 뽑기 (무기가 풀에 없으면 에러 방지용으로 아무거나 뽑음)
        if (weapons.Count > 0)
            selectedItems.Add(weapons[Random.Range(0, weapons.Count)]);
        else
            selectedItems.Add(itemsToSpawn[Random.Range(0, itemsToSpawn.Length)]);

        // 방어구 1개 뽑기
        if (armors.Count > 0)
            selectedItems.Add(armors[Random.Range(0, armors.Count)]);
        else
            selectedItems.Add(itemsToSpawn[Random.Range(0, itemsToSpawn.Length)]);

        // 남은 1개는 전체 풀에서 완전 랜덤으로 뽑기
        selectedItems.Add(itemsToSpawn[Random.Range(0, itemsToSpawn.Length)]);

        // 4. 선택된 3개의 아이템을 기존과 완벽하게 똑같은 방식으로 화면에 생성
        foreach (var data in selectedItems)
        {
            GameObject newItemObj = Instantiate(itemPrefab);

            // 부모를 '보상 패널'로 설정
            newItemObj.transform.SetParent(rewardPanel, false);

            InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();

            // 데이터 초기화 
            newItem.Initialize(data, 100, gridInteract);
            newItem.onGridX = -1;
            newItem.onGridY = -1;

            Debug.Log($"오프닝 보상 생성 완료: {data.itemName}");
        }
    }
}
    
