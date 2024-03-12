using Core;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMark : PoolableMono
{
    [SerializeField] private float _rotateSpeed;
   
    private Coroutine _showingCoroutine;
    private static float _time = 1.5f;
    private Coroutine _rotateCoroutine;

    private Transform _ringTrm;
    private Material _tutMat;

    private void Awake()
    {
        _ringTrm = transform.Find("Ring");
        _tutMat = transform.Find("Visual").GetComponent<MeshRenderer>().material;
    }

    public override void OnPop()
    {
        if(_rotateCoroutine == null)
        {
            _rotateCoroutine = StartCoroutine(RotateCoroutine());
        }
        On();
    }

    public override void OnPush()
    {
        if(_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }
    }

    private void On()
    {
        var seq = DOTween.Sequence();

        _ringTrm.localScale = Vector3.up;
        _tutMat.SetFloat("_time", 0f);

        seq.Append(_ringTrm.DOScale(new Vector3(0.2f, 1f, 0.2f), _time));
        seq.Join(DOTween.To(() => _tutMat.GetFloat("_time"), value => _tutMat.SetFloat("_time", value), 0.9f, _time));
    }

    public void Off()
    {
        var seq = DOTween.Sequence();

        _ringTrm.localScale = new Vector3(0.2f, 1f, 0.2f);
        _tutMat.SetFloat("_time", 0.9f);

        seq.Append(_ringTrm.DOScale(new Vector3(0f, 1f, 0f), _time));
        seq.Join(DOTween.To(() => _tutMat.GetFloat("_time"), value => _tutMat.SetFloat("_time", value), 0f, _time));
        seq.AppendCallback(() => SceneControlManager.Instance.DeleteObject(this));
    }

    private IEnumerator RotateCoroutine()
    {
        Transform rotateTarget = transform;
        Transform lookAtTrm = Define.MainCam.transform;
        while(true)
        {
            rotateTarget.LookAt(lookAtTrm.position);
            yield return null;
        }
    }
}
