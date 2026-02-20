using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridInteract : MonoBehaviour
{
    // 조작 모드 정의 (내부용)
    public enum InteractMode { Manage, Use }
    public InteractMode currentMode = InteractMode.Manage; // 기본은 정리 모드

    // 외부에서 아이템을 들고 있는지 확인할 수 있는 프로퍼티
    public bool IsDraggingItem => selectedItem != null;
    //플레이어가 확장 포인트를 가지고 있는지 확인하는 프로퍼티
    public bool HasPendingExpansion => player != null && player.expandPoints > 0;

    [Header("연결 필요")]
    public InventoryGrid inventoryGrid; // 인벤토리 그리드 참조
    [SerializeField] private Canvas canvas; // UI 렌더링 최상위 부모 (드래그 시 좌표 변환용)
    public Player player; // 플레이어 참조 

    [Header("하이라이트 색상")]
    public Color validColor = new Color(0, 1, 0, 0.3f);   // 초록색 (반투명)
    public Color invalidColor = new Color(1, 0, 0, 0.3f); // 빨간색 (반투명)

    [Header("상태 변수")]
    public InventoryItem selectedItem; // 현재 들고 있는 아이템
    private RectTransform selectedItemRect; // 들고 있는 아이템의 RectTransform

    [Header("외부 아이템 보관소")]
    public Transform worldItemHolder; 


    [Header("UI")]
    public UITooltip uiTooltip; // 인스펙터에서 할당

    private Vector2Int originalGridPos; // 원래 위치 (X, Y) 묶어서 관리
    private Vector2Int currentMouseSlotPos;

    // 현재 배치가 가능한 슬롯의 하이라이트 된 슬롯들의 목록 (전체를 다 돌면서 끄지 않기 위해)
    private List<SlotUI> highlightedSlots = new List<SlotUI>();

    // 현재 시너지 미리보기로 하이라이트 켜진 아이템 목록
    private List<InventoryItem> currentPreviewTargets = new List<InventoryItem>();

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

    #region [Input Handlers] 입력 이벤트 핸들러 (외부에서 호출)

    // 아이템 클릭 (InventoryItem.OnPointerDown에서 호출) -> 집기/사용
    public void OnItemClicked(InventoryItem item)
    {
        if (selectedItem != null) return;

        // 1. 아이템 사용을 시도해야 하는 상황인지 판단
        // 조건: [사용 모드]이면서 동시에 [전투 상태]여야 함
        bool shouldTryUse = (currentMode == InteractMode.Use && GameManager.instance.currentState == GameState.Battle);

        if (shouldTryUse)
        {
            // [사용] 전투 중 + 사용 모드 -> 아이템 사용
            BattleManager.instance.OnUseItem(item);
        }
        else
        {
            // [집기] 그 외 모든 상황
            // 1. 정리 모드일 때
            // 2. 사용 모드지만 전투가 끝났을 때 (보상 루팅 등) -> 여기서 자연스럽게 집기로 넘어감
            PickUpItem(item);
        }
    }

    // 슬롯 클릭 (SlotUI.OnPointerDown에서 호출) -> 놓기
    public void OnClickSlot(int x, int y)
    {
        // 1. 클릭한 슬롯의 데이터 가져오기
        InventorySlot clickedSlot = inventoryGrid.GetLogicalSlot(x, y);

        if (clickedSlot == null) return;

        if (!clickedSlot.isUnlocked)
        {
            // 플레이어 연결 확인 및 포인트 체크
            if (player != null && player.expandPoints > 0)
            {
                // 1. 포인트 차감
                player.expandPoints--;

                // 2. 그리드에게 해금 명령 (데이터 변경 + 색상 변경)
                inventoryGrid.UnlockSlot(x, y);

                // 3. 플레이어 정보 갱신 (포인트 줄어든 것 반영)
                player.UpdateUI();

                Debug.Log($" 가방 칸 해금 완료! (남은 포인트: {player.expandPoints})");
            }
            else
            {
                // 포인트가 없거나 플레이어 연결이 안 된 경우
                Debug.Log(" 가방을 확장할 포인트가 부족합니다! (레벨업이 필요합니다)");
            }

            // 잠긴 칸을 클릭했을 때는 '아이템 배치'를 하지 않고 여기서 함수 종료
            return;
        }
        // 손에 아이템이 있을 때만 배치 시도
        if (selectedItem != null)
        {
            PlaceItemOnGrid(x, y);
        }
    }

    // 배경 클릭 (BackgroundInteract.OnPointerDown에서 호출) -> 떨구기
    public void DropItemOutside()
    {
        // 좌표 정보 초기화 (-1은 '밖'을 의미)
        selectedItem.onGridX = -1;
        selectedItem.onGridY = -1;

        // 다시 클릭 가능하게
        selectedItem.SetRaycastTarget(true);

        selectedItem.transform.SetParent(worldItemHolder);

        selectedItem.data.ApplyBaseStats(selectedItem); // 기본 스탯 복원

        Debug.Log($"{selectedItem.data.itemName}을(를) 바닥에 두었습니다.");

        selectedItem = null;
        ClearHighlight();
        ClearSynergyPreview();
    }

    // 아이템 호버링 (InventoryItem.OnPointerEnter) -> 툴팁 켜기
    public void OnItemPointerEnter(InventoryItem item)
    {
        // 아이템을 들고 있지 않을 때만 툴팁 표시
        if (selectedItem == null)
        {
            uiTooltip.ShowTooltip(item);
        }
    }

    // 아이템 호버링 종료 (InventoryItem.OnPointerExit) -> 툴팁 끄기
    public void OnItemPointerExit(InventoryItem item)
    {
        uiTooltip.HideTooltip();
    }

    // 모드 토글 함수 (UI 버튼에 연결)
    public void ToggleInteractMode()
    {
        currentMode = (currentMode == InteractMode.Manage) ? InteractMode.Use : InteractMode.Manage;
        Debug.Log($" 조작 모드 변경: {currentMode}");
    }
    #endregion

    #region [Core Logic] 핵심 기능 (집기, 배치)

    // 아이템 집기 로직
    private void PickUpItem(InventoryItem item)
    {
        // 1. 아이템이 그리드 안에 있었다면? -> 그리드 데이터에서 제거
        if (item.onGridX != -1 && item.onGridY != -1)
        {
            originalGridPos = new Vector2Int(item.onGridX, item.onGridY);
            inventoryGrid.PickUpItem(item.onGridX, item.onGridY); // 그리드 배열 비우기
        }
        else
        {
            // 그리드 밖(보상 패널, 바닥)에 있던 아이템임 -> 돌아갈 곳 없음
            originalGridPos = new Vector2Int(-1, -1);
        }

        // 2. 선택 상태로 전환
        selectedItem = item;
        selectedItemRect = selectedItem.GetComponent<RectTransform>();

        // 3. 렌더링 최상위로 이동
        if (canvas != null)
        {
            selectedItem.transform.SetParent(canvas.transform);
            selectedItem.transform.SetAsLastSibling();
        }

        // 집는 동안 Raycast 끄기 (아래 슬롯 클릭 가능하게)
        selectedItem.SetRaycastTarget(false);

        inventoryGrid.RecalculateAllStats(); // 시너지 재계산

        // 즉시 하이라이트 갱신
        if (currentMouseSlotPos.x != -1)
        {
            UpdateHighlight(currentMouseSlotPos.x, currentMouseSlotPos.y);
            UpdateSynergyPreview(currentMouseSlotPos.x, currentMouseSlotPos.y);
        }

        Debug.Log($"[PickUp] {selectedItem.data.itemName} 집음");
    }

    // 아이템 배치 로직
    private void PlaceItemOnGrid(int x, int y)
    {
        bool canPlace = inventoryGrid.CheckPosition(x, y, selectedItem.Width, selectedItem.Height);

        if (canPlace)
        {
            // 데이터 배치
            inventoryGrid.PlaceItem(selectedItem, x, y);
            selectedItem.SetRaycastTarget(true);

            // 시각적 위치 재설정
            selectedItemRect.SetParent(inventoryGrid.GetComponent<RectTransform>());

            // 중앙 앵커(Center)로 강제 통일
            selectedItemRect.anchorMin = new Vector2(0.5f, 0.5f);
            selectedItemRect.anchorMax = new Vector2(0.5f, 0.5f);
            selectedItemRect.pivot = new Vector2(0.5f, 0.5f);
            selectedItemRect.localScale = Vector3.one;

            selectedItem = null;
            ClearHighlight();
            ClearSynergyPreview();
            inventoryGrid.RecalculateAllStats();
            Debug.Log("아이템 배치 성공!");
        }
        else
        {
            Debug.Log("여기엔 놓을 수 없습니다.");

            // 이번 리팩토링에서는 실패 시 그냥 들고 있도록 유지하는 게 '바닥 놓기'와 호환성이 좋음
        }
    }
    #endregion

    #region [Visuals] 시각 효과 (하이라이트, 시너지, 회전)
    // 마우스가 슬롯에 들어옴
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

    // 마우스가 슬롯에서 나감
    public void OnExitSlot(int x, int y)
    {
        // 나간 슬롯이 현재 슬롯과 같다면 초기화
        if (x == currentMouseSlotPos.x && y == currentMouseSlotPos.y)
        {
            currentMouseSlotPos = new Vector2Int(-1, -1);
            ClearHighlight();
            ClearSynergyPreview();  // 시너지 미리보기 끄기

            // 툴팁 끄기 안전장치
            uiTooltip.HideTooltip();
        }
    }

    // 아이템 회전
    private void RotateItem()
    {
        selectedItem.Rotate();

        if (currentMouseSlotPos.x != -1 && currentMouseSlotPos.y != -1)
        {
            UpdateHighlight(currentMouseSlotPos.x, currentMouseSlotPos.y);
        }
    }

    // 시너지 미리보기 계산
    private void UpdateSynergyPreview(int tempX, int tempY)
    {
        // 1. 기존에 켜져있던 미리보기 끄기
        ClearSynergyPreview();

        // 데이터나 효과가 없으면 패스
        if (selectedItem.data.effects == null) return;

        // 2. 들고 있는 아이템의 모든 효과를 돌면서 타겟 탐색
        foreach (var effect in selectedItem.data.effects)
        {
            // 'Passive' 효과만 미리보기 (OnClick 등은 미리 보여줄 필요 없음)
            if (effect.triggerType == EffectTriggerType.Passive)
            {
                List<InventoryItem> targets = effect.GetTargetItems(selectedItem, inventoryGrid, tempX, tempY);

                // 찾은 타겟들의 하이라이트 켜기
                foreach (var target in targets)
                {
                    // 이미 켜진 애가 아니면 켜기
                    if (!currentPreviewTargets.Contains(target))
                    {
                        target.SetHighlight(true); // InventoryItem에 구현한 함수 호출
                        currentPreviewTargets.Add(target); // 명단에 추가
                    }
                }
            }
        }
    }

    // 미리보기 끄는 함수
    private void ClearSynergyPreview()
    {
        foreach (var item in currentPreviewTargets)
        {
            if (item != null)
            {
                item.SetHighlight(false); // 끄기
            }
        }
        currentPreviewTargets.Clear();
    }

    // 배치 가능 여부에 따른 색상 업데이트
    private void UpdateHighlight(int startX, int startY)
    {
        // 1. 기존 하이라이트 싹 지우기 (잔상 방지)
        ClearHighlight();

        // 2. 배치 가능 여부 판단 -> 색상 결정
        bool isValid = inventoryGrid.CheckPosition(startX, startY, selectedItem.Width, selectedItem.Height);
        Color highlightColor = isValid ? validColor : invalidColor;

        // 시너지 미리보기도 같이 갱신
        if (isValid)
        {
            UpdateSynergyPreview(startX, startY); // 가능할 때만 미리보기 계산
        }
        else
        {
            ClearSynergyPreview(); // 불가능하면 싹 끄기
        }

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
    #endregion
}

