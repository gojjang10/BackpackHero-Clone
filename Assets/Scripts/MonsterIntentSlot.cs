using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIntentSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public Image iconImage;           // 칼, 방패 등 아이콘
    public TextMeshProUGUI valueText; // 10, 5 등 수치

    // 외부에서 데이터를 받아서 UI 갱신
    public void SetIntent(Sprite sprite, int value, MonsterMoveType type)
    {
        // 1. 아이콘 설정
        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        // 2. 수치 표시 (대기/버프는 숫자가 필요 없음)
        if (type == MonsterMoveType.Wait)
        {
            valueText.text = ""; // 빈칸
        }
        else
        {
            valueText.text = value.ToString();
        }

        // 3. 슬롯 켜기
        gameObject.SetActive(true);
    }

    // 슬롯 끄기 (초기화용)
    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
