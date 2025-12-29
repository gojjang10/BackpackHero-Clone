using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public BaseItemData data; // 아이템 원본 데이터
    
    [Header("실시간 아이템 변수")]
    // BaseItemData에서 초기화된 후, 게임 중에 변할 수 있는 값들
    public int finalDamage;     // 공격력
    public int finalDefense;    // 방어력

    [Header("그리드 포지션")]
    public int onGridX; // 현재 아이템의 그리드 X 좌표
    public int onGridY; // 현재 아이템의 그리드 Y 좌표

    // 내부 컴포넌트
    private Image itemImage; // 아이템 이미지
    private RectTransform rectTransform;    // 아이템 RectTransform

    private void Awake()
    {
        // 초기화
        itemImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(BaseItemData data)
    {
        this.data = data;

        // 이미지 및 크기 설정
        if (itemImage != null) itemImage.sprite = data.icon;

        // Grid의 CellSize를 받아와서 곱해야 함. 우선 임의로 50으로 설정.
        rectTransform.sizeDelta = new Vector2(data.width * 50, data.height * 50);

        // 스탯 초기화 위임
        data.InitializeStats(this);
    }
}
