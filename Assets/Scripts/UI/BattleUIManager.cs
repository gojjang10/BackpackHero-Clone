using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("전투 UI")]
    public GameObject battleUIPanel; // 모든 전투 UI


    [Header("상단 정보 UI")]
    public TextMeshProUGUI turnInfoText; // "나의 턴"

    [Header("결과 패널 UI")]
    public GameObject endGamePanel;      // 검은 배경 패널
    public TextMeshProUGUI resultText;   // VICTORY / GAME OVER

    public void OnEnable()
    {
        if (battleUIPanel != null)
            battleUIPanel.SetActive(true);
    }

    public void OnDisable()
    {
        if (battleUIPanel != null)
            battleUIPanel.SetActive(false);
    }

    // 턴 정보 텍스트 갱신
    public void UpdateTurnText(string text)
    {
        if (turnInfoText != null)
            turnInfoText.text = text;
    }

    // 승리 화면 켜기
    public void ShowWinUI()
    {
        if (endGamePanel != null) endGamePanel.SetActive(true);
        if (resultText != null) resultText.text = "VICTORY!";
    }

    // 패배 화면 켜기
    public void ShowLoseUI()
    {
        if (endGamePanel != null) endGamePanel.SetActive(true);
        if (resultText != null) resultText.text = "GAME OVER...";
    }
}
