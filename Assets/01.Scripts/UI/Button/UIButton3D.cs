using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickHandler
{
    public UnityEvent OnClickEvent;

    public void OnClickHandle()
    {
        OnClickEvent?.Invoke();
    }
}