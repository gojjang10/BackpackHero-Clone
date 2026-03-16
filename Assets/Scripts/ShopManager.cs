using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("연결")]
    public Transform shopItemContainer; // 상점 아이템이 진열될 패널 
    public ItemSpawner itemSpawner;     // 아이템 생성기
    public Player player;

    // 상점 아이템 생성 함수
    public void GenerateShopItems(int itemCount = 3)
    {
        if (itemSpawner != null && shopItemContainer != null)
        {
            // 스포너에게 생성을 완벽하게 위임
            itemSpawner.SpawnShopItems(itemCount, shopItemContainer);
        }
        else
        {
            Debug.LogError("ShopManager에 진열장(Container)이나 스포너가 연결되지 않았습니다!");
        }
    }
}
