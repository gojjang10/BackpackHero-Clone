using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 인스턴스

    [Header("현재 게임 상태")]
    public GameState currentState = GameState.Exploration; // 기본은 탐험 모드

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

    public void RelocationInventory()
    {
        currentState = (currentState == GameState.Exploration) ? GameState.Battle : GameState.Exploration;
        Debug.Log($"[GameManager] 상태 변경: {currentState}");
    }

    public void ExplorationMode()
    {
        currentState = GameState.Exploration;
        Debug.Log($"[GameManager] 상태 변경: {currentState}");
    }

    public void BattleMode()
    {
        currentState = GameState.Battle;
        Debug.Log($"[GameManager] 상태 변경: {currentState}");
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
