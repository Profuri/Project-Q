using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButton3D : UIComponent, IPointerClickHandler
{
    public UnityEvent OnClickEvent;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");    
    }
}