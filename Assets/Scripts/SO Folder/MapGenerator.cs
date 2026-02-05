using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("설정 파일")]
    public MapConfig config;       // 아까 만든 설정 파일 연결
    public Transform nodeParent;   // 노드들이 생성될 부모 오브젝트
    public GameObject nodePrefab;  // 화면에 보여줄 동그라미 프리팹
    public GameObject linePrefab;  // 노드 연결선 프리팹

    [Header("배치 설정")]
    public float nodeSpacing = 1.5f; // 노드 간격

    // 생성된 맵 데이터를 저장할 딕셔너리 (좌표 -> 노드데이터)
    // public으로 열어서 다른 매니저가 볼 수 있게 함
    public Dictionary<Vector2Int, MapNode> mapGrid = new Dictionary<Vector2Int, MapNode>();

    private void Start()
    {
        GenerateMap();
    }

    // 맵 생성의 메인 함수
    public void GenerateMap()
    {
        // 1. 청소하기 (기존 데이터 및 오브젝트 삭제)
        mapGrid.Clear();
        foreach (Transform child in nodeParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 시작점과 끝점 정하기 (왼쪽 중앙 -> 오른쪽 중앙)
        int centerY = config.gridHeight / 2;
        Vector2Int startPos = new Vector2Int(0, centerY);
        Vector2Int endPos = new Vector2Int(config.gridWidth - 1, centerY);

        // 3. 워커(길 뚫는 로직) 실행
        // 설정된 횟수만큼 길을 뚫습니다.
        for (int i = 0; i < config.maxPathCount; i++)
        {
            CreatePath(startPos, endPos);
        }

        // 4. 생성된 노드들의 타입을 결정하고 화면에 그리기
        foreach (var node in mapGrid.Values)
        {
            // (1) 타입 결정
            if (node.coordinate == startPos)
            {
                node.nodeType = NodeType.Neutral; // 시작점은 이벤트(또는 빈방)
                node.isAccessible = true;       // 시작점은 바로 갈 수 있음
            }
            else if (node.coordinate == endPos)
            {
                node.nodeType = NodeType.NextStair; // 아니면 다음 층 계단
            }
            else
            {
                node.nodeType = GetRandomType(); // 나머지는 랜덤
            }

            // (2) 화면에 그리기 (Instantiate)
            SpawnNodeObject(node);
        }

        DrawLines();    // 모든 노드 연결선 그리기
    }

    // 길 뚫기 알고리즘 
    private void CreatePath(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;
        AddNodeData(current); // 시작점 데이터 생성

        // 끝점의 X좌표에 도달할 때까지 계속 오른쪽으로 이동
        while (current.x < end.x)
        {
            // 다음 어디로 갈지 후보 리스트
            List<Vector2Int> candidates = new List<Vector2Int>();

            // 1. 오른쪽(전진)은 무조건 후보
            candidates.Add(current + Vector2Int.right);

            // 2. 위/아래(갈림길)는 확률적으로 추가
            if (Random.value < config.branchProbability)
            {
                // 맵 밖으로 안 나가는지 체크 후 추가
                if (current.y < config.gridHeight - 1) candidates.Add(current + Vector2Int.up);
                if (current.y > 0) candidates.Add(current + Vector2Int.down);
            }

            // 후보 중 하나 랜덤 선택
            Vector2Int next = candidates[Random.Range(0, candidates.Count)];

            // 데이터 생성 및 연결
            AddNodeData(next);

            // 현재 노드와 다음 노드를 연결 (Graph 구조 형성)
            MapNode currentNode = mapGrid[current];
            if (!currentNode.nextNodes.Contains(next))
            {
                currentNode.nextNodes.Add(next);
            }

            // 이동
            current = next;
        }

        // 마지막 위치가 끝점이 아닐 수도 있으니, 끝점과 강제 연결
        if (current != end)
        {
            AddNodeData(end);
            MapNode lastNode = mapGrid[current];
            if (!lastNode.nextNodes.Contains(end))
            {
                lastNode.nextNodes.Add(end);
            }
        }
    }

    // 딕셔너리에 노드 데이터 추가 (중복 방지)
    private void AddNodeData(Vector2Int coord)
    {
        if (!mapGrid.ContainsKey(coord))
        {
            // 일단 타입은 Battle로 해두고 나중에 GenerateMap에서 덮어씌움
            mapGrid.Add(coord, new MapNode(coord, NodeType.Battle));
        }
    }

    // 랜덤 타입 뽑기 (심플 버전)
    private NodeType GetRandomType()
    {
        int total = config.neutralWeight + config.battleWeight + config.shopWeight;
        int random = Random.Range(0, total);

        if (random < config.neutralWeight) return NodeType.Neutral;
        random -= config.neutralWeight;

        if (random < config.battleWeight) return NodeType.Battle;

        return NodeType.Shop;
    }

    // 화면에 동그라미 생성
    private void SpawnNodeObject(MapNode node)
    {
        // 1. 전체 맵의 크기 계산 (중앙 정렬을 위해)
        float mapWidth = (config.gridWidth - 1) * nodeSpacing;
        float mapHeight = (config.gridHeight - 1) * nodeSpacing;

        // 2. 중앙 보정값 (Offset) 계산
        // (너비의 절반, 높이의 절반만큼 왼쪽/아래로 당겨야 중앙에 옴)
        Vector3 centerOffset = new Vector3(mapWidth / 2f, mapHeight / 2f, 0);

        // 3. 현재 노드의 '로컬' 좌표 계산
        Vector3 localPos = new Vector3(node.coordinate.x * nodeSpacing, node.coordinate.y * nodeSpacing, 0);

        // 4. 최종 위치 = 로컬 좌표 - 중앙 보정값
        Vector3 finalPos = localPos - centerOffset;

        // 5. 생성 및 부모 설정
        // Instantiate 할 때 부모(nodeParent)를 바로 지정
        GameObject go = Instantiate(nodePrefab, nodeParent);

        // position(월드)이 아니라 localPosition(부모 기준)을 사용
        go.transform.localPosition = finalPos;

        // 이름 예쁘게 짓기
        go.name = $"Node ({node.coordinate.x}, {node.coordinate.y})";

        // 색깔 바꾸기 (타입별로)
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (node.nodeType == NodeType.Battle) sr.color = Color.red;
        else if (node.nodeType == NodeType.Shop) sr.color = Color.yellow;
        else if (node.nodeType == NodeType.Neutral) sr.color = Color.blue;
        else if (node.nodeType == NodeType.Boss) sr.color = Color.black;
        else if (node.nodeType == NodeType.NextStair) sr.color = Color.green;

        MapNodeVisual visual = go.GetComponent<MapNodeVisual>();

        visual.Setup(node); // 노드 데이터 연결
    }

    // 선 그리는 함수
    private void DrawLines()
    {
        foreach (var node in mapGrid.Values)
        {
            // 이 노드에서 뻗어나가는 모든 연결선 그리기
            foreach (var nextNodeCoord in node.nextNodes)
            {
                if (mapGrid.ContainsKey(nextNodeCoord))
                {
                    MapNode targetNode = mapGrid[nextNodeCoord];

                    // 1. 시작점과 끝점 계산 (로컬 좌표 기준)
                    Vector3 startPos = GetNodeLocalPosition(node.coordinate);
                    Vector3 endPos = GetNodeLocalPosition(targetNode.coordinate);

                    // 2. 프리팹 생성
                    GameObject lineObj = Instantiate(linePrefab, nodeParent);

                    // 3. 선 연결
                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();

                    lr.useWorldSpace = false;

                    // 선 두께가 너무 두꺼우면 여기서 강제로 줄여줍니다.
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;

                    lr.positionCount = 2;
                    lr.SetPosition(0, startPos);
                    lr.SetPosition(1, endPos);
                }
            }
        }
    }

    // 로컬 좌표 계산 함수
    private Vector3 GetNodeLocalPosition(Vector2Int coord)
    {
        float mapWidth = (config.gridWidth - 1) * nodeSpacing;
        float mapHeight = (config.gridHeight - 1) * nodeSpacing;
        Vector3 centerOffset = new Vector3(mapWidth / 2f, mapHeight / 2f, 0);

        return new Vector3(coord.x * nodeSpacing, coord.y * nodeSpacing, 0) - centerOffset;
    }
}
