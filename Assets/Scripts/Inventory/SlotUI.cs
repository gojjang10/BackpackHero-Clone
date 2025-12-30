using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public int x, y;

    private Image backgroundImage;

    [Header("색상 설정")]
    public Color unlockedColor = Color.white;       // 해금됨 (흰색)
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 잠김 (어두운 회색, 반투명)

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    // 좌표 설정
    public void SetCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // 잠금 상태에 따라 색상 변경
    public void SetLockedState(bool isLocked)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isLocked ? lockedColor : unlockedColor;
        }
    }
}
