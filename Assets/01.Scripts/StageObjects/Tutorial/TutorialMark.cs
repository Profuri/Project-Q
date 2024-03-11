using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMark : PoolableMono
{
    [SerializeField] private float _rotateSpeed;
    private Coroutine _rotateCoroutine;

    public override void OnPop()
    {
        if(_rotateCoroutine == null)
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

    private IEnumerator RotateCoroutine()
    {
        Transform rotateTarget = null;
        while(true)
        {
            if(rotateTarget == null)
            {
                rotateTarget = transform.GetChild(0);
            }
            rotateTarget?.Rotate(Vector3.up * (_rotateSpeed * Time.deltaTime));
            yield return null;
        }
    }
}
