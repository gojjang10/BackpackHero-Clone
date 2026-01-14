using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 인스턴스

    [Header("현재 게임 상태")]
    public GameState currentState = GameState.Exploration; // 기본은 탐험 모드

    private void Awake()
    {
        // 싱글톤 패턴 적용 (어디서든 GameManager.instance로 접근 가능하게)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 파괴 안 됨
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 상태 변경 테스트용 
    private void Update()
    {
        // T 키를 누르면 전투 <-> 탐험 모드 전환 (테스트용)
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentState = (currentState == GameState.Exploration) ? GameState.Battle : GameState.Exploration;
            Debug.Log($"[GameManager] 상태 변경: {currentState}");
        }
    }
}
