using UnityEngine;

public class AirMaxHeightController : MonoBehaviour
{
    [SerializeField] private FanObject _fanObject;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private ParticleSystem _maxHeightParticle;

    public void SetHeight(float height)
    {
        transform.localPosition = new Vector3(0, height, 0);
    }
    
    public Vector3 GetWorldColSize()
    {
        var parentScale = transform.parent.localScale;
        var colScale = _boxCollider.size;
        return parentScale.Multiply(colScale);
    }

    public Vector3 GetLocalColSize()
    {
        return _boxCollider.size;
    }

    public void StopParticle()
    {
        _maxHeightParticle.Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_fanObject.Enabled && _maxHeightParticle.isStopped)
        {
            _maxHeightParticle.Play();
        }
    }

}