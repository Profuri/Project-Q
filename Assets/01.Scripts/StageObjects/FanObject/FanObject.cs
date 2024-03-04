using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class FanObject : InteractableObject
{
    [Header("Air Settings")] 
    [SerializeField] private AxisType _airDir;
    [SerializeField] private float _collisionSize;
    [SerializeField] private float _airMaxHeight;
    [SerializeField] private float _airPower;
    [SerializeField] private LayerMask _effectedMask;
    
    [Header("Fan Settings")]
    [SerializeField] private float _fanMaxSpeed;
    [SerializeField] private float _fanAccelerator;
    [SerializeField] private float _fanDecelerator;

    private Transform _fanTrm;
    private ParticleSystem _airParticle;
    
    private float _currentFanSpeed;

    private bool _enabled;

    public override void Awake()
    {
        base.Awake();

        var model = transform.Find("Model");
        _fanTrm = model.Find("Fan");
        _airParticle = _fanTrm.Find("AirParticle").GetComponent<ParticleSystem>();
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        
        RotateFan();
        FloatingOther();
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        EnableFan();
    }

    private void EnableFan()
    {
        _enabled = true;
        _airParticle.Play();
    }

    private void ReleaseFan()
    {
        _enabled = false;
        _airParticle.Stop();
    }
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (interactValue)
        {
            if (_enabled)
            {
                return;
            }
            
            EnableFan();
        }
        else
        {
            ReleaseFan();
        }
    }

    private void FloatingOther()
    {
        if (!_enabled)
        {
            return;
        }

        if (!CheckCollision(out var hits, out var size))
        {
            return;
        }
        
        for (var i = 0; i < size; i++)
        {
            var hit = hits[i];

            if (hit.collider.TryGetComponent(out ObjectUnit unit))
            {
                if (!unit.staticUnit)
                {
                    var airVelocity = unit.Rigidbody.velocity;
                    airVelocity.SetAxisElement(_airDir, _airPower);
                    unit.SetVelocity(airVelocity, false);       
                }
            }
        }
    }

    private bool CheckCollision(out RaycastHit[] hits, out int size)
    {
        hits = new RaycastHit[10];
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement(_airDir, 0.1f);
        size = Physics.BoxCastNonAlloc(
            transform.position,
            colSize / 2f,
            Vector3ExtensionMethod.GetAxisDir(_airDir),
            hits, 
            transform.rotation,
            _airMaxHeight,
            _effectedMask
        );
        return size > 0;
    }

    private void RotateFan()
    {
        _currentFanSpeed += (_enabled ? _fanAccelerator : -_fanDecelerator) * Time.deltaTime;
        _currentFanSpeed = Mathf.Clamp(_currentFanSpeed, 0, _fanMaxSpeed);
        _fanTrm.Rotate(new Vector3(0, _currentFanSpeed * Time.deltaTime, 0));
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var center = transform.position + Vector3ExtensionMethod.GetAxisDir(_airDir) * _airMaxHeight;
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement(_airDir, 0.1f);
        Gizmos.DrawWireCube(center, colSize);
    }
    
    private void OnValidate()
    {
        _airParticle = transform.Find("Model/Fan/AirParticle").GetComponent<ParticleSystem>();
        var particleMainSetting = _airParticle.main;
        particleMainSetting.startLifetime = _airMaxHeight / 10f;
    }

#endif
}
