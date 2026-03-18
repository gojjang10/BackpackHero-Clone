using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("설정 파일")]

    public List<MapConfig> stageConfigs;    // 단일 변수 대신 리스트로 변경
    private MapConfig currentConfig;        // 현재 생성 중인 맵의 설정 (내부 로직용)
    public MapConfig CurrentConfig => currentConfig; // 외부에서 현재 설정을 읽을 수 있게 하는 프로퍼티
    public Transform nodeParent;   // 노드들이 생성될 부모 오브젝트
    public GameObject nodePrefab;  // 화면에 보여줄 동그라미 프리팹
    public GameObject linePrefab;  // 노드 연결선 프리팹

    [Header("플레이어 아이콘")]
    public GameObject playerIconPrefab; //  플레이어 아이콘 프리팹 연결할 변수
    private GameObject playerIconInstance; // 생성된 아이콘을 들고 있을 변수

    [Header("배치 설정")]
    public float nodeSpacing = 1.5f; // 노드 간격

    [Header("플레이어 아이콘 이동 연출")]
    public float iconMoveSpeed = 5f; // 초당 이동 속도 (숫자가 클수록 빠름)
    public bool IsIconMoving { get; private set; } // 상태 잠금용 프로퍼티

    // 생성된 맵 데이터를 저장할 딕셔너리 (좌표 -> 노드데이터)
    // public으로 열어서 다른 매니저가 볼 수 있게 함
    public Dictionary<Vector2Int, MapNode> mapGrid = new Dictionary<Vector2Int, MapNode>();
    // 맵 시각화된 오브젝트를 관리할 딕셔너리 (좌표 -> 노드 시각 오브젝트)
    public Dictionary<Vector2Int, MapNodeVisual> visualGrid = new Dictionary<Vector2Int, MapNodeVisual>();

    // 맵 생성 전 레시피(Config)만 먼저 세팅하는 함수
    public void LoadConfig()
    {
        if (stageConfigs == null || stageConfigs.Count == 0)
        {
            Debug.LogError("MapGenerator: 설정 파일 리스트(stageConfigs)가 비어있습니다!");
            return;
        }

        int floorIndex = 0;
        if (GameManager.instance != null)
        {
            floorIndex = GameManager.instance.currentFloor - 1;
        }

        if (floorIndex >= stageConfigs.Count)
        {
            floorIndex = stageConfigs.Count - 1;
        }

        currentConfig = stageConfigs[floorIndex];
        Debug.Log($"[MapGenerator] {floorIndex + 1}층 데이터 로드 완료 (설정: {currentConfig.name})");
    }

    // 맵 생성의 메인 함수
    public void GenerateMap()
    {
        // [안전 장치] 데이터가 없으면 맵 생성 중단
        if (currentConfig == null)
        {
            Debug.LogError("맵을 생성하기 전에 LoadConfig()를 먼저 호출해야 합니다!");
            return;
        }

        // 1. 청소하기 (기존 데이터 및 오브젝트 삭제)
        mapGrid.Clear();
        visualGrid.Clear(); //  추가: 비주얼 딕셔너리도 맵 생성 시마다 비워줌
        foreach (Transform child in nodeParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 시작점과 끝점 정하기 (config -> currentConfig 로 변경)
        int centerY = currentConfig.gridHeight / 2;
        Vector2Int startPos = new Vector2Int(0, centerY);
        Vector2Int endPos = new Vector2Int(currentConfig.gridWidth - 1, centerY);

        // 3. 워커(길 뚫는 로직) 실행
        for (int i = 0; i < currentConfig.maxPathCount; i++)
        {
            CreatePath(startPos, endPos);
        }

        // 4. 생성된 노드들의 타입을 결정하고 화면에 그리기
        foreach (var node in mapGrid.Values)
        {
            // (1) 타입 결정
            if (node.coordinate == startPos)
            {
                node.nodeType = NodeType.Neutral;
                node.isAccessible = true;
            }
            else if (node.coordinate == endPos)
            {
                // 마지막 층은 보스방, 그 외에는 다음 계단으로 설정
                node.nodeType = currentConfig.isBossStage ? NodeType.Boss : NodeType.NextStair;
            }
            else
            {
                node.nodeType = GetRandomType();
            }

            // (2) 화면에 그리기
            SpawnNodeObject(node);
        }

        DrawLines();

        // 데이터상으로도 시작점 설정
        if (mapGrid.ContainsKey(startPos))
        {
            MapNode startNode = mapGrid[startPos];
            SpawnPlayerIcon(startNode);

            if (StageManager.Instance != null)
            {
                StageManager.Instance.SetCurrentNode(startNode);
            }
        }

    }

    // 아이콘 생성 및 초기 위치 설정
    private void SpawnPlayerIcon(MapNode startNode)
    {
        if (playerIconInstance != null) Destroy(playerIconInstance);
        playerIconInstance = Instantiate(playerIconPrefab, nodeParent);
        UpdatePlayerIconPosition(startNode);
    }

    public void UpdatePlayerIconPosition(MapNode targetNode)
    {
        if (playerIconInstance == null) return;
        Vector3 targetPos = GetNodeLocalPosition(targetNode.coordinate);
        playerIconInstance.transform.localPosition = targetPos;
    }

    // 길 뚫기 알고리즘 
    private void CreatePath(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;
        AddNodeData(current);

        while (current.x < end.x)
        {
            List<Vector2Int> candidates = new List<Vector2Int>();

            // 1. 오른쪽
            candidates.Add(current + Vector2Int.right);

            // 2. 위/아래 (확률 체크 시 currentConfig 사용)
            if (Random.value < currentConfig.branchProbability)
            {
                if (current.y < currentConfig.gridHeight - 1) candidates.Add(current + Vector2Int.up);
                if (current.y > 0) candidates.Add(current + Vector2Int.down);
            }

            Vector2Int next = candidates[Random.Range(0, candidates.Count)];
            AddNodeData(next);

            MapNode currentNode = mapGrid[current];
            MapNode nextNode = mapGrid[next];

            if (!currentNode.nextNodes.Contains(next)) currentNode.nextNodes.Add(next);
            if (!nextNode.nextNodes.Contains(current)) nextNode.nextNodes.Add(current);

            current = next;
        }

        if (current != end)
        {
            while (current.y != end.y)
            {
                Vector2Int next = current;

                // 보스방이 내 위치보다 위에 있으면 위로, 아래에 있으면 아래로 한 칸 이동
                if (current.y < end.y) next.y += 1;
                else next.y -= 1;

                AddNodeData(next);
                MapNode currentNode = mapGrid[current];
                MapNode nextNode = mapGrid[next];

                // 양방향으로 안전하게 연결 (이제 시각적으로 선이 겹치는 착시가 사라집니다)
                if (!currentNode.nextNodes.Contains(next)) currentNode.nextNodes.Add(next);
                if (!nextNode.nextNodes.Contains(current)) nextNode.nextNodes.Add(current);

                current = next;
            }
        }
    }

    private void AddNodeData(Vector2Int coord)
    {
        if (!mapGrid.ContainsKey(coord))
        {
            mapGrid.Add(coord, new MapNode(coord, NodeType.Battle));
        }
    }

    // 랜덤 타입 뽑기 (currentConfig 사용)
    private NodeType GetRandomType()
    {
        int total = currentConfig.neutralWeight + currentConfig.battleWeight + currentConfig.shopWeight;
        int random = Random.Range(0, total);

        if (random < currentConfig.neutralWeight) return NodeType.Neutral;
        random -= currentConfig.neutralWeight;

        if (random < currentConfig.battleWeight) return NodeType.Battle;

        return NodeType.Shop;
    }

    // 화면에 동그라미 생성
    private void SpawnNodeObject(MapNode node)
    {
        // 맵 크기 계산 시 currentConfig 사용
        float mapWidth = (currentConfig.gridWidth - 1) * nodeSpacing;
        float mapHeight = (currentConfig.gridHeight - 1) * nodeSpacing;

        Vector3 centerOffset = new Vector3(mapWidth / 2f, mapHeight / 2f, 0);
        Vector3 localPos = new Vector3(node.coordinate.x * nodeSpacing, node.coordinate.y * nodeSpacing, 0);
        Vector3 finalPos = localPos - centerOffset;

        GameObject go = Instantiate(nodePrefab, nodeParent);
        go.transform.localPosition = finalPos;
        go.name = $"Node ({node.coordinate.x}, {node.coordinate.y})";

        MapNodeVisual visual = go.GetComponent<MapNodeVisual>();
        visual.Setup(node);
        
        // 생성된 시각 오브젝트를 딕셔너리에 저장 (추후 업데이트용)
        visualGrid.Add(node.coordinate, visual);
    }

    private void DrawLines()
    {
        foreach (var node in mapGrid.Values)
        {
            foreach (var nextNodeCoord in node.nextNodes)
            {
                if (mapGrid.ContainsKey(nextNodeCoord))
                {
                    MapNode targetNode = mapGrid[nextNodeCoord];
                    Vector3 startPos = GetNodeLocalPosition(node.coordinate);
                    Vector3 endPos = GetNodeLocalPosition(targetNode.coordinate);

                    GameObject lineObj = Instantiate(linePrefab, nodeParent);
                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                    lr.useWorldSpace = false;
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                    lr.positionCount = 2;
                    lr.SetPosition(0, startPos);
                    lr.SetPosition(1, endPos);
                }
            }
        }
    }

    // 로컬 좌표 계산 (currentConfig 사용)
    private Vector3 GetNodeLocalPosition(Vector2Int coord)
    {
        float mapWidth = (currentConfig.gridWidth - 1) * nodeSpacing;
        float mapHeight = (currentConfig.gridHeight - 1) * nodeSpacing;
        Vector3 centerOffset = new Vector3(mapWidth / 2f, mapHeight / 2f, 0);

        return new Vector3(coord.x * nodeSpacing, coord.y * nodeSpacing, 0) - centerOffset;
    }

    // 맵을 따라 구불구불 이동시키는 함수
    // System.Action onComplete : "도착하면 이 함수를 실행해줘!" 라는 뜻의 콜백입니다.
    public void MovePlayerIconAlongPath(List<MapNode> path, System.Action onComplete)
    {
        if (playerIconInstance == null || path == null || path.Count == 0) return;
        StartCoroutine(MoveAlongPathRoutine(path, onComplete));
    }

    // 여러 노드를 차례대로 방문하는 코루틴
    private IEnumerator MoveAlongPathRoutine(List<MapNode> path, System.Action onComplete)
    {
        IsIconMoving = true;

        // 리스트에 담긴 노드들을 순서대로 하나씩 꺼내서 이동합니다.
        foreach (MapNode node in path)
        {
            Vector3 startPos = playerIconInstance.transform.localPosition;
            Vector3 targetPos = GetNodeLocalPosition(node.coordinate);

            float distance = Vector3.Distance(startPos, targetPos);
            float duration = distance / iconMoveSpeed; // 시간 = 거리 / 속력
            float elapsed = 0f;

            if (duration > 0f)
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    // t는 0에서 1로 천천히 증가하는 값입니다. (0%에서 100%로)
                    float t = elapsed / duration;
                    // Lerp 함수를 사용해서 시작점에서 목표점까지 부드럽게 이동합니다.
                    playerIconInstance.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                    yield return null;
                }
            }
            // 오차 교정
            playerIconInstance.transform.localPosition = targetPos;
        }

        IsIconMoving = false;

        // 이동이 완전히 끝났으니, StageManager에게 "도착했어!"라고 알려줍니다.
        onComplete?.Invoke();
    }

    // 외부에서 특정 좌표의 노드 색상을 업데이트하라고 시킬 때 쓰는 함수
    public void UpdateNodeColor(Vector2Int coord)
    {
        if (visualGrid.ContainsKey(coord))
        {
            visualGrid[coord].UpdateVisualState();
        }
    }
}
