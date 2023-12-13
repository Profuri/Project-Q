using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    //[HideInInspector] public Stage prevStage = null;
    [HideInInspector] public Stage  NextStage = null;
    [HideInInspector] public bool   IsEndStage = false;
    [HideInInspector] public int    CurStageNum = 0;
    public event Action OnStageChange;

    public void Awake()
    {
        //�Ҵ��� ��� StageManager���� ���ٲ���
        OnStageChange += () => gameObject.SetActive(true);
    }

    public void ActiveStage()
    {
        OnStageChange?.Invoke();
    }

    public void GoNext()
    {
        NextStage.ActiveStage();
    }
}
