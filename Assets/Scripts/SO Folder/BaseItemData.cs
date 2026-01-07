using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Base Item", menuName = "Item Data/Base Item Data")]
public class BaseItemData : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string id;               // 아이템 고유 ID
    public string itemName;         // 아이템 이름
    [TextArea] public string description; // 툴팁에 띄울 설명
    public Sprite icon;             // 인벤토리에서 보여질 이미지

    [Header("인벤토리 그리드에 적용되는 크기")]
    public int width = 1;           // 가로 크기
    public int height = 1;          // 세로 크기

    [Header("아이템 구분 및 기능")]
    public ItemType itemType;       // 액티브/패시브 구분
    public List<ItemTag> itemTags; // 태그 리스트 (Weapon, Armor 등)

    public List<BaseItemEffect> effects; // 이 아이템이 가진 특수 능력들 (공격, 회복, 시너지 등)

    // 초기화 위임 함수
    // InventoryItem(껍데기)이 생성될 때, 데이터(이 클래스)가 직접 스탯을 설정해주는 식으로 한번 구현해보기.
    // 자식 클래스(예를 들어 WeaponData)에서 override 해서 공격력 등을 추가할 예정.
    public virtual void ApplyBaseStats(InventoryItem itemInstance)
    {
        // 기본 BaseItemData는 특별한 스탯이 없으므로 비워둠.
        // 나중에 자식들이 이 함수를 덮어써서 자기만의 로직을 정의.
    }
}
