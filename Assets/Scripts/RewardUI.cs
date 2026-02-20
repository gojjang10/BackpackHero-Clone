using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [Header("시스템 연결")]
    public Player player;

    [Header("UI 메시지")]
    public TextMeshProUGUI infoText;

    private void OnEnable()
    {
        if (player != null)
        {
            // 1. 플레이어의 포인트 변경 이벤트에 내 함수를 등록 (구독)
            player.OnExpandPointsChanged += UpdateInfoText;

            // 2. 패널이 처음 켜졌을 때 현재 상태로 한 번 업데이트
            UpdateInfoText(player.expandPoints);
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            // 패널이 꺼지거나 삭제될 때 구독을 해지(메모리 누수 방지)
            player.OnExpandPointsChanged -= UpdateInfoText;
        }
    }

    // 포인트가 변경될 때마다(또는 OnEnable 때) 이 함수가 한 번만 딱 실행됩니다.
    private void UpdateInfoText(int currentPoints)
    {
        if (infoText == null) return;

        if (currentPoints > 0)
        {
            infoText.text = $"<color=red>가방을 확장하세요! (남은 포인트: {currentPoints})</color>";
        }
        else
        {
            infoText.text = "보상을 챙기고 맵(M)을 열어 모험을 계속하세요.";
        }
    }
}
