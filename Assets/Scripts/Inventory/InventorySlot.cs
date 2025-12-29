using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // 인스펙터에서 보기 위해 직렬화
public class InventorySlot
{
    public int x; // 그리드 좌표 X
    public int y; // 그리드 좌표 Y
    public bool isUnlocked; // 슬롯 잠금 해제 여부
    public InventoryItem inventoryItem; // 현재 이 슬롯을 차지하고 있는 아이템 (없으면 null)

    // 생성자
    public InventorySlot(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.isUnlocked = false; // 기본적으로 잠금 상태
        this.inventoryItem = null;
    }

    // 빈 슬롯인지 확인하는 함수
    public bool IsEmpty()
    {
        // inventoryItem이 null이면 true 반환
        return inventoryItem == null;
    }
}
