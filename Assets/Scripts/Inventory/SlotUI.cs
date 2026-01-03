using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int x, y;
    private GridInteract gridInteract; // 매니저 참조

    private Image backgroundImage;  // 배경 이미지 컴포넌트

    [Header("하이라이트 연결")]
    public Image highlightImage;    // 하이라이트 이미지 컴포넌트

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

    // 클릭되었을 때 호출되는 이벤트 함수 (유니티 엔진이 자동 호출)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gridInteract != null)
        {
            // 매니저에게 "나(x,y) 클릭됐어!" 라고 보고
            gridInteract.OnClickSlot(x, y);
        }
    }

    // 마우스가 인벤토리 그리드 안에 들어오는 타이밍에 호출되는 이벤트 함수 (유니티 엔진이 자동 호출)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gridInteract != null) gridInteract.OnEnterSlot(x, y);
    }

    // 마우스가 인벤토리 그리드 밖으로 나가는 타이밍에 호출되는 이벤트 함수 (유니티 엔진이 자동 호출)
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gridInteract != null) gridInteract.OnExitSlot(x, y);
    }

    // 기본 잠금 상태 설정
    public void SetUnlockState(bool isUnlocked)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }

    // 하이라이트 켜기/끄기 함수
    public void SetHighlight(bool isOn, Color color)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(isOn); // 껐다 켰다
            highlightImage.color = color; // 색상 변경 (초록/빨강)
        }
    }
}
