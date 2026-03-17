using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 50f;    // 위로 올라가는 속도
    public float fadeDuration = 1f;  // 사라지는데 걸리는 시간

    public void Show(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;

        StartCoroutine(AnimateRoutine());
    }


    // 위로 올라가는 연출
    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Color startColor = textMesh.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < fadeDuration)
        {
            // 1. 위로 스르륵 이동
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 2. 색상이 서서히 투명해짐
            textMesh.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. 연출이 다 끝나면 심플하게 파괴
        Destroy(gameObject);

        // 우선 오브젝트 풀링이 필요없다라고 판단해서 그냥 Destroy로 처리했지만, 나중에 성능 최적화가 필요하다면 오브젝트 풀링으로 변경을 고려합니다.
    }
}
