using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 그리드 탐색 관련 전역 함수들을 모아둔 static 클래스
public static class GridSearcher
{
    // 특정 시작 좌표에서 주어진 패턴에 따라 유효한 타겟 좌표들을 반환하는 전역 함수
    // * 불규칙한 고정 범위(예: 대각선, L자 모양 등)를 탐색할 때 사용

    /// <summary>
    /// 기준 좌표에서 주어진 패턴에 따라 유효한 좌표들을 반환합니다. (예 : 특정 좌표, 대각선, L자 모양 등)
    /// </summary>
    /// <param name="startX">아이템의 시작 X 좌표 (왼쪽 기준)</param>
    /// <param name="startY">아이템의 시작 Y 좌표 (아래 기준)</param>
    /// <param name="pattern">검사하고 싶은 패턴 </param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <returns>유효한 좌표들이 담긴 리스트 (Vector2Int)</returns>
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

    /// <summary>
    /// 기준 좌표에서 오른쪽 방향으로 지정된 범위만큼의 유효한 좌표들을 반환합니다.
    /// </summary>
    /// <param name="startX">아이템의 시작 X 좌표 (왼쪽 기준)</param>
    /// <param name="startY">아이템의 시작 Y 좌표 (아래 기준)</param>
    /// <param name="width">아이템의 가로 길이 (회전 고려됨)</param>
    /// <param name="height">아이템의 세로 길이 (회전 고려됨)</param>
    /// <param name="range">탐색할 범위 (몇 칸까지 검사할지)</param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <returns>유효한 좌표들이 담긴 리스트 (Vector2Int)</returns>
    public static List<Vector2Int> GetRightCoords(int startX, int startY, int width, int height, int range, InventoryGrid grid)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        // 시작점: 아이템의 오른쪽 끝 (startX + width)
        // 범위: range만큼 오른쪽으로 감
        for (int r = 0; r < range; r++)
        {
            int targetX = startX + width + r;
            for (int y = 0; y < height; y++) // 아이템 높이만큼 위로 훑음
            {
                int targetY = startY + y;
                if (grid.IsValidCoordinate(targetX, targetY))
                    coords.Add(new Vector2Int(targetX, targetY));
            }
        }
        return coords;
    }

    /// <summary>
    /// 기준 좌표에서 왼쪽 방향으로 지정된 범위만큼의 유효한 좌표들을 반환합니다.
    /// </summary>
    /// <param name="startX">아이템의 시작 X 좌표 (왼쪽 기준)</param>
    /// <param name="startY">아이템의 시작 Y 좌표 (아래 기준)</param>
    /// <param name="width">아이템의 가로 길이 (회전 고려됨)</param>
    /// <param name="height">아이템의 세로 길이 (회전 고려됨)</param>
    /// <param name="range">탐색할 범위 (몇 칸까지 검사할지)</param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <returns>유효한 좌표들이 담긴 리스트 (Vector2Int)</returns>
    public static List<Vector2Int> GetLeftCoords(int startX, int startY, int width, int height, int range, InventoryGrid grid)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        // 시작점: 아이템의 왼쪽 바로 옆 (startX - 1)
        for (int r = 0; r < range; r++)
        {
            int targetX = startX - 1 - r;
            for (int y = 0; y < height; y++)
            {
                int targetY = startY + y;
                if (grid.IsValidCoordinate(targetX, targetY))
                    coords.Add(new Vector2Int(targetX, targetY));
            }
        }
        return coords;
    }

    /// <summary>
    /// 기준 좌표에서 위쪽 방향으로 지정된 범위만큼의 유효한 좌표들을 반환합니다.
    /// </summary>
    /// <param name="startX">아이템의 시작 X 좌표 (왼쪽 기준)</param>
    /// <param name="startY">아이템의 시작 Y 좌표 (아래 기준)</param>
    /// <param name="width">아이템의 가로 길이 (회전 고려됨)</param>
    /// <param name="height">아이템의 세로 길이 (회전 고려됨)</param>
    /// <param name="range">탐색할 범위 (몇 칸까지 검사할지)</param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <returns>유효한 좌표들이 담긴 리스트 (Vector2Int)</returns>
    public static List<Vector2Int> GetUpCoords(int startX, int startY, int width, int height, int range, InventoryGrid grid)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        // 시작점: 아이템의 위쪽 끝 (startY + height)
        for (int r = 0; r < range; r++)
        {
            int targetY = startY + height + r;
            for (int x = 0; x < width; x++) // 아이템 너비만큼 옆으로 훑음
            {
                int targetX = startX + x;
                if (grid.IsValidCoordinate(targetX, targetY))
                    coords.Add(new Vector2Int(targetX, targetY));
            }
        }
        return coords;
    }

    /// <summary>
    /// 기준 좌표에서 아래쪽 방향으로 지정된 범위만큼의 유효한 좌표들을 반환합니다.
    /// </summary>
    /// <param name="startX">아이템의 시작 X 좌표 (왼쪽 기준)</param>
    /// <param name="startY">아이템의 시작 Y 좌표 (아래 기준)</param>
    /// <param name="width">아이템의 가로 길이 (회전 고려됨)</param>
    /// <param name="height">아이템의 세로 길이 (회전 고려됨)</param>
    /// <param name="range">탐색할 범위 (몇 칸까지 검사할지)</param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <returns>유효한 좌표들이 담긴 리스트 (Vector2Int)</returns>
    public static List<Vector2Int> GetDownCoords(int startX, int startY, int width, int height, int range, InventoryGrid grid)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        // 시작점: 아이템의 아래쪽 바로 밑 (startY - 1)
        for (int r = 0; r < range; r++)
        {
            int targetY = startY - 1 - r;
            for (int x = 0; x < width; x++)
            {
                int targetX = startX + x;
                if (grid.IsValidCoordinate(targetX, targetY))
                    coords.Add(new Vector2Int(targetX, targetY));
            }
        }
        return coords;
    }
}

