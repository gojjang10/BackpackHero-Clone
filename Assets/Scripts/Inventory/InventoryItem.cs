using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public BaseItemData data; // 아이템 원본 데이터

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

    // 아이템 생성 시 호출되는 초기화 함수
    public void Initialize(BaseItemData data, int tileSize)
    {
        this.data = data;

        // 1. 이미지 설정
        if (itemImage != null)
        {
            itemImage.sprite = data.icon;
        }

        // 2. 크기 설정
        SetSize(tileSize);

        // 3. 스탯 초기화 (데이터 원본에 있는 수치를 가져옴)
        data.InitializeStats(this);
    }

    // 타일 크기에 맞춰 UI 크기 조절 (리팩토링으로 분리함)
    public void SetSize(int tileSize)
    {
        if (rectTransform != null)
        {
            // 초기 사이즈 설정 (회전되지 않은 상태 기준)
            rectTransform.sizeDelta = new Vector2(data.width * tileSize, data.height * tileSize);
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
}
