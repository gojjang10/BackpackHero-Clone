using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("현재 데이터 (읽기 전용)")]
    public BaseMonsterData data; // 현재 몬스터의 원본 데이터
    public MonsterPatternSO patternData; // AI 패턴 데이터 (없을 수도 있음)

    [Header("실시간 상태")]
    public int currentHp;
    public int currentBlock; // 방어도

    // 행동 관리용 큐
    private Queue<MonsterIntent> intentQueue = new Queue<MonsterIntent>();

    [Header("연결 요소")]
    public SpriteRenderer spriteRenderer; // 이미지를 바꿔주기 위해 필요
    public GameObject selectionMark;      // 선택 화살표
    public MonsterUI monsterUI;          // 몬스터 UI

    // 팩토리가 이 함수를 호출해서 몬스터를 세팅함 (초기화)
    // 초기화 함수 (Factory에서 호출)
    public void Init(BaseMonsterData inputData, MonsterPatternSO aiPattern = null)
    {
        data = inputData;
        patternData = aiPattern; // 패턴 주입 (없으면 null)

        currentHp = data.maxHp;
        currentBlock = 0;

        if (spriteRenderer != null && data.icon != null)
            spriteRenderer.sprite = data.icon;

        gameObject.name = data.monsterName;
        monsterUI.UpdateHP(currentHp, data.maxHp);
        monsterUI.UpdateIntentUI(null);

        // ★ 첫 턴 행동 계획 미리 수립
        PlanNextTurn();
    }

    // 1. [AI 계획] 다음 턴 행동 결정 (Player Turn 시작 시 호출)
    public void PlanNextTurn()
    {
        intentQueue.Clear(); // 큐 초기화

        // 패턴 데이터가 있으면 행동 뽑아오기
        if (patternData != null)
        {
            MonsterIntent intent = patternData.GetNextIntent();
            intentQueue.Enqueue(intent);
        }
        else
        {
            // 패턴이 없으면 대기행동
            intentQueue.Enqueue(new MonsterIntent(MonsterMoveType.Wait, 0));
        }

        if (monsterUI != null)
        {
            monsterUI.UpdateIntentUI(intentQueue);
        }

        // 디버깅용 로그
        if (intentQueue.Count > 0)
        {
            MonsterIntent next = intentQueue.Peek();
            Debug.Log($" [AI 계획] {name}: {next.type} (위력: {next.value})");
        }
    }


    // 2. [AI 실행] 큐에 있는 행동 수행 (Enemy Turn 시작 시 호출)
    public IEnumerator PerformTurnRoutine(Player player)
    {
        // 큐가 빌 때까지 하나씩 꺼내서 실행
        while (intentQueue.Count > 0)
        {
            MonsterIntent action = intentQueue.Dequeue(); // 값 타입 복사

            yield return StartCoroutine(ProcessAction(action, player));
            yield return new WaitForSeconds(0.5f); // 행동 간 딜레이
        }
    }

    // 개별 행동 처리 로직
    private IEnumerator ProcessAction(MonsterIntent action, Player player)
    {
        // 연출용 (커졌다 작아짐)
        Vector3 originalScale = transform.localScale;
        Vector3 actionScale = originalScale * 1.2f;

        float duration = 0.1f;
        float time = 0;

        // 커지는 애니메이션
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, actionScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = actionScale;

        // ★ 실제 로직 분기
        switch (action.type)
        {
            case MonsterMoveType.Attack:
                Debug.Log($" {name} 공격! 데미지 {action.value}");
                if (player != null) player.TakeDamage(action.value);
                break;

            case MonsterMoveType.Defend:
                Debug.Log($" {name} 방어! 방어도 +{action.value}");
                currentBlock += action.value;   // 방어도 작업 필요함
                // monsterUI.UpdateBlock(currentBlock); // UI 갱신 필요
                break;

            case MonsterMoveType.Wait:
                Debug.Log($" {name} 대기...");
                break;
        }

        yield return new WaitForSeconds(0.1f); // 타격감 텀

        // 원래 크기로 복구
        transform.localScale = originalScale;
    }

    // 피해 처리 함수
    public void TakeDamage(int damage)
    {
 
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
        monsterUI.UpdateHP(currentHp, data.maxHp);
    }

    // 사망 처리 함수
    public void OnDie()
    {
        Destroy(gameObject);
    }

    // 마우스 클릭 처리 (타겟 선택용)
    private void OnMouseDown()
    {
        if (BattleManager.instance != null && GameManager.instance.currentState == GameState.Battle)
        {
            BattleManager.instance.SelectTarget(this);
        }
    }

    // 선택 표시 토글 함수
    public void SetSelection(bool isSelected)
    {
        if (selectionMark != null) selectionMark.SetActive(isSelected);
    }
}
