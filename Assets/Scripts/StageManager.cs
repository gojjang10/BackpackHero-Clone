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
                battlePanel.SetActive(true);
                OpenInventory();
                // 전투 매니저가 있다면 시작
                if (BattleManager.instance != null) BattleManager.instance.StartBattle();

                break;

            case NodeType.Shop:
                shopPanel.SetActive(true);
                OpenInventory();
                break;

            case NodeType.Neutral: // 이벤트 or 빈 방
                break;

            case NodeType.NextStair:
                Debug.Log(">>> 다음 층으로 이동 로직 실행! (맵 재생성 등)");
                // 여기선 나중에 GameManager.NextFloor() 같은 걸 호출하면 됨
                ShowMap(); // 임시로 다시 맵 보여주기
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

    // 노드 클릭 시 호출될 함수 (이동 시도)
    public void TryMoveToNode(MapNode targetNode)
    {
        // 1. 이미 같은 곳을 클릭했다면? (무시)
        if (currentNode == targetNode) return;

        // 2. 연결된 노드인지 확인 (Logic Check)
        // 현재 노드의 nextNodes 리스트에 타겟 좌표가 들어있는가?
        if (currentNode.nextNodes.Contains(targetNode.coordinate))
        {
            // [이동 승인!] 
            MoveToNode(targetNode);
        }
        else
        {
            // [이동 거절]
            Debug.Log("갈 수 없는 길입니다! (연결되지 않음)");
            // 여기에 "길이 없어요" 같은 UI 메시지를 띄울 수도 있음
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
}
