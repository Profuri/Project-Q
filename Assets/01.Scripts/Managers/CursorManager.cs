using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    private static HashSet<MonoBehaviour> _uiHashSet = new HashSet<MonoBehaviour>();
    public static int CurrentCnt => _uiHashSet.Count;

    public static void RegisterUI(MonoBehaviour component)
    {
        if (_uiHashSet.Contains(component)) return;

        _uiHashSet.Add(component);
        ReloadCursor();
    }

    public static void UnRegisterUI(MonoBehaviour component)
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
