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
    private Transform _projectionConeTrm;
    private Transform _rotateTarget;
    private Material _tutMat;

    public override void OnPop()
    {
        _rotateTarget = transform.Find("RotateTarget");

        _ringTrm = _rotateTarget.Find("Ring");
        _tutMat = _rotateTarget.Find("Visual").GetComponent<MeshRenderer>().material;
        _projectionConeTrm = transform.Find("ProjectionCone");

        if (_rotateCoroutine == null)
        {
            _rotateCoroutine = StartCoroutine(RotateCoroutine());
        }
    }

    public override void OnPush()
    {
        if(_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }
    }

    public void On()
    {
        var seq = DOTween.Sequence();

        _ringTrm.localScale = Vector3.up;
        _tutMat.SetFloat("_time", 0f);
        _projectionConeTrm.localScale = new Vector3(10f, 0f, 10f);

        seq.Append(_ringTrm.DOScale(new Vector3(0.2f, 1f, 0.2f), _time));
        seq.Join(DOTween.To(() => _tutMat.GetFloat("_time"), value => _tutMat.SetFloat("_time", value), 0.9f, _time));
        seq.Join(_projectionConeTrm.DOScale(new Vector3(10f, 10f, 10f), _time));
    }

    public void Off()
    {
        var seq = DOTween.Sequence();

        _ringTrm.localScale = new Vector3(0.2f, 1f, 0.2f);
        _tutMat.SetFloat("_time", 0.9f);
        _projectionConeTrm.localScale = Vector3.one * 10f;

        seq.Append(_ringTrm.DOScale(new Vector3(0f, 1f, 0f), _time));
        seq.Join(DOTween.To(() => _tutMat.GetFloat("_time"), value => _tutMat.SetFloat("_time", value), 0f, _time));
        seq.Join(_projectionConeTrm.DOScale(new Vector3(10f, 0f, 10f), _time));
        seq.AppendCallback(() => SceneControlManager.Instance.DeleteObject(this));
    }

    private IEnumerator RotateCoroutine()
    {
        Transform rotateTarget = _rotateTarget;
        Transform lookAtTrm = Define.MainCam.transform;
        while(true)
        {
            rotateTarget.LookAt(lookAtTrm.position);
            yield return null;
        }
    }
}
