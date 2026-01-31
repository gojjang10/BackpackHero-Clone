using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundInteract : MonoBehaviour, IPointerDownHandler
{
    private GridInteract gridInteract;

    private void Start()
    {
        gridInteract = FindObjectOfType<GridInteract>();
    }

    // 배경(맨바닥)이 클릭되었을 때 호출됨
    public void OnPointerDown(PointerEventData eventData)
    {
        // GridInteract가 있고, 현재 아이템을 들고 있다면 -> 떨구기!
        if (gridInteract != null && gridInteract.selectedItem != null)
        {
            gridInteract.DropItemOutside();
        }
    }
}
