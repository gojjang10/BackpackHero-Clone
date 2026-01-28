using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI statsText;

    [Header("배경 오브젝트")]
    public GameObject tooltipObject; // 툴팁 전체를 껐다 켰다 할 부모 오브젝트

    // 문자열 조합용 빌더 (최적화)
    private StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        // 시작하면 꺼두기
        HideTooltip();
    }

    public void ShowTooltip(InventoryItem item)
    {
        tooltipObject.SetActive(true);

        // 1. 이름 설정
        nameText.text = item.data.itemName;

        // 2. 설명 설정
        descText.text = item.data.description;

        // 3. 스탯 정보 조합 (StringBuilder 사용)
        sb.Clear(); // 내용 비우기

        // 태그 정보 추가
        sb.Append("<color=yellow>[");
        if (item.data.itemTags != null)
        {
            for (int i = 0; i < item.data.itemTags.Count; i++)
            {
                sb.Append(item.data.itemTags[i].ToString());
                if (i < item.data.itemTags.Count - 1) sb.Append(", ");
            }
        }
        sb.Append("]</color>\n\n"); // 줄바꿈 두 번

        // 공격력/방어력 정보 (0보다 클 때만 표시)
        if (item.currentAttack > 0)
            sb.Append($"공격력: <color=red>{item.currentAttack}</color>\n");

        if (item.currentBlock > 0)
            sb.Append($"방어력: <color=blue>{item.currentBlock}</color>\n");

        if (item.currentEnergyCost > 0)
            sb.Append($"행동력 소모량: <color=green>{item.currentEnergyCost}</color>\n");

        // 추후 마나 소모량 등도 여기서 추가하면 좋을까 생각중

        statsText.text = sb.ToString();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }

    // 툴팁이 마우스 따라다니게 하려면 Update에서 처리
    private void Update()
    {
        if (tooltipObject.activeSelf)
        {
            // 마우스 위치에서 살짝 오른쪽 아래에 표시
            transform.position = Input.mousePosition + new Vector3(15, -15, 0);
        }
    }
}
