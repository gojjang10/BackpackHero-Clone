using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    [Header("자식 오브젝트 연결")]
    public SpriteRenderer rippleRenderer; // 자식 1: 파동 링
    public SpriteRenderer flashRenderer;  // 자식 2: 섬광 유령

    private Material rippleMat;
    private Material flashMat;

    //  Sprite 대신 원본 렌더러를 통째로 받고, 색상(Nullable)도 받습니다.
    public void Setup(SpriteRenderer originalRenderer, Color? inputColor = null)
    {
        // 1. [섬광 설정] 원본 렌더러의 모든 속성(좌우반전 등)을 완벽하게 복사
        if (flashRenderer != null && originalRenderer != null)
        {
            flashRenderer.sprite = originalRenderer.sprite;
            flashRenderer.flipX = originalRenderer.flipX; // 좌우 반전 복사
            flashRenderer.flipY = originalRenderer.flipY; // 상하 반전 복사
            flashRenderer.drawMode = originalRenderer.drawMode; // Tiled/Simple 모드 복사
            flashRenderer.size = originalRenderer.size;

            flashMat = flashRenderer.material;
            flashMat.SetFloat("_HitEffectBlend", 1f); // 하얗게

            Color c = flashRenderer.color;
            c.a = 1f;
            flashRenderer.color = c;

            // 크기 동기화 (혹시 모를 스케일 문제 방지)
            flashRenderer.transform.localScale = Vector3.one;
        }

        // 2. [파동 설정] 색상 적용하기
        if (rippleRenderer != null)
        {
            rippleMat = rippleRenderer.material;

            // 밖에서 색깔을 넣어줬으면 그 색으로 변경
            if (inputColor.HasValue)
            {
                rippleMat.SetColor("_RippleColor", inputColor.Value);
            }
            // (입력 안 했으면 셰이더 기본값 사용)

            rippleMat.SetFloat("_RippleProgress", 0f);
            rippleMat.SetFloat("_RippleIntensity", 0.8f);
        }

        // 3. 애니메이션 시작
        StartCoroutine(PlayEffectRoutine());
    }

    private IEnumerator PlayEffectRoutine()
    {
        float duration = 0.4f; // 연출 시간 (살짝 줄여도 좋습니다)
        float time = 0f;

        while (time < duration)
        {
            float progress = time / duration;

            // ① 파동: 퍼지면서 투명해짐
            if (rippleMat != null)
            {
                rippleMat.SetFloat("_RippleProgress", progress);
                rippleMat.SetFloat("_RippleIntensity", 0.8f * (1f - progress));
            }

            // ② 섬광 유령: 투명해지며 사라짐
            if (flashRenderer != null)
            {
                Color color = flashRenderer.color;
                color.a = 1f - progress;
                flashRenderer.color = color;
            }

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
