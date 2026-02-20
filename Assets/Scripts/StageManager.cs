using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 싱글톤 구현
    // To do: 필요 시 제거
    public static StageManager Instance;

    [Header("UI 연결")]
    public GameObject battlePanel; // 전투 UI 패널
    public GameObject shopPanel;   // 상점 UI 패널
    public GameObject rewardPanel; // 보상 UI 패널
    public GameObject mapPanel;       // 맵 전체 부모 (MapPanelArea)
    public GameObject inventoryPanel; // 인벤토리 전체 부모

    [Header("아이템 청소를 위한 게임 오브젝트")]
    public Transform worldItemHolder;

    [Header("맵 이동 상태")]
    public MapNode currentNode; // ★ 현재 플레이어가 있는 노드 데이터

    [Header("시스템 연결")]
    public GridInteract gridInteract; // 인스펙터에서 연결

    // 맵 제너레이터 참조 (아이콘 옮기라고 시켜야 하니까)
    public MapGenerator mapGenerator;


    private void Awake()
    {
        Instance = this;    // 싱글톤 할당
    }

    private void Start()
    {
        ShowMap(); // 게임 시작 시 맵부터 보여주기
    }

    // 맵을 켜고 나머지는 끄는 함수
    public void ShowMap()
    {
        battlePanel.SetActive(false);
        shopPanel.SetActive(false);
        rewardPanel.SetActive(false);

        mapPanel.SetActive(true); // 맵 켜기

        CleanUpWorldItems(); // 필드 아이템 정리
    }

    // 노드 타입을 받아서 스테이지 전환
    public void EnterStage(NodeType type)
    {

        if (GameManager.instance.currentState == GameState.Battle)
        {
            Debug.Log(" 전투 중에는 이동할 수 없습니다 (전투를 먼저 끝내세요)");
            return; // 함수 강제 종료
        }

        Debug.Log($"스테이지 진입 시도: {type}");

        // 1. 맵 끄기 (이제 해당 방으로 들어감)
        mapPanel.SetActive(false);

        // 2. 기존 패널 초기화
        battlePanel.SetActive(false);
        shopPanel.SetActive(false);
        rewardPanel.SetActive(false);
        CleanUpWorldItems();

        // 3. 타입별 분기 처리
        switch (type)
        {
            case NodeType.Battle:

                if (currentNode.isCleared)
                {
                    Debug.Log($" 이미 정복한 지역입니다. ({currentNode.coordinate}) - 전투 스킵");
                }
                else
                {
                    battlePanel.SetActive(true);
                    OpenInventory();
                    // 전투 매니저가 있다면 시작
                    if (BattleManager.instance != null) BattleManager.instance.StartBattle();
                }

                break;

            case NodeType.Shop:
                shopPanel.SetActive(true);
                OpenInventory();
                break;

            case NodeType.Neutral: // 이벤트 or 빈 방
                break;

            case NodeType.NextStair:
                Debug.Log(">>> 계단을 발견했습니다! 다음 층으로 이동합니다.");

                // 1. GameManager의 층수(Floor) 증가
                // (이 변수가 바뀌어야 MapGenerator가 다음 단계 설정을 가져옵니다)
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentFloor++;
                    Debug.Log($"=== {GameManager.instance.currentFloor}층 진입 ===");
                }

                // 2. 맵 재생성 요청
                // (MapGenerator.GenerateMap() 안에서 기존 맵 삭제 -> 새 설정 로드 -> 새 맵 생성 -> 플레이어 위치 초기화 수행)
                if (mapGenerator != null)
                {
                    mapGenerator.GenerateMap();
                }

                // 3. 맵 화면 활성화
                ShowMap();
                break;
        }
    }

    // 보관소 자식들을 전멸시키는 함수
    private void CleanUpWorldItems()
    {
        if (worldItemHolder == null) return;

        // 자식이 몇 명인지 세고 다 지움
        foreach (Transform child in worldItemHolder)
        {
            Destroy(child.gameObject);
        }
    }

    // 인벤토리를 켜주는 헬퍼 함수
    private void OpenInventory()
    {
        if (StageManager.Instance.inventoryPanel != null)
        {
            StageManager.Instance.inventoryPanel.SetActive(true);
        }
    }

    // 맵과 인벤토리 토글 함수 
    public void ToggleMapState()
    {
        // 아이템을 들고 있는지 검사 (GridInteract를 통해 간접 확인)
        if (gridInteract != null && gridInteract.IsDraggingItem)
        {
            Debug.Log(" 아이템을 정리하고 지도를 펼쳐주세요!");
            return;
        }

        // 확장 포인트가 남아있는지 검사 (GridInteract를 통해 간접 확인)
        if (gridInteract != null && gridInteract.HasPendingExpansion)
        {
            Debug.Log(" 가방 확장을 완료해야 맵을 펼칠 수 있습니다!");
            return; // 함수 강제 종료 
        }

        // 맵이 꺼져있다면? -> 맵을 켜고 인벤토리를 끈다.
        bool isMapOpening = !mapPanel.activeSelf;

        mapPanel.SetActive(isMapOpening);
        inventoryPanel.SetActive(!isMapOpening);
    }

    // 초기화 함수 (MapGenerator가 맵 다 만들고 호출해줌)
    public void SetCurrentNode(MapNode node)
    {
        currentNode = node;
        Debug.Log($"현재 위치 초기화: {node.coordinate}");
    }

    // 이동 시도 함수
    public void TryMoveToNode(MapNode targetNode)
    {
        // 1. 현재 전투 중인지 확인 
        if (GameManager.instance.currentState == GameState.Battle)
        {
            Debug.Log(" 전투 중에는 이동할 수 없습니다! (전투를 먼저 끝내세요)");
            return;
        }

        // 2. 같은 노드 클릭 방지
        if (currentNode == targetNode) return;

        // 3. 길 찾기 (연결 여부 + 클리어 여부)
        if (IsPathAvailable(currentNode, targetNode))
        {
            // ★ 모든 검문을 통과했을 때만 실행!
            MoveToNode(targetNode);
        }
        else
        {
            Debug.Log("지나갈 수 없는 길입니다.");
        }
    }

    // 실제 이동 처리
    private void MoveToNode(MapNode targetNode)
    {
        Debug.Log($"이동: {currentNode.coordinate} -> {targetNode.coordinate}");

        // 1. 데이터 갱신
        currentNode = targetNode;

        // 2. 비주얼 갱신 (아이콘 이동)
        if (mapGenerator != null)
        {
            mapGenerator.UpdatePlayerIconPosition(targetNode);
        }

        // 3. 스테이지 진입 (기존 로직 연결)
        EnterStage(targetNode.nodeType);
    }

    // ★ BFS 길 찾기 알고리즘
    // 시작점에서 목표점까지, '클리어된 방'들만 밟아서 갈 수 있니?
    private bool IsPathAvailable(MapNode start, MapNode target)
    {
        // 방문 체크용 (무한 루프 방지)
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<MapNode> queue = new Queue<MapNode>();

        queue.Enqueue(start);
        visited.Add(start.coordinate);

        while (queue.Count > 0)
        {
            MapNode current = queue.Dequeue();

            // 목표 발견 -> 가는 길 있음
            if (current == target) return true;

            // 지금 검사하는 방이 '목표'가 아닌데 '안 깬 방'이라면? 여기서 길 막힘.
            if (!current.isCleared && current != start)
            {
                continue; // 더 이상 이쪽 길로는 못 감
            }

            // 연결된 이웃 노드 탐색
            foreach (Vector2Int neighborPos in current.nextNodes)
            {
                // 아직 방문 안 했으면 큐에 넣기
                if (!visited.Contains(neighborPos))
                {
                    // (주의: 매니저가 맵 데이터를 알고 있어야 함)
                    // MapGenerator에서 mapGrid를 가져오거나, StageManager가 들고 있어야 함
                    if (mapGenerator.mapGrid.ContainsKey(neighborPos))
                    {
                        visited.Add(neighborPos);
                        queue.Enqueue(mapGenerator.mapGrid[neighborPos]);
                    }
                }
            }
        }

        return false; // 길 못 찾음
    }
}
