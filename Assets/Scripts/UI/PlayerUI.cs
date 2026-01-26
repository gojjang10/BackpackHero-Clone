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
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI energyText;

    public void UpdateHP(int currentHp, int maxHp)
    {
        hpSlider.value = (float)currentHp / maxHp;
        hpText.text = $"{currentHp} / {maxHp}";
    }

    public void UpdateStats(int armor, int energy)
    {
        armorText.text = $"{armor}";
        energyText.text = $"{energy}";
    }
}
