using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BaseItemData data; // 아이템 원본 데이터
    public Image itemImage; // 아이템 이미지

    [Header("하이라이트 오브젝트")]    
    public GameObject highlightObject;

    public bool isRotated = false; // 회전 여부 (true = 90도 회전됨)

    public int Width
    {
        get { return isRotated ? data.height : data.width; }
    }

    public int Height
    {
        get { return isRotated ? data.width : data.height; }
    }

    [Header("실시간 아이템 변수")]
    // BaseItemData에서 초기화된 후, 게임 중에 변할 수 있는 값들
    public int currentAttack;     // 공격력
    public int currentBlock;    // 방어력
    public int currentEnergyCost; // 현재 마나 소모량 (시너지 등으로 변할 수 있음)

    [Header("그리드 포지션")]
    public int onGridX; // 현재 아이템의 그리드 X 좌표
    public int onGridY; // 현재 아이템의 그리드 Y 좌표

    [Header("상점 UI")]
    public TextMeshProUGUI priceText; // 아이템 프리팹 하위의 가격 텍스트
    public bool isInShop = false;     // 현재 상점 진열대에 있는지 여부

    // 내부 컴포넌트
    private GridInteract gridInteract;
    private RectTransform rectTransform;    // 아이템 RectTransform

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 아이템 생성 시 호출되는 초기화 함수
    public void Initialize(BaseItemData data, int tileSize, GridInteract manager)
    {
        this.data = data;
        this.gridInteract = manager;

        // 1. 이미지 설정
        if (itemImage != null)
        {
            itemImage.sprite = data.icon;
            SetRaycastTarget(true);
        }

        // 2. 크기 설정
        SetSize(tileSize);

        // 3. 스탯 초기화 (데이터 원본에 있는 수치를 가져옴)
        ResetStats();
    }

    // 아이템 클릭 이벤트 함수
    public void OnPointerDown(PointerEventData eventData)
    {

        if (gridInteract != null) gridInteract.OnItemClicked(this);
    }

    // 아이템에 마우스가 들어왔을 때 호출되는 이벤트 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gridInteract != null) gridInteract.OnItemPointerEnter(this);
    }

    // 아이템에서 마우스가 나갔을 때 호출되는 이벤트 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gridInteract != null) gridInteract.OnItemPointerExit(this);
    }

    // Raycast Target 제어 (집었을 때 끄고, 놓았을 때 켜기 위함)
    public void SetRaycastTarget(bool isOn)
    {
        if (itemImage != null)
        {
            itemImage.raycastTarget = isOn;
        }
    }

    // 시너지 재계산 전에 "순정 상태"로 돌리는 함수
    public void ResetStats()
    {
        // 1. 일단 모든 스탯 0으로 초기화 (찌꺼기 제거)
        currentAttack = 0;
        currentBlock = 0;

        // 2. 데이터(설계도)에 적힌 기본값 적용
        data.ApplyBaseStats(this);
    }



    // 타일 크기에 맞춰 UI 크기 조절 (리팩토링으로 분리함)
    public void SetSize(int tileSize)
    {
        if (rectTransform != null)
        {
            Vector2 newSize = new Vector2(data.width * tileSize, data.height * tileSize);
            rectTransform.sizeDelta = newSize;
        }
    }

    // 아이템 회전 로직 (GridInteract에서 R키를 누르면 호출)
    public void Rotate()
    {
        // 1. 논리적 상태 토글 (가로/세로 개념이 바뀜)
        isRotated = !isRotated;

        // 2. 시각적 회전 (UI 이미지를 90도 돌림)
        if (rectTransform != null)
        {
            // 시계 방향으로 90도 회전 (Z축 -90)
            // 0도와 -90도를 왔다갔다함
            rectTransform.localRotation = Quaternion.Euler(0, 0, isRotated ? -90 : 0);
        }
    }

    // 아이템 하이라이트 켜기/끄기
    public void SetHighlight(bool isActive)
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(isActive);
        }
    }

    // 가격 표시 활성화
    public void SetupAsShopItem()
    {
        isInShop = true;
        if (priceText != null)
        {
            priceText.gameObject.SetActive(true);
            priceText.text = data.basePrice.ToString() + " G"; // 가격 표시
        }
    }
    // 가격 표시 비활성화
    public void SetAsInventoryItem()
    {
        isInShop = false;
        if (priceText != null)
        {
            priceText.gameObject.SetActive(false); // 가방에 들어오면 가격표 숨김
        }
    }
}
