using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridInteract : MonoBehaviour
{
    [Header("연결 필요")]
    public InventoryGrid inventoryGrid; // 인벤토리 그리드 참조
    [SerializeField] private Canvas canvas; // UI 렌더링 최상위 부모 (드래그 시 좌표 변환용)

    [Header("상태 변수")]
    public InventoryItem selectedItem; // 현재 들고 있는 아이템
    private RectTransform selectedItemRect; // 들고 있는 아이템의 RectTransform

    // 되돌리기 위한 위치 기억
    private int originalX = -1;
    private int originalY = -1;

    private void Update()
    {
        // 1. 아이템을 들고 있다면 마우스 따라다니기 (Dragging)
        if (selectedItem != null)
        {
            // 아이템의 위치를 마우스 위치로 갱신
            // (UI 모드일 때는 RectTransform의 position에 마우스 position을 넣으면 됨)
            selectedItemRect.position = Input.mousePosition;
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
        // 1. 해당 좌표에 아이템이 있는지 확인 (InventoryGrid 함수 활용)
        InventoryItem targetItem = inventoryGrid.GetItem(x, y);

        if (targetItem != null)
        {
            // 2. 원래 위치 기억 (실패 시 되돌리기 위해)
            originalX = targetItem.onGridX;
            originalY = targetItem.onGridY;

            // 3. 그리드에서 아이템 제거 (데이터 비우기)
            // (PickUpItem 함수는 아이템을 리턴하면서 그리드에서는 지워줌)
            selectedItem = inventoryGrid.PickUpItem(x, y);
            selectedItemRect = selectedItem.GetComponent<RectTransform>();

            // 4. 시각적 처리: 아이템을 맨 앞으로 가져오기 (다른 슬롯에 가려지지 않게)
            if (canvas != null)
            {
                selectedItem.transform.SetParent(canvas.transform); // 캔버스 직속으로 옮겨서 제일 위에 그리기
                selectedItem.transform.SetAsLastSibling(); // 형제들 중 가장 마지막 = 가장 위에 그려짐
            }

            // 5. 클릭 통과 설정 (드래그 중에 내 아래에 있는 슬롯을 인식해야 하므로)
            // 아이템 이미지의 Raycast Target을 꺼야 슬롯 클릭이 됨.
            Image img = selectedItem.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;

            Debug.Log($"아이템 집음: {selectedItem.data.itemName}");
        }
    }

    // 아이템 놓기 로직
    private void PlaceItem(int x, int y)
    {
        // 1. 놓을 수 있는지 검사 (크기, 잠금 여부, 다른 아이템 충돌 등)
        bool canPlace = inventoryGrid.CheckPosition(x, y, selectedItem.data.width, selectedItem.data.height);

        if (canPlace)
        {
            // [성공] 새로운 위치에 배치
            inventoryGrid.PlaceItem(selectedItem, x, y);
            Debug.Log("아이템 배치 성공!");
        }
        else
        {
            // [실패] 원래 위치로 되돌리기 (Return)
            Debug.Log("배치 실패! 원래 자리로 돌아갑니다.");
            inventoryGrid.PlaceItem(selectedItem, originalX, originalY);
        }

        selectedItem = null; // 손 비우기
    }
}

