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
}
