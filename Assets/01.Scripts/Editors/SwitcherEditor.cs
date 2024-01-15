#if UNITY_EDITOR

using Cinemachine;
using InputControl;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VirtualCam;

[CustomEditor(typeof(CameraSwitcherInEditor))]
public class SwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Debug.Log("bb");
    }
    void OnSceneGUI()
    {

        Debug.Log("bb");
        CameraSwitcherInEditor _switcher = (CameraSwitcherInEditor)target;
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.KeyDown:
            {
                Debug.Log("a");
                _switcher.SwitchHere(e.keyCode);
                e.Use();
                    
                break;
            }
        }
    }
}

#endif