using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    private static HashSet<UIComponent> _uiHashSet = new HashSet<UIComponent>();
    public static int CurrentCnt => _uiHashSet.Count;



    public static void RegisterUI(UIComponent component)
    {
        if (_uiHashSet.Contains(component)) return;

        _uiHashSet.Add(component);
        ReloadCursor();

    }

    public static void UnRegisterUI(UIComponent component)
    {
        if(_uiHashSet.Contains(component))
        {
            _uiHashSet.Remove(component);
            ReloadCursor();
        }
        else
        {
            Debug.LogError($"This component is not registered!: {component}");
        }
    }

    public static void ClearUIHash()
    {
        _uiHashSet.Clear();
    }

    public static void ReloadCursor()
    {
        if(CurrentCnt < 1)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
