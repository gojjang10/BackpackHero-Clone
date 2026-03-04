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

    //  몬스터 본체(Monster.cs)가 이 함수를 호출해서 자기 이미지를 넘겨줍니다.
    public void Setup(Sprite monsterSprite)
    {
        // 1. [섬광 설정] 몬스터 이미지 복사 & 하얗게 만들기
        if (flashRenderer != null)
        {
            flashRenderer.sprite = monsterSprite; // 몬스터 모양 복사!
            flashMat = flashRenderer.material;

            // 쉐이더 값 초기화
            flashMat.SetFloat("_HitEffectBlend", 1f);

            // 유령의 투명도 초기화 
            Color c = flashRenderer.color;
            c.a = 1f;
            flashRenderer.color = c;
        }

        // 2. [파동 설정]
        if (rippleRenderer != null)
        {
            rippleMat = rippleRenderer.material;
            rippleMat.SetFloat("_RippleProgress", 0f); // 중심에서 시작
            rippleMat.SetFloat("_RippleIntensity", 0.8f); // 밝기
        }

        // 3. 애니메이션 시작
        StartCoroutine(PlayEffectRoutine());
    }

    private IEnumerator PlayEffectRoutine()
    {
        float duration = 0.5f; // 연출 시간 
        float time = 0f;

        while (time < duration)
        {
            float progress = time / duration;

            // ① 파동: 퍼지면서 투명해짐
            if (rippleMat != null)
            {
                rippleMat.SetFloat("_RippleProgress", progress);
                // 링의 밝기가 서서히 줄어듦
                rippleMat.SetFloat("_RippleIntensity", 0.8f * (1f - progress));
            }

            // ② 섬광 유령: 하얀 상태 그대로 투명해지며 사라짐
            if (flashRenderer != null)
            {
                // SpriteRenderer의 자체 Alpha 값을 줄여서 투명하게 만듭니다.
                Color color = flashRenderer.color;
                color.a = 1f - progress;
                flashRenderer.color = color;
            }

            time += Time.deltaTime;
            yield return null;
        }

        // 끝났으니 퇴장
        Destroy(gameObject);
    }
}
