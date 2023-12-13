using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage : MonoBehaviour
{
    //[HideInInspector] public Stage prevStage = null;
    [HideInInspector] public Stage  NextStage = null;
    [HideInInspector] public bool   IsEndStage = false;
    [HideInInspector] public int    CurStageNum = 0;
    public event Action OnStageChange;
    public bool IsPlayerEnter;

    public void Awake()
    {
        //�Ҵ��� ��� StageManager���� ���ٲ���
        //OnStageChange += () => gameObject.SetActive(true);
    }

    public void ActiveStage()
    {
        Debug.Log($"Active : {this.gameObject.name}");
        OnStageChange?.Invoke();
        gameObject.SetActive(true);
    }

    public void GoNext()
    {
        NextStage.ActiveStage();
        EmphasisNextStage();
    }

    private void EmphasisNextStage()
    {
        Transform camerTrm = CameraManager.Instance.CameraContainerTrm;
        Vector3 cameraDiff = CameraManager.Instance.CameraDiff;
        Vector3 nextPos = NextStage.transform.position + cameraDiff;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(4f);
        seq.Append(camerTrm.DOMove(nextPos, 1f)).SetEase(Ease.OutCirc);
        seq.AppendInterval(0.3f);
        seq.Append(camerTrm.DOMove(transform.position + cameraDiff, 0.5f));
        seq.OnComplete(() =>
        {
            //�÷��̾� �ٽ� �����̰�
            MakeBridge();
            GameManager.Instance.Player.SetEnableInput(true);    
        });
    }

    private void MakeBridge()
    {
        Vector3 startPos;

        Vector3 endPos; 
    }


    private void OnTriggerEnter(Collider collider)
    {
        PlayerController player;
        if(collider.TryGetComponent<PlayerController>(out player) && !IsPlayerEnter)
        {
            IsPlayerEnter = true;
            Transform camerTrm = CameraManager.Instance.CameraContainerTrm;
            Vector3 camerDiff = CameraManager.Instance.CameraDiff;
            //��� Next�Ѿ� ���� �� ���� ��� ���� ���������� ��������� ��
            Vector3 nextPos = transform.position + camerDiff;

            camerTrm.DOMove(nextPos, 0.5f);


            GetComponent<Collider>().enabled = false;
        }
    }


}
