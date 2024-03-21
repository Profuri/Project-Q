using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickable
{
    public UnityEvent OnClickEvent;

    public void OnClickHandle()
    {
        OnClickEvent?.Invoke();
        Debug.Log(1);
    }
}