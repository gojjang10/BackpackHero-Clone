using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("HP")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    [Header("Stats")]
    public GameObject blockGroup; // 방패 아이콘+텍스트의 부모 오브젝트
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI energyText;

    [Header("Level & EXP")] 
    public TextMeshProUGUI levelText; // "Lv.1" 표시용
    public Slider expSlider;          // 경험치 게이지
    public TextMeshProUGUI expText;   // "3 / 10" 표시용

    public void UpdateHP(int currentHp, int maxHp)
    {
        hpSlider.value = (float)currentHp / maxHp;
        hpText.text = $"{currentHp} / {maxHp}";
    }

    public void UpdateStats(int armor, int energy)
    {
        energyText.text = $"{energy}";

        // 방어도가 있을 때만 방패 아이콘 표시
        if (blockGroup != null)
        {
            if (armor > 0)
            {
                blockGroup.SetActive(true);
                if (armorText != null) armorText.text = $"{armor}";
            }
            else
            {
                blockGroup.SetActive(false); // 0이면 숨김
            }
        }
    }

    // 경험치 및 레벨 업데이트 함수
    public void UpdateExp(int currentExp, int maxExp, int level)
    {
        // 1. 레벨 텍스트 갱신
        if (levelText != null)
        {
            levelText.text = $"Lv.{level}";
        }

        // 2. 슬라이더 갱신
        if (expSlider != null)
        {
            // 0으로 나누기 에러 방지 (maxExp가 0일 경우 대비)
            float ratio = (maxExp > 0) ? (float)currentExp / maxExp : 0;
            expSlider.value = ratio;
        }

        // 3. 수치 텍스트 갱신
        if (expText != null)
        {
            expText.text = $"{currentExp} / {maxExp}";
        }
    }
}
