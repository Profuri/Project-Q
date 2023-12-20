using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage : MonoBehaviour
{
    [HideInInspector] public Stage  PrevStage = null;
    [HideInInspector] public Stage  NextStage = null;
    [HideInInspector] public bool   IsEndStage = false;
    [HideInInspector] public int    CurStageNum = 0;
    public event Action OnStageChange;
    public bool IsPlayerEnter;


    private List<BridgeObject> _bridgeList;
    public List<BridgeObject> BridgeList => _bridgeList;

    public void Awake()
    {
        _bridgeList = new ();
        //�Ҵ��� ��� StageManager���� ���ٲ���
        // stage event�߰�
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
        seq.AppendInterval(3.5f);
        seq.Append(camerTrm.DOMove(nextPos, 1f)).SetEase(Ease.OutCirc);
        seq.AppendInterval(0.3f);
        seq.Append(camerTrm.DOMove(transform.position + cameraDiff, 0.3f).SetEase(Ease.Unset));
        //seq.Append(camerTrm.DOMove(GameManager.Instance.Player.transform.position + cameraDiff, 0.5f));
        seq.OnComplete(() =>
        {
            //�÷��̾� �ٽ� �����̰�
            MakeBridge();
            GameManager.Instance.Player.SetParent(NextStage.transform);
            GameManager.Instance.Player.SetEnableInput(true); 
        });
    }


    private void MakeBridge()
    {
        Transform bridgeStart = transform.Find("BridgeStartPos");
        Vector3 startPos = bridgeStart == null 
            ? GameManager.Instance.Player.transform.position
            : bridgeStart.position;


        Transform bridgeEnd = NextStage.transform.Find("BridgeEndPos");
        Vector3 endPos = bridgeEnd == null
            ? (NextStage.transform.position - bridgeStart.position).normalized * 10f
            : bridgeEnd.position;

        float interval = 0.2f;
        StartCoroutine(MakeBridgeCor(startPos, endPos, interval));
    }

    private IEnumerator MakeBridgeCor(Vector3 startPos, Vector3 endPos, float interval = 0.05f)
    {
        float t = interval / 2;
        while(t < 1)
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            BridgeObject bridgeObj = Instantiate(
                    StageManager.Instance.BridgePrefab
                    , pos + Vector3.down * 10f
                    , Quaternion.identity).GetComponent<BridgeObject>();
            bridgeObj.Move(pos, 0.45f);
            BridgeList.Add(bridgeObj);

            t += interval;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void EnterStage(Vector3 stageStartPos)
    {
        GameManager.Instance.Player.PlayerUnit.ReSetOriginPos(stageStartPos);
        if (PrevStage.BridgeList.Count > 0)
        {
            StartCoroutine(RemoveBridge(0.3f));
        }
    }

    private IEnumerator RemoveBridge(float delay)
    {
        for(int i = PrevStage.BridgeList.Count-1; i >= 0; i--)
        {
            BridgeObject bridge = PrevStage.BridgeList[i];
            bridge.Move(bridge.transform.position + Vector3.down * 10, 0.45f);
            yield return new WaitForSeconds(delay);
        }    
        //Destroy(bridge);
        PrevStage.gameObject.SetActive(false);
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

            camerTrm.DOMove(nextPos, 0.5f).SetEase(Ease.OutQuad);
            //GetComponent<Collider>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player;
        if (other.TryGetComponent<PlayerController>(out player))
        {
            IsPlayerEnter = false;
        }
    }
}
