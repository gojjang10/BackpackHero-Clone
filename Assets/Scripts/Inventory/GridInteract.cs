using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridInteract : MonoBehaviour
{
    [Header("연결 필요")]
    public InventoryGrid inventoryGrid; // 인벤토리 그리드 참조
    [SerializeField] private Canvas canvas; // UI 렌더링 최상위 부모 (드래그 시 좌표 변환용)

    [Header("하이라이트 색상")]
    public Color validColor = new Color(0, 1, 0, 0.3f);   // 초록색 (반투명)
    public Color invalidColor = new Color(1, 0, 0, 0.3f); // 빨간색 (반투명)

    [Header("상태 변수")]
    public InventoryItem selectedItem; // 현재 들고 있는 아이템
    private RectTransform selectedItemRect; // 들고 있는 아이템의 RectTransform

    private Vector2Int originalGridPos; // 원래 위치 (X, Y) 묶어서 관리
    private Vector2Int currentMouseSlotPos;

    // 현재 하이라이트 된 슬롯들의 목록 (전체를 다 돌면서 끄지 않기 위해)
    private List<SlotUI> highlightedSlots = new List<SlotUI>();

    private void Update()
    {
        // 1. 아이템을 들고 있다면 마우스 따라다니기 (Dragging)
        if (selectedItem != null)
        {
            // 아이템의 위치를 마우스 위치로 갱신
            // (UI 모드일 때는 RectTransform의 position에 마우스 position을 넣으면 됨)
            selectedItemRect.position = Input.mousePosition;

            // [추가] 2. R키를 누르면 회전 시도
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateItem();
            }
        }
    }

    // 마우스가 슬롯에 들어왔을 때 -> 하이라이트 갱신
    public void OnEnterSlot(int x, int y)
    {
        currentMouseSlotPos.x = x;
        currentMouseSlotPos.y = y;

        // 아이템을 들고 있을 때만 하이라이트 표시
        if (selectedItem != null)
        {
            UpdateHighlight(x, y);
        }
    }

    // 마우스가 슬롯에서 나갔을 때 -> 하이라이트 끄기
    public void OnExitSlot(int x, int y)
    {
        // 나간 슬롯이 현재 슬롯과 같다면 초기화
        if (x == currentMouseSlotPos.x && y == currentMouseSlotPos.y)
        {
            currentMouseSlotPos = new Vector2Int(-1, -1);
            ClearHighlight();
        }
    }

    // SlotUI에서 클릭 이벤트가 발생하면 이 함수를 호출함
    public void OnClickSlot(int x, int y)
    {
        // [CASE A] 손에 아무것도 없을 때 -> 잡기 (Pick Up)
        if (selectedItem == null)
        {
            PickUpItem(x, y);
        }
        // [CASE B] 손에 아이템이 있을 때 -> 놓기 (Place)
        else
        {
            PlaceItem(x, y);
        }
    }

    // 아이템 집기 로직
    private void PickUpItem(int x, int y)
    {
        InventoryItem targetItem = inventoryGrid.GetItem(x, y);
        if (targetItem == null) return;

        // 1. 원래 위치 기억
        originalGridPos = new Vector2Int(targetItem.onGridX, targetItem.onGridY);

        // 2. 그리드에서 제거 및 선택 상태로 전환
        selectedItem = inventoryGrid.PickUpItem(x, y);
        selectedItemRect = selectedItem.GetComponent<RectTransform>();

        // 3. 렌더링 순서 조정 (제일 위로)
        if (canvas != null)
        {
            selectedItem.transform.SetParent(canvas.transform);
            selectedItem.transform.SetAsLastSibling();
        }

        // 4. 클릭 통과 설정 (Raycast Target Off)
        var img = selectedItem.GetComponent<Image>();
        if (img != null) img.raycastTarget = false;

        // 5. 집자마자 현재 마우스 위치 기준으로 하이라이트 갱신
        UpdateHighlight(x, y);

        Debug.Log($"[PickUp] {selectedItem.data.itemName}을(를) 집었습니다.");
    }

    // 아이템 놓기 로직
    private void PlaceItem(int x, int y)
    {
        // 1. 놓을 수 있는지 검사 (크기, 잠금 여부, 다른 아이템 충돌 등)
        bool canPlace = inventoryGrid.CheckPosition(x, y, selectedItem.Width, selectedItem.Height);

        if (canPlace)
        {
            // [성공] 새로운 위치에 배치
            inventoryGrid.PlaceItem(selectedItem, x, y);
            Debug.Log("아이템 배치 성공!");
            ExecuteItemEffects(selectedItem);
        }
        else
        {
            // [실패] 원래 위치로 되돌리기 (Return)

            // 회전 상태가 바뀌었을 수도 있으니, 원래 자리로 돌아갈 수 있는지 재검사
            bool canReturn = inventoryGrid.CheckPosition(originalGridPos.x, originalGridPos.y, selectedItem.Width, selectedItem.Height);

            if(canReturn)
            {
                inventoryGrid.PlaceItem(selectedItem, originalGridPos.x, originalGridPos.y);
            }
            else
            {
                Debug.Log("배치 실패! 원래 자리로 돌아갑니다.");
                selectedItem.Rotate();
                inventoryGrid.PlaceItem(selectedItem, originalGridPos.x, originalGridPos.y);
            }
        }

        selectedItem = null; // 손 비우기
        ClearHighlight();   // 하이라이트 끄기
    }

    private void RotateItem()
    {
        selectedItem.Rotate();

        if (currentMouseSlotPos.x != -1 && currentMouseSlotPos.y != -1)
        {
            UpdateHighlight(currentMouseSlotPos.x, currentMouseSlotPos.y);
        }
    }

    // 효과 실행을 위한 보조 함수 (깔끔하게 분리)
    private void ExecuteItemEffects(InventoryItem item)
    {
        foreach (var effect in item.data.effects)
        {
            // OnClick(테스트용)으로 설정된 효과를 놓는 순간 확인
            if (effect.triggerType == EffectTriggerType.OnClick)
            {
                effect.Execute(item, inventoryGrid);
            }
        }
    }

    private void UpdateHighlight(int startX, int startY)
    {
        // 1. 기존 하이라이트 싹 지우기 (잔상 방지)
        ClearHighlight();

        // 2. 배치 가능 여부 판단 -> 색상 결정
        bool isValid = inventoryGrid.CheckPosition(startX, startY, selectedItem.Width, selectedItem.Height);
        Color highlightColor = isValid ? validColor : invalidColor;

        // 3. 아이템 크기만큼 반복하며 슬롯 색칠하기
        for (int x = 0; x < selectedItem.Width; x++)
        {
            for (int y = 0; y < selectedItem.Height; y++)
            {
                int targetX = startX + x;
                int targetY = startY + y;

                // 유효한 좌표에 있는 슬롯만 칠하기
                SlotUI slot = inventoryGrid.GetSlotUI(targetX, targetY); 
                if (slot != null)
                {
                    slot.SetHighlight(true, highlightColor);

                    // 변경된 슬롯을 리스트에 추가 (나중에 이것만 끄기 위해)
                    highlightedSlots.Add(slot);
                }
            }
        }
    }

    // 모든 슬롯의 하이라이트 끄기
    private void ClearHighlight()
    {
        // 이전에 색칠했던 슬롯들만 찾아서 끄기
        foreach (var slot in highlightedSlots)
        {
            if (slot != null)
            {
                slot.SetHighlight(false, Color.white);
            }
        }
        // 리스트 비우기
        highlightedSlots.Clear();
    }
}

