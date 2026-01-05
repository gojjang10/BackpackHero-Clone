using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 그리드 탐색 관련 전역 함수들을 모아둔 static 클래스
public static class GridSearcher
{
    // 특정 시작 좌표에서 주어진 패턴에 따라 유효한 타겟 좌표들을 반환하는 전역 함수
    public static List<Vector2Int> GetValidCoordinates(int startX, int startY, List<Vector2Int> pattern, InventoryGrid grid)
    {
        // 유효한 타겟 좌표들을 담을 리스트
        List<Vector2Int> validTargets = new List<Vector2Int>(pattern.Count);

        // 매개변수로 받은 패턴을 순회하면서 유효한 좌표인지 체크
        foreach (Vector2Int offset in pattern)
        {
            int targetX = startX + offset.x;
            int targetY = startY + offset.y;

            // 그리드 밖으로 나가는지 체크 (InventoryGrid의 함수 재사용)
            if (grid.IsValidCoordinate(targetX, targetY))
            {
                // 유효한 좌표이면 리스트에 추가
                validTargets.Add(new Vector2Int(targetX, targetY));
            }
        }
        return validTargets;
    }
}
