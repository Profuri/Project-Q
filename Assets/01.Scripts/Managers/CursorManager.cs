using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    public static void SetCursorEnable(bool isVisible) => Cursor.visible = isVisible;
    public static void SetCursorLockState(CursorLockMode lockMode) => Cursor.lockState = lockMode;
}
