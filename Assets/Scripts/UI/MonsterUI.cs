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

    [Header("Intent 시스템 (신규)")]
    public GameObject intentPanel;          // 슬롯들의 부모 (Horizontal Layout Group)
    public MonsterIntentSlot[] intentSlots; // 미리 만들어둔 슬롯들 (3개)

    [Header("Intent 아이콘 리소스")]
    public Sprite attackIcon;
    public Sprite defendIcon;
    public Sprite buffIcon;
    public Sprite debuffIcon;
    public Sprite waitIcon;

    public void UpdateHP(int currentHp, int maxHp)
    {
        hpSlider.value = (float)currentHp / maxHp;
        hpText.text = $"{currentHp} / {maxHp}";
    }

    // ★ Intent UI 업데이트 함수 (신규)
    public void UpdateIntentUI(Queue<MonsterIntent> intentQueue)
    {
        // 1. 일단 모든 슬롯 끄기 (초기화)
        if (intentSlots != null)
        {
            foreach (var slot in intentSlots)
            {
                if (slot != null) slot.Clear();
            }
        }

        if (intentQueue == null || intentQueue.Count == 0) return;

        // 2. 큐 내용물 확인 (배열로 복사)
        MonsterIntent[] intents = intentQueue.ToArray();

        // 3. 앞에서부터 차례대로 채워넣기
        for (int i = 0; i < intents.Length; i++)
        {
            // 슬롯 개수보다 행동이 많으면 중단 (예외 처리)
            if (i >= intentSlots.Length) break;

            MonsterIntent intent = intents[i];
            Sprite icon = GetIconByType(intent.type);

            // 슬롯에 데이터 주입하고 켜기
            if (intentSlots[i] != null)
            {
                intentSlots[i].SetIntent(icon, intent.value, intent.type);
            }
        }
    }

    // 타입에 맞는 아이콘 반환
    private Sprite GetIconByType(MonsterMoveType type)
    {
        switch (type)
        {
            case MonsterMoveType.Attack: return attackIcon;
            case MonsterMoveType.Defend: return defendIcon;
            case MonsterMoveType.Buff: return buffIcon;
            case MonsterMoveType.Debuff: return debuffIcon;
            default: return waitIcon;
        }
    }
}
