using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 인스턴스

    [Header("현재 게임 상태")]
    public GameState currentState = GameState.StartReward; // 기본은 탐험 모드
    public int currentFloor = 1; // 현재 층수 (1층부터 시작)

    [Header("시스템 연결")]
    public ItemSpawner itemSpawner;       // 아이템 생성기
    public GameObject rewardUIObject;     // 보상 패널 UI
    public GameObject mapUIObject;        // 맵 UI 패널 (처음엔 숨기기 위함)

    private BattleUIManager battleUIManager;

    private void Awake()
    {
        // 싱글톤 패턴 적용 (어디서든 GameManager.instance로 접근 가능하게)
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // 씬 넘어가도 파괴 안 됨
        }
    }

    private void Start()
    {
        // ★ 1. 게임 시작 시 오프닝(시작 보상) 실행
        OpeningRoutine();
    }

    private void OpeningRoutine()
    {

        SetState(GameState.StartReward);

        // 맵 가리기, 보상 창 띄우기
        if (mapUIObject != null) mapUIObject.SetActive(false);
        if (rewardUIObject != null) rewardUIObject.SetActive(true);

        // 시작 아이템 3개 스폰 (개수는 마음대로 조절)
        if (itemSpawner != null)
        {
            itemSpawner.SpawnRewardItems(3);
            Debug.Log("시작 보상이 지급되었습니다. 아이템을 가방에 넣으세요!");
        }
    }

    //// ★ 2. [모험 시작] 버튼에 연결할 함수
    //public void OnStartAdventureClicked()
    //{
    //    // 보상 창 닫고, 맵 열기
    //    if (rewardUIObject != null) rewardUIObject.SetActive(false);
    //    if (mapUIObject != null) mapUIObject.SetActive(true);

    //    // 상태를 탐험 모드로 변경
    //    SetState(GameState.Exploration);

    //    Debug.Log("모험을 시작합니다!");
    //}

    // 상태 변경 함수
    public void SetState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"게임 상태 변경: {currentState}");

    }

    // [타이틀로] 버튼에 연결할 함수
    public void OnClickToTitle()
    {
        //Time.timeScale = 1f; 추후에 게임을 정지시켰을시에 다시 활성화 시켜주는 코드
        SceneManager.LoadScene("TitleScene"); // 
    }

    public void InitBattleUIManager(BattleUIManager uiManager)
    {
        battleUIManager = uiManager;
    }

    public void OnGameClear()
    {
        battleUIManager.ShowWinUI();
    }
}
