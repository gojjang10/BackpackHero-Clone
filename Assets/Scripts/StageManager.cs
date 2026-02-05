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

    // ★ 핵심 변경: 인덱스가 아니라 '노드 타입'을 받아서 스테이지 전환
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
}
