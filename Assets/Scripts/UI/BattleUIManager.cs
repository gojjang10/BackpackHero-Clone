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

    // 데미지 텍스트 생성 함수
    public void SpawnDamageText(int damage, Vector3 worldPosition, bool isPlayer)
    {
        if (damageTextPrefab == null) return;

        // 1. 심플하게 즉시 생성!
        GameObject textObj = Instantiate(damageTextPrefab, damageTextParent, false);
        RectTransform rectTransform = textObj.GetComponent<RectTransform>(); // UI니까 RectTransform 사용

        // 2. 월드 좌표(캐릭터) -> 스크린 좌표(픽셀)로 변환
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // 3. ★ 핵심: 스크린 좌표 -> UI 캔버스 내부의 로컬 좌표로 완벽하게 통역!
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            damageTextParent.GetComponent<RectTransform>(),
            screenPos,
            Camera.main,
            out Vector2 localPos
        );

        // 4. 머리 위로 살짝 올려주기
        localPos.y += 50f;

        // 5. transform.position(월드)이 아니라 anchoredPosition(UI)에 꽂아줍니다!
        rectTransform.anchoredPosition = localPos;

        // 6. 연출 슛!
        Color textColor = isPlayer ? Color.red : Color.white;
        string textStr = $"-{damage}";

        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        floatingText.Show(textStr, textColor); // 위치 넘겨주는 부분 삭제됨!
    }
}
