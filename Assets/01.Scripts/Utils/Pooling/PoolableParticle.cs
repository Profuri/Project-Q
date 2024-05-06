using System.Collections;
using UnityEngine;

public class PoolableParticle : PoolableMono
{
    private ParticleSystem _particleSystem;
    
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion quaternion)
    {
        _particleSystem.transform.SetPositionAndRotation(position, quaternion);
    }

    public void SetDuration(float duration)
    {
        if (_particleSystem.isPlaying)
        {
            Debug.LogWarning("[PoolableParticle] Duration can't set playing particle. stop particle");
            _particleSystem.Stop();
        }

        var mainModule = _particleSystem.main;
        mainModule.duration = duration;
    }

    public void Play()
    {
        StartSafeCoroutine("ParticlePlayRoutine", PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        _particleSystem.Play();
        yield return new WaitUntil(() => !_particleSystem.isPlaying);
        _particleSystem.Stop();
        PoolManager.Instance.Push(this);
    }
    
    public override void OnPop()
    {
        _particleSystem.Stop();
    }
    
    public override void OnPush()
    {
    }
}