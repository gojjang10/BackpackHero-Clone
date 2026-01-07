using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상/하/좌/우 방향 기반 탐색 전략
[CreateAssetMenu(menuName = "Inventory/Strategy/Directional Strategy")]
public class DirectionalStrategy : SearchStrategy
{
    [Header("범위 설정")]
    public SearchDirection direction; // 방향 설정
    [Min(1)] public int range = 1;    // 몇 칸까지 확인할것인지 (기본 1)

    public override List<Vector2Int> GetCoords(InventoryItem source, InventoryGrid grid, int originX, int originY)
    {
        // 1. 아이템의 현재 크기 (회전 고려됨)
        int w = source.Width;
        int h = source.Height;

        // 2. 방향에 따라 GridSearcher의 다른 계산 함수 호출
        switch (direction)
        {
            case SearchDirection.RightSide:
                return GridSearcher.GetRightCoords(originX, originY, w, h, range, grid);

            case SearchDirection.LeftSide:
                return GridSearcher.GetLeftCoords(originX, originY, w, h, range, grid);

            case SearchDirection.UpSide:
                return GridSearcher.GetUpCoords(originX, originY, w, h, range, grid);

            case SearchDirection.DownSide:
                return GridSearcher.GetDownCoords(originX, originY, w, h, range, grid);

            default:
                return new List<Vector2Int>();
        }
    }
}
