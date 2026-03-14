using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [Header("연결")]
    public RawImage backgroundImage; // 흘러갈 배경 이미지

    [Header("설정")]
    public float scrollSpeed = 0.5f; // 흘러가는 속도

    // 현재 돌고 있는 코루틴을 기억해둘 변수 (나중에 끄기 위해 필요함)
    private Coroutine scrollCoroutine;

    // 외부에서 호출해서 스크롤을 시작하는 함수 (예: 맵 보여줄 때)
    public void StartScrolling()
    {
        // 혹시 이미 돌고 있는 코루틴이 있다면 안전하게 끄고 시작 (중복 실행 방지)
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
        }

        // 코루틴 실행
        scrollCoroutine = StartCoroutine(ScrollRoutine());
    }
    // 외부에서 호출해서 스크롤을 멈추는 함수 (예: 전투 시작 시)
    public void StopScrolling()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null; // 메모리 비우기
        }
    }

    // 실제 이미지를 밀어주는 코루틴 
    private IEnumerator ScrollRoutine()
    {
        if (backgroundImage == null) yield break; // 예외 처리

        // StopCoroutine이 호출되기 전까지 무한 루프
        while (true)
        {
            Rect currentUV = backgroundImage.uvRect;
            currentUV.x += scrollSpeed * Time.deltaTime;
            backgroundImage.uvRect = currentUV;

            yield return null; // 다음 프레임까지 대기
        }
    }
}
