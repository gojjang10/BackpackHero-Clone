using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("현재 데이터 (읽기 전용)")]
    public BaseMonsterData data; // 현재 몬스터의 원본 데이터

    [Header("실시간 상태")]
    public int currentHp;
    public int currentBlock; // 방어도

    [Header("연결 요소")]
    public SpriteRenderer spriteRenderer; // 이미지를 바꿔주기 위해 필요
    public GameObject selectionMark;      // 선택 화살표
    public MonsterUI monsterUI;          // 몬스터 UI

    // 팩토리가 이 함수를 호출해서 몬스터를 세팅함 (초기화)
    public void Init(BaseMonsterData inputData)
    {
        // 1. 데이터 주입
        data = inputData;

        // 2. 스탯 초기화
        currentHp = data.maxHp;
        currentBlock = 0;

        // 3. 몬스터 이미지 설정 
        if (spriteRenderer != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }

        // 4. 이름 변경 (하이어라키 창에서 보기 편하게)
        gameObject.name = data.monsterName;

        // 5. UI 초기화
        monsterUI.UpdateHP(currentHp, data.maxHp);
        Debug.Log($" 몬스터 생성 완료: {data.monsterName} (HP: {currentHp})");
    }

    private void OnMouseDown()
    {
        // 배틀 매니저가 있고, 현재 전투 중이라면 타겟 등록 요청
        if (BattleManager.instance != null && GameManager.instance.currentState == GameState.Battle)
        {
            BattleManager.instance.SelectTarget(this);
        }
    }

    // 플레이어가 선택했을 시 하이라이트 표시 함수
    public void SetSelection(bool isSelected)
    {
        if (selectionMark != null)
        {
            selectionMark.SetActive(isSelected);
        }

    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        // UI 업데이트
        monsterUI.UpdateHP(currentHp, data.maxHp);
        Debug.Log($" {data.monsterName}에게 {damage} 데미지! (남은 HP: {currentHp}/{data.maxHp})");
    }

    // 매니저가 사망처리를 요청하는 함수
    public void OnDie()
    {
        Destroy(gameObject);
    }

    public IEnumerator AttackRoutine(Player targetPlayer)
    {
        // 1. 원래 크기 저장 
        Vector3 originalScale = transform.localScale;
        Vector3 attackScale = originalScale * 1.2f;

        // 2. 공격 연출 (커짐)
        float duration = 0.1f;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, attackScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = attackScale;

        yield return new WaitForSeconds(0.1f); // 타격감 텀

        // 3. 실제 데미지 적용
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(data.attackDamage);
        }

        // 4. 복구 연출 (원래 크기로 정확히 복구)
        transform.localScale = originalScale;
    }
}
