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
        // 그리드 배열 초기화
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

        bool isUnlocked = logicalGrid[x, y].isUnlocked;

        // SlotUI에게 시각적 상태 변경 요청
        visualGrid[x, y].SetLockedState(isUnlocked);
    }

    // 좌표 유효성 검사
    public bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < maxColumns && y >= 0 && y < maxRows;
    }

    // 배치 가능 여부 확인
    // startX, startY: 배치하려는 기준점 (왼쪽 아래부터 시작)
    // width, height: 아이템의 크기
    public bool CheckPosition(int startX, int startY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;

                // 1. 그리드 밖으로 나가는지? (좌표 유효성 검사)
                if (!IsValidCoordinate(checkX, checkY)) return false;

                // 2. 잠긴 슬롯인지?
                if (!logicalGrid[checkX, checkY].isUnlocked) return false;

                // 3. 이미 다른 아이템이 있는지?
                if (!logicalGrid[checkX, checkY].IsEmpty()) return false;
            }
        }

        // 모든 검사 통과. true 반환
        return true;
    }

    // [아이템 배치 실행]
    // 검사가 끝난(CheckPosition 통과) 아이템을 실제로 데이터에 등록하고 위치를 잡음
    public void PlaceItem(InventoryItem item, int startX, int startY)
    {
        BaseItemData data = item.data; // InventoryItem의 데이터 참조

        // 1. 아이템에게 자신의 위치 기억시키기
        item.onGridX = startX;
        item.onGridY = startY;

        // 2. 논리적 그리드(데이터) 채우기
        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                // 해당 슬롯에 "이 아이템이 차지하고 있음" 표시
                logicalGrid[startX + x, startY + y].inventoryItem = item;
            }
        }

        // 3. 시각적 위치 설정 (UI 이동)
        RectTransform rt = item.GetComponent<RectTransform>();
        rt.SetParent(slotParent); // 그리드 패널의 자식으로 설정 (그래야 같이 움직임)
        rt.localScale = Vector3.one; // 크기 초기화

        // ★ 위치 계산 로직 (InitializeGrid와 동일한 오프셋 방식 적용)
        float xOffset = (maxColumns - 1) * 0.5f;
        float yOffset = (maxRows - 1) * 0.5f;

        // 아이템의 중심점 계산: (시작좌표 + 아이템너비의 절반)
        // 예: 너비가 2칸이면, 0.5칸만큼 더 오른쪽으로 가야 중심임.
        float centerX = startX + (data.width - 1) * 0.5f;
        float centerY = startY + (data.height - 1) * 0.5f;

        float posX = (centerX - xOffset) * tileSize;
        float posY = (centerY - yOffset) * tileSize;

        rt.anchoredPosition = new Vector2(posX, posY);
    }

    // [아이템 집기(제거)] - 나중에 GridInteract에서 쓸 것 미리 추가
    public InventoryItem PickUpItem(int x, int y)
    {
        // 좌표 유효성 검사 (예외 처리)
        if (!IsValidCoordinate(x, y)) return null;

        // 해당 좌표의 아이템 가져와서 유효성 검사
        InventoryItem item = logicalGrid[x, y].inventoryItem;
        if (item == null) return null;

        // 아이템이 차지하던 모든 슬롯 비우기
        CleanGridReference(item);

        return item;
    }

    // 아이템이 있던 자리 null로 만들기
    private void CleanGridReference(InventoryItem item)
    {
        for (int x = 0; x < item.data.width; x++)
        {
            for (int y = 0; y < item.data.height; y++)
            {
                int targetX = item.onGridX + x;
                int targetY = item.onGridY + y;
                if (IsValidCoordinate(targetX, targetY))
                {
                    logicalGrid[targetX, targetY].inventoryItem = null;
                }
            }
        }
    }

    // 특정 좌표의 아이템 가져오기
    public InventoryItem GetItem(int x, int y)
    {
        if (!IsValidCoordinate(x, y)) return null;
        return logicalGrid[x, y].inventoryItem;
    }

    // 특정 좌표의 SlotUI 가져오기
    public SlotUI GetSlotUI(int x, int y)
    {
        // 좌표 유효성 검사
        if (!IsValidCoordinate(x, y)) return null;
        return visualGrid[x, y];
    }


    // 자동 배치 시도 함수(테스트 용)
    public bool AutoPlaceItem(InventoryItem item)
    {
        // 왼쪽 아래(0,0)부터 순서대로 탐색
        for (int y = 0; y < maxRows; y++)
        {
            for (int x = 0; x < maxColumns; x++)
            {
                if (CheckPosition(x, y, item.data.width, item.data.height))
                {
                    PlaceItem(item, x, y);
                    return true; // 배치 성공!
                }
            }
        }
        return false; // 자리 없음
    }
}
