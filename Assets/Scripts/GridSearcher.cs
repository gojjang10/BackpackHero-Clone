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

    /// <summary>
    /// BFS(너비 우선 탐색) 알고리즘을 사용하여, 특정 태그(예 : 전도체)를 가진 아이템을 타고 연결된 모든 좌표를 반환합니다.
    /// <para>
    /// * 시작 아이템의 크기(Width, Height)가 1x1보다 클 경우, 아이템이 차지하는 모든 칸을 시작점으로 잡고 탐색을 시작합니다.
    /// </para>
    /// </summary>
    /// <param name="startX">탐색을 시작할 기준 아이템의 X 좌표</param>
    /// <param name="startY">탐색을 시작할 기준 아이템의 Y 좌표</param>
    /// <param name="width">기준 아이템의 가로 크기 (회전 고려됨)</param>
    /// <param name="height">기준 아이템의 세로 크기 (회전 고려됨)</param>
    /// <param name="grid">검사할 인벤토리 그리드 참조</param>
    /// <param name="connectorTag">연결 통로로 인식할 아이템 태그 (예: Conductive)</param>
    /// <returns>연결된 모든 유효 좌표 리스트 (자기 자신은 포함되지 않음)</returns>
    public static List<Vector2Int> GetConnectedCoords(int startX, int startY, int width, int height, InventoryGrid grid, ItemTag connectorTag)
    {
        List<Vector2Int> visitedCoords = new List<Vector2Int>(); // 결과 리스트
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();     // 탐색 대기열
        HashSet<Vector2Int> enqueued = new HashSet<Vector2Int>(); // 중복 방지

        // 1. 탐색 방향
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 2. 시작 영역 설정 
        // 아이템이 차지하는 모든 칸(x, y)을 기준으로 잡아야 함
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int partPos = new Vector2Int(startX + x, startY + y);

                // 내 자신이 차지하는 자리는 '이미 방문함' 처리해서 결과에 포함 안 되게 막음
                enqueued.Add(partPos);

                // 내 몸통의 각 칸마다 상하좌우를 확인해서 큐에 넣음
                foreach (var dir in directions)
                {
                    Vector2Int neighbor = partPos + dir;

                    // 유효한 좌표이고 && 아직 방문/예약 안 했고 && 자신이 아니라면
                    if (grid.IsValidCoordinate(neighbor.x, neighbor.y) && !enqueued.Contains(neighbor))
                    {
                        toVisit.Enqueue(neighbor);
                        enqueued.Add(neighbor);
                    }
                }
            }
        }

        // 3. BFS 루프
        while (toVisit.Count > 0)
        {
            Vector2Int currentPos = toVisit.Dequeue();

            InventoryItem item = grid.GetItem(currentPos.x, currentPos.y);

            if (item != null)
            {
                // A. 아이템 발견. 결과에 추가
                visitedCoords.Add(currentPos);

                // B. 태그를 가진 아이템이라면 더 뻗어나감
                if (HasTag(item, connectorTag))
                {
                    foreach (var dir in directions)
                    {
                        Vector2Int nextPos = currentPos + dir;

                        if (grid.IsValidCoordinate(nextPos.x, nextPos.y) && !enqueued.Contains(nextPos))
                        {
                            toVisit.Enqueue(nextPos);
                            enqueued.Add(nextPos);
                        }
                    }
                }
            }
        }

        return visitedCoords;
    }

    // 아이템 태그 리스트에 특정 태그가 있는지 확인
    private static bool HasTag(InventoryItem item, ItemTag tag)
    {
        if (item.data.itemTags == null) return false;
        return item.data.itemTags.Contains(tag);
    }
}

