using System.Collections;
using System.Collections.Generic;
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
}
