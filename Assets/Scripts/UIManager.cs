using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 어디서든 부르기 쉽게 싱글톤

    [Header("UI 패널 연결")]
    public GameObject mapPanel;
    public GameObject inventoryPanel;

    [Header("UI 슬라이드 연출")]
    public RectTransform inventoryPanelRect;
    public RectTransform mapPanelRect;
    public float slideDuration = 0.35f;

    [Header("씬 트랜지션 연출")]
    public CanvasGroup transitionGroup; // TransitionPanel의 CanvasGroup
    public TextMeshProUGUI floorNameText; // 층 이름 텍스트

    [Header("시스템 경고 UI")]
    public CanvasGroup warningGroup;      // 방금 만든 SystemWarningPanel
    public TextMeshProUGUI warningText;   // 그 안의 텍스트
    private Coroutine warningCoroutine;   // 현재 실행 중인 코루틴 (중복 실행 방지용)

    // 외부에서 애니메이션 중인지 확인할 수 있게 프로퍼티로 선언 (읽기만 가능)
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // 게임 시작 시 초기 위치 셋업
    public void SetupInitialState()
    {
        mapPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        if (mapPanelRect != null) mapPanelRect.anchoredPosition = Vector2.zero;
        if (inventoryPanelRect != null) inventoryPanelRect.anchoredPosition = new Vector2(-1920f, 0f);
    }

    // 애니메이션 없이 즉시 맵을 화면 중앙에 띄움
    public void SnapToMap()
    {
        IsTransitioning = false;
        mapPanel.SetActive(true);
        inventoryPanel.SetActive(false);

        if (mapPanelRect != null) mapPanelRect.anchoredPosition = Vector2.zero;
        if (inventoryPanelRect != null) inventoryPanelRect.anchoredPosition = new Vector2(-1920f, 0f);
    }

    // 애니메이션 없이 즉시 인벤토리를 화면 중앙에 띄움
    public void SnapToInventory()
    {
        IsTransitioning = false;
        mapPanel.SetActive(false);
        inventoryPanel.SetActive(true);

        if (mapPanelRect != null) mapPanelRect.anchoredPosition = new Vector2(1920f, 0f);
        if (inventoryPanelRect != null) inventoryPanelRect.anchoredPosition = Vector2.zero;
    }

    // 외부(StageManager 등)에서 애니메이션을 명령할 때 쓰는 함수
    public void SlideMapAndInventory(bool openMap)
    {
        if (IsTransitioning) return;
        StartCoroutine(SlidePanelsRoutine(openMap));
    }

    // 맵과 인벤토리 패널을 슬라이드로 전환하는 코루틴
    private IEnumerator SlidePanelsRoutine(bool openMap)
    {
        IsTransitioning = true;

        mapPanel.SetActive(true);
        inventoryPanel.SetActive(true);

        Vector2 invStartPos = inventoryPanelRect.anchoredPosition;
        Vector2 mapStartPos = mapPanelRect.anchoredPosition;

        Vector2 invTargetPos = openMap ? new Vector2(-1920f, 0f) : Vector2.zero;
        Vector2 mapTargetPos = openMap ? Vector2.zero : new Vector2(1920f, 0f);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);

            inventoryPanelRect.anchoredPosition = Vector2.Lerp(invStartPos, invTargetPos, t);
            mapPanelRect.anchoredPosition = Vector2.Lerp(mapStartPos, mapTargetPos, t);

            yield return null;
        }

        inventoryPanelRect.anchoredPosition = invTargetPos;
        mapPanelRect.anchoredPosition = mapTargetPos;

        if (openMap) inventoryPanel.SetActive(false);
        else mapPanel.SetActive(false);

        IsTransitioning = false;
    }

    // 트랜지션 메인 함수
    // 텍스트 내용과, 화면이 완전히 까매졌을 때 몰래 실행할 함수(콜백)를 받습니다.
    public void DoFloorTransition(string text, Action onMidpointCallback)
    {
        if (transitionGroup == null) return;
        StartCoroutine(TransitionRoutine(text, onMidpointCallback));
    }

    private IEnumerator TransitionRoutine(string text, Action onMidpointCallback)
    {
        // 1. 방어 코드 (트랜지션 중에는 클릭을 막기 위해 Raycast를 켬)
        IsTransitioning = true;
        transitionGroup.blocksRaycasts = true;
        floorNameText.text = text;

        // 2. Fade Out (화면 까매지기)
        float duration = 1.0f; // 1초 동안 어두워짐
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transitionGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        transitionGroup.alpha = 1f;

        // 3. 완벽한 암전 상태 여기서 맵을 바꿉니다 (콜백 실행)
        onMidpointCallback?.Invoke();

        // 4. 텍스트를 읽을 수 있게 1.5초 정도 대기
        yield return new WaitForSeconds(1.5f);

        // 5. Fade In (화면 밝아지기)
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transitionGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        transitionGroup.alpha = 0f;

        // 6. 연출 끝 다시 마우스 클릭이 되도록 Raycast를 끔
        transitionGroup.blocksRaycasts = false;
    }

    // 외부에서 "UIManager.Instance.ShowWarning("골드가 부족!");" 이렇게 부를 함수
    public void ShowWarning(string message)
    {
        if (warningGroup == null || warningText == null) return;

        // 만약 이미 팝업이 떠서 코루틴이 돌고 있다면? 
        // 하던 걸 강제로 멈추고 새로운 메시지로 덮어씌웁니다. (광클 대비)
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }

        // 새 코루틴 시작!
        warningCoroutine = StartCoroutine(WarningRoutine(message));
    }

    private IEnumerator WarningRoutine(string message)
    {
        // 1. 텍스트 세팅 및 초기화
        warningText.text = message;
        warningGroup.alpha = 0f;
        warningGroup.gameObject.SetActive(true);

        float fadeDuration = 0.2f; // 0.2초 만에 빠르게 등장
        float elapsed = 0f;

        // 2. Fade In (등장)
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            warningGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        warningGroup.alpha = 1f;

        // 3. 1초 동안 대기 (유저가 글씨를 읽을 시간)
        yield return new WaitForSeconds(1.0f);

        // 4. Fade Out (서서히 사라짐)
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            warningGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        warningGroup.alpha = 0f;
        warningGroup.gameObject.SetActive(false); // 완전히 꺼줌
    }
}
