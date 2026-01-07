using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SearchStrategy : ScriptableObject
{
    // 어떤 아이템(source)을 기준으로, 어디에서 탐색할 건지 좌표 반환
    public abstract List<Vector2Int> GetCoords(InventoryItem source, InventoryGrid grid, int originX, int originY);
}
