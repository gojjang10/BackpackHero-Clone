using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Strategy/Network Strategy (BFS)")]
public class NetworkSearchStrategy : SearchStrategy
{
    [Header("연결 통로 태그")]
    [Tooltip("이 태그를 가진 아이템을 통해 연결이 확장됩니다. (예: Conductive)")]
    public ItemTag connectorTag;

    public override List<Vector2Int> GetCoords(InventoryItem source, InventoryGrid grid, int originX, int originY)
    {
        int w = source.Width;
        int h = source.Height;

        // 내 위치(origin) 기준으로 'connectorTag'로 연결된 좌표들 반환
        return GridSearcher.GetConnectedCoords(originX, originY, w, h, grid, connectorTag);
    }
}
