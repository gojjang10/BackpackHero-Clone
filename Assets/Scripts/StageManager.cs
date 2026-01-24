using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 싱글톤 구현
    // To do: 필요 시 제거
    public static StageManager Instance;

    [Header("스테이지 순서 설정")]
    public List<StageType> stageList;   // 인스펙터에서 [Battle, Shop, Battle] 순서로 넣기 (To do : 임시 순서로 넣어놓은 것이기 때문에 추후에 리팩토링 염두)
    public int currentStageIndex = 0;   // 현재 스테이지 인덱스

    [Header("UI 연결")]
    public GameObject battlePanel; // 전투 UI 패널
    public GameObject shopPanel;   // 상점 UI 패널

    private void Awake()
    {
        Instance = this;    // 싱글톤 할당
    }

    private void Start()
    {
        LoadCurrentStage();
    }

    // 현재 스테이지 로드
    public void LoadCurrentStage()
    {
        // 1. 인덱스 초과 체크
        // To do : 우선은 직선 순회 형태로 구현, 추후에 스테이지 선택 시스템 등으로 확장 필요
        if (currentStageIndex >= stageList.Count)
        {
            Debug.Log("모든 스테이지 클리어! 게임 종료.");
            return;
        }

        StageType currentType = stageList[currentStageIndex];
        Debug.Log($"스테이지 시작: {currentType}");

        // 2. 패널 초기화 (다 끄고 시작)
        battlePanel.SetActive(false);
        shopPanel.SetActive(false);

        // 3. 타입에 맞는 패널 켜기
        switch (currentType)
        {
            case StageType.Battle:
                battlePanel.SetActive(true);
                // To do : 여기에 BattleManager.StartBattle() 같은거 나중에 추가
                break;
            case StageType.Shop:
                shopPanel.SetActive(true);
                break;
        }
    }

    // 다음 스테이지로 이동
    public void NextStage()
    {
        currentStageIndex++;
        LoadCurrentStage();
    }
}
