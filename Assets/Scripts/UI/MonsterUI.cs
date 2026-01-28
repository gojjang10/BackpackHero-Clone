using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    [Header("HP")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    public void UpdateHP(int currentHp, int maxHp)
    {
        hpSlider.value = (float)currentHp / maxHp;
        hpText.text = $"{currentHp} / {maxHp}";
    }
}
