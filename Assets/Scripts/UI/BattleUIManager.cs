using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("전투 UI")]
    public GameObject battleUIPanel; // 모든 전투 UI


    [Header("슬라이드 알림 UI (턴, 전투 시작, 승리)")]
    public RectTransform slideBanner;     // 날아다닐 배너 패널의 RectTransform
    public TextMeshProUGUI slideText;     // 배너 안의 텍스트
    private Coroutine slideCoroutine;     // 중복 실행 방지용

    [Header("결과 패널 UI")]
    public GameObject endGamePanel;      // 검은 배경 패널
    public TextMeshProUGUI resultText;   // VICTORY / GAME OVER

    [Header("데미지 텍스트")]
    public GameObject damageTextPrefab; // 아까 만든 프리팹 연결
    public Transform damageTextParent;  // 전투 캔버스 등 부모 연결

    private void Start()
    {
        GameManager.instance.InitBattleUIManager(this);
    }

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

    // 텍스트 슬라이드 연출 함수 
    public void ShowSlideNotification(string text)
    {
        if (slideBanner == null || slideText == null) return;

        // 기존에 날아가고 있던 알림이 있다면 강제 정지 후 새 알림 띄우기
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideRoutine(text));
    }

    // 부드러운 이동 코루틴
    private IEnumerator SlideRoutine(string text)
    {
        slideText.text = text;
        slideBanner.gameObject.SetActive(true);

        // 시작, 중앙, 끝 위치 설정 (화면 가로 1920 기준 넉넉하게 1500)
        Vector2 startPos = new Vector2(1500, 0);
        Vector2 centerPos = Vector2.zero;
        Vector2 endPos = new Vector2(-1500, 0);

        float slideSpeed = 0.15f; // 날아오는 속도
        float waitTime = 0.7f;   // 대기 시간

        // 1. 우측 -> 중앙 (Slide In)
        float elapsed = 0f;
        while (elapsed < slideSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideSpeed;
            float smoothT = t * t * (3f - 2f * t); // SmoothStep (부드러운 감속)
            slideBanner.anchoredPosition = Vector2.Lerp(startPos, centerPos, smoothT);
            yield return null;
        }
        slideBanner.anchoredPosition = centerPos;

        // 2. 중앙에서 대기
        yield return new WaitForSeconds(waitTime);

        // 3. 중앙 -> 좌측 (Slide Out)
        elapsed = 0f;
        while (elapsed < slideSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideSpeed;
            float smoothT = t * t * (3f - 2f * t);
            slideBanner.anchoredPosition = Vector2.Lerp(centerPos, endPos, smoothT);
            yield return null;
        }
        slideBanner.anchoredPosition = endPos;
        slideBanner.gameObject.SetActive(false); // 연출 끝나면 끄기
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

    // 데미지 텍스트 생성 함수
    public void SpawnDamageText(int damage, Vector3 worldPosition, bool isPlayer)
    {
        if (damageTextPrefab == null) return;

        // 1. 심플하게 즉시 생성
        GameObject textObj = Instantiate(damageTextPrefab, damageTextParent, false);
        RectTransform rectTransform = textObj.GetComponent<RectTransform>(); // UI니까 RectTransform 사용

        // 2. 월드 좌표(캐릭터) -> 스크린 좌표(픽셀)로 변환
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // 3. 스크린 좌표 -> UI 캔버스 내부의 로컬 좌표로
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            damageTextParent.GetComponent<RectTransform>(),
            screenPos,
            Camera.main,
            out Vector2 localPos
        );

        // 4. 머리 위로 살짝 올려주기
        localPos.y += 50f;

        // 5. anchoredPosition(UI)에 초기화
        rectTransform.anchoredPosition = localPos;

        // 6. 연출
        Color textColor = isPlayer ? Color.red : Color.white;
        string textStr = $"-{damage}";

        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        floatingText.Show(textStr, textColor); // 위치 넘겨주는 부분 삭제됨!
    }
}
