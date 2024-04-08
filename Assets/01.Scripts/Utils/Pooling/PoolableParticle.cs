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

    public void Play()
    {
        CoroutineManager.Instance.StartSafeCoroutine(GetInstanceID(), PlayRoutine());
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