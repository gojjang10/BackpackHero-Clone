using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("상단 정보 UI")]
    public TextMeshProUGUI energyText;   // "Energy: 3/3"
    public TextMeshProUGUI turnInfoText; // "나의 턴"

    [Header("결과 패널 UI")]
    public GameObject endGamePanel;      // 검은 배경 패널
    public TextMeshProUGUI resultText;   // VICTORY / GAME OVER

    // 행동력 텍스트 갱신
    public void UpdateEnergy(int current, int max)
    {
        if (energyText != null)
            energyText.text = $"Energy: {current} / {max}";
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
