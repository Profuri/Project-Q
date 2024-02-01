using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class FanObject : InteractableObject
{
    [SerializeField] private Transform _fanTrm;
    [SerializeField] private AirMaxHeightController _maxHeightController;
    [SerializeField] private ParticleSystem _airParticle;
    
    [Header("Air Settings")]
    [SerializeField] private float _airMaxHeight;
    [SerializeField] private float _airPower;
    [SerializeField] private LayerMask _effectedMask;
    
    [Header("Fan Settings")]
    [SerializeField] private float _fanMaxSpeed;
    [SerializeField] private float _fanAccelerator;
    [SerializeField] private float _fanDecelerator;

    private float _currentFanSpeed;
    private Vector3 _poweredDir;

    private bool _enabled;
    public bool Enabled => _enabled;

    private void Update()
    {
        _poweredDir = transform.up;
        RotateFan();
        FloatingOther();
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
        _maxHeightController.StopParticle();
    }
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
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

            if (hit.collider.TryGetComponent(out PlayerController playerController))
            {
                var movementModule = playerController.GetModule<PlayerMovementModule>();
                movementModule.SetForce(_poweredDir * _airPower);
                // movementModule.SetVerticalVelocity(_airPower);
            }

            if (hit.collider.TryGetComponent(out InteractableObject interactableObject))
            {
                if (interactableObject.IsInteract)
                {
                    continue;
                }
                
                if (!interactableObject.Attribute.HasFlag(EInteractableAttribute.AFFECTED_FROM_AIR))
                {
                    continue;
                }

                if (!interactableObject.TryGetComponent<Rigidbody>(out var rigid))
                {
                    continue;
                }
                
                rigid.velocity = _poweredDir * _airPower;
            }
        }
    }

    private bool CheckCollision(out RaycastHit[] hits, out int size)
    {
        hits = new RaycastHit[10];
        size = Physics.BoxCastNonAlloc(
            transform.position,
            _maxHeightController.GetWorldColSize() / 2f,
            _poweredDir,
            hits, 
            transform.rotation,
            _airMaxHeight / 2f,
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
        if (_maxHeightController == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.matrix = _maxHeightController.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _maxHeightController.GetLocalColSize());
    }
    
    private void OnValidate()
    {
        if (_maxHeightController == null || _airParticle == null)
        {
            return;
        }
        
        _maxHeightController.SetHeight(_airMaxHeight);

        var particleMainSetting = _airParticle.main;
        particleMainSetting.startLifetime = _airMaxHeight / 10f * 1.5f;
    }

#endif
}
