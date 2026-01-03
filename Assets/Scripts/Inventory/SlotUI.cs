using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerDownHandler
{
    public int x, y;

    private Image backgroundImage;
    private GridInteract gridInteract; // 매니저 참조

    [Header("색상 설정")]
    public Color unlockedColor = Color.white;       // 해금됨 (흰색)
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 잠김 (어두운 회색, 반투명)

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        gridInteract = FindObjectOfType<GridInteract>();
    }

    // 좌표 설정
    public void SetCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // 잠금 상태에 따라 색상 변경
    public void SetLockedState(bool isUnlocked)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }

    // ★ 클릭되었을 때 호출되는 함수 (유니티 엔진이 자동 호출)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gridInteract != null)
        {
            // 매니저에게 "나(x,y) 클릭됐어!" 라고 보고
            gridInteract.OnClickSlot(x, y);
        }
    }
}
