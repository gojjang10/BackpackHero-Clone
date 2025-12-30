using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [Header("크기 설정 (전체 맵 크기)")]
    public int maxColumns = 8;  // 가로 (Width)
    public int maxRows = 5;     // 세로 (Height)

    [Header("초기 시작 크기 (해금 영역)")]
    public int startColumns = 4; // 처음에 열려있는 가로 칸 수
    public int startRows = 3;    // 처음에 열려있는 세로 칸 수

    [Header("UI 설정")]
    public float tileSize = 100f;     // 슬롯 크기 (픽셀)
    public GameObject slotPrefab;     // SlotUI 프리팹
    public Transform slotParent;      // 슬롯이 생성될 부모 (GridPanel)

    // 내부 관리용
    private InventorySlot[,] logicalGrid;   // 아이템 데이터 그리드
    private SlotUI[,] visualGrid;           // UI 그리드

    // 디버깅용 (인스펙터에서 리스트로 확인 가능)
    public List<InventorySlot> debugSlotList;

    private void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        logicalGrid = new InventorySlot[maxColumns, maxRows];
        visualGrid = new SlotUI[maxColumns, maxRows];
        debugSlotList = new List<InventorySlot>();

        // 중앙 정렬을 위한 오프셋 계산
        float xOffset = (maxColumns - 1) * 0.5f;   
        float yOffset = (maxRows - 1) * 0.5f;

        // 초기 해금 시작 인덱스 계산 (가운데서부터 시작)
        int startXIndex = (maxColumns - startColumns) / 2;
        int startYIndex = (maxRows - startRows) / 2;

        for (int x = 0; x < maxColumns; x++)
        {
            for (int y = 0; y < maxRows; y++)
            {
                // 1. 논리 슬롯 생성
                logicalGrid[x, y] = new InventorySlot(x, y);
                debugSlotList.Add(logicalGrid[x, y]);   // 디버깅용 리스트에 추가

                // 2. 시각 슬롯(UI) 생성
                GameObject slotObj = Instantiate(slotPrefab, slotParent);   
                slotObj.name = $"Slot_{x}_{y}"; // 하이어라키창에서 보기 쉽게 이름 지정

                SlotUI slotUI = slotObj.GetComponent<SlotUI>();
                if (slotUI != null) slotUI.SetCoord(x, y);  // 슬롯들이 가지는 좌표 설정
                visualGrid[x, y] = slotUI;

                // 3. 위치 잡기 (중앙 정렬)
                RectTransform rect = slotObj.GetComponent<RectTransform>();
                float posX = (x - xOffset) * tileSize;
                float posY = (y - yOffset) * tileSize;
                rect.anchoredPosition = new Vector2(posX, posY);

                // 4. 일단 모두 잠금 상태로 시작 (UI 업데이트)
                logicalGrid[x, y].isUnlocked = false;
                UpdateSlotVisual(x, y);
            }
        }

        // 5. 초기 시작 영역 해금
        for (int x = 0; x < startColumns; x++)
        {
            for (int y = 0; y < startRows; y++)
            {
                UnlockSlot(startXIndex + x, startYIndex + y);
            }
        }
    }

    // 슬롯 해금 함수
    public void UnlockSlot(int x, int y)
    {
        if (IsValidCoordinate(x, y))
        {
            logicalGrid[x, y].isUnlocked = true;
            UpdateSlotVisual(x, y);
        }
    }

    // UI 상태 업데이트 (잠김/해제 시각화)
    void UpdateSlotVisual(int x, int y)
    {
        if (visualGrid[x, y] == null) return;

        bool unlocked = logicalGrid[x, y].isUnlocked;

        // SlotUI에게 시각적 상태 변경 요청
        visualGrid[x, y].SetLockedState(!unlocked);
    }

    // 좌표 유효성 검사
    public bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < maxColumns && y >= 0 && y < maxRows;
    }
}
