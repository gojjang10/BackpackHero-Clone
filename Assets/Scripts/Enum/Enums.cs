using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 타입 구분용 열거형
public enum ItemType
{
    Passive, // 장착만 해도 효과가 있음 (클릭 불가)
    Active   // 클릭해서 사용해야 함 (행동력 소모 등)
}

// 아이템 효과 발동 조건 구분용 열거형
public enum EffectTriggerType 
{   
    OnClick, // 클릭 시 발동
    OnPlace  // 그리드에 놓일 때 발동
}

public enum SearchDirection
{
    RightSide,     // 동적 방식: 내 오른쪽 면 전체를 보겠다.
    LeftSide,      // 동적 방식: 내 왼쪽 면 전체를 보겠다.
    UpSide,        // 동적 방식: 내 위쪽 면 전체를 보겠다.
    DownSide       // 동적 방식: 내 아래쪽 면 전체를 보겠다.
}
