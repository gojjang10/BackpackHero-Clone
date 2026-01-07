using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 타입 구분용 열거형
public enum ItemType
{
    Passive, // 장착만 해도 효과가 있음 (클릭 불가)
    Active   // 클릭해서 사용해야 함 (행동력 소모 등)
}

// 아이템 태그 구분용 열거형
public enum ItemTag
{
    None,
    Weapon,  // 무기
    Armor,   // 방어구
    Potion,  // 포션
    Gem,     // 보석
    Food     // 음식
}

// 아이템 효과 발동 조건 구분용 열거형
public enum EffectTriggerType 
{
    None,
    Passive,    // 배치 시 자동 적용 (시너지, 스탯 버프 등) - 재계산 대상 O
    OnClick,    // 클릭 시 발동 (포션 사용 등) - 재계산 대상 X
    OnTurnEnd   // 턴 종료 시 발동 - 재계산 대상 X
}

// 탐색 방향 구분용 열거형
public enum SearchDirection
{
    RightSide,     // 동적 방식: 내 오른쪽 면 전체를 보겠다.
    LeftSide,      // 동적 방식: 내 왼쪽 면 전체를 보겠다.
    UpSide,        // 동적 방식: 내 위쪽 면 전체를 보겠다.
    DownSide       // 동적 방식: 내 아래쪽 면 전체를 보겠다.
}
