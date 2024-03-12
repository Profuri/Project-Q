using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PoolableVFX : PoolableMono
{
    private VisualEffect _visualEffect;
    private Coroutine _runningCoroutine;

    private void Awake()
    {
        _runningCoroutine = null;
        _visualEffect = GetComponent<VisualEffect>();
    }

    public void SetPositionAndRotation(Vector3 position = default, Quaternion quaternion = default)
    {
        _visualEffect.transform.SetPositionAndRotation(position, quaternion);
    }
    
    public void Play()
    {
        if (_runningCoroutine is not null)
        {
            StopCoroutine(_runningCoroutine);
        }
        _runningCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        _visualEffect.Play();
        yield return new WaitUntil(() => _visualEffect.pause);
        _visualEffect.Stop();
        PoolManager.Instance.Push(this);
    }
    
    public override void OnPop()
    {
        _visualEffect.Stop();
    }

    public override void OnPush()
    {
    }
}
